using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public CustomerRequest[] allCustomerRequests;
    public HashSet<int> completedCustomers = new HashSet<int>();

    [SerializeField] private List<int> completedCustomersList = new List<int>(); //Only to see the hashset in the inspector

    [Header("Items")]
    public GameObject ItemPrefab;
    public Item[] allItems;
    public List<int> unlockedItems = new List<int>();

    [Header("Particles")]
    public ParticleSystem cauldronParticleSystem;
    public GameObject smokeParticlesPrefab;

    public static GameController instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void UnlockRandomItem()
    {
        if (allItems.Length == 0)
        {
            Debug.LogError("No items are available in allItems array.");
            return;
        }

        if (unlockedItems.Count >= allItems.Length)
        {
            Debug.LogError("All items are already unlocked.");
            return;
        }

        int randomIndex;
        do
        {
            randomIndex = UnityEngine.Random.Range(0, allItems.Length);
        } while (unlockedItems.Contains(randomIndex));

        unlockedItems.Add(randomIndex);

        Item itemUnlocked = allItems[randomIndex];

        UserInterfaceController.instance.ShowGeneralPopup(
            itemUnlocked.itemName, 
            $"You unlocked <b>{itemUnlocked.itemName}</b>\nThis is what it does:\n\n{itemUnlocked.GetActionsText()}"
        );

        Debug.Log("Unlocked item at index: " + randomIndex);
    }

    public void GoToMonsterView()
    {
        UserInterfaceController.instance.SetCustomerRequestVisibleState(false);
        MoveCamera(new Vector2(9.2f, 0.8f));
    }

    public void GoToItemsView()
    {
        MoveCamera(new Vector2(0,0));
    }

    public void MoveCamera(Vector2 position)
    {
        Vector3 newPosition = new Vector3(position.x, position.y, -10);

        StartCoroutine(MoveCameraCoroutine(newPosition));
    }

    private IEnumerator MoveCameraCoroutine(Vector3 newPosition)
    {
        float time = 0;
        float transitionDuration = 0.5f;
        Vector3 start = Camera.main.transform.position;

        while (time < transitionDuration)
        {
            Camera.main.transform.position = Vector3.Lerp(start, newPosition, time / transitionDuration);
            time += Time.deltaTime;
            yield return null;
        }

        // Ensure the camera is exactly in the final position at the end
        Camera.main.transform.position = newPosition;
    }

    void Start()
    {
        cauldronParticleSystem = GameObject.FindGameObjectWithTag("Cauldron").transform.GetComponentInChildren<ParticleSystem>();
        NextCustomer(false, true);
    }

    public CustomerRequest GetCurrentCustomerRequest()
    {
        return allCustomerRequests[currentCustomerRequestIndex];
    }

    private CustomerRequest GetRandomValidCustomerRequest() 
    {
        List<CustomerRequest> validCustomerRequests = allCustomerRequests
            .Select((request, index) => new { Request = request, Index = index })  // Project each request with its index
            .Where(x => !completedCustomers.Contains(x.Index))  // Filter out requests with indexes in the completedCustomers HashSet
            .Select(x => x.Request).ToList();  // Select only the request part for the result

        while (validCustomerRequests.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, validCustomerRequests.Count); // Get a random index
            CustomerRequest request = validCustomerRequests[index];  // Retrieve the request at this index
            if (RequestChecker.RequestIsPossibleWithUnlockedItems(request))
            {
                return request;  // Return the request if it passes the checker
            }
            validCustomerRequests.RemoveAt(index);  // Remove the request from the list if it fails the checker
        }

        throw new InvalidOperationException("No valid customer requests available.");
    }

    private int currentCustomerRequestIndex = 0;
    public void NextCustomer(bool retry, bool isFirst = false)
    {
        roundIsDone = false;

        if (!retry && !isFirst)
        {
            if (allCustomerRequests[currentCustomerRequestIndex] != null)
            {
                completedCustomers.Add(currentCustomerRequestIndex);
                completedCustomersList = completedCustomers.ToList();
            }
        }

        if (completedCustomers.Count % 2 == 0 && completedCustomers.Count > 0)
        {
            UnlockRandomItem();
        }

        MonsterController.instance.ResetMonster();
        ResetItems();

        CustomerRequest customerRequest = allCustomerRequests[currentCustomerRequestIndex];

        if (!retry)
        {
            customerRequest = GetRandomValidCustomerRequest();
            currentCustomerRequestIndex = Array.FindIndex(allCustomerRequests, x => x.Equals(customerRequest));
        }

        cauldronParticleSystem.gameObject.SetActive(false);

        UserInterfaceController.instance.SetCustomerRequestVisibleState(true);
        UserInterfaceController.instance.SetCustomerText(customerRequest.GetMonsterDescription());

        UserInterfaceController.instance.SetReportVisibleState(false);

        MoveCamera(new Vector2(0,0));

        //Debug.Log($"Customer Request Possibilty: {RequestChecker.RequestIsPossibleWithUnlockedItems(allCustomerRequests[customerIndex])}");
    }

    private void ResetItems()
    {
        MonsterController.instance.addedItems.Clear();

        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");

        foreach (GameObject item in items)
        {
            Destroy(item);
        }

        foreach (int i in unlockedItems)
        {
            Transform spawnedItem = Instantiate(ItemPrefab).transform;
            spawnedItem.position = new Vector3(UnityEngine.Random.Range(-7.5f, 0f), UnityEngine.Random.Range(-1.0f, 2.5f), 0);
            ItemBehaviour behaviour = spawnedItem.gameObject.GetComponent<ItemBehaviour>();
            behaviour.item = allItems[i];
            behaviour.UpdateGraphics();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnPlayerDoneClicked();
        }
    }

    private bool roundIsDone = false;
    public void OnPlayerDoneClicked()
    {
        if (MonsterController.instance.IsInvokingActions())
        {
            return;
        }

        if (roundIsDone)
        {
            return;
        }

        StartCoroutine(SummonAndCreateMonster());
    }



    IEnumerator SummonAndCreateMonster()
    {
        cauldronParticleSystem.gameObject.SetActive(true);
        GoToMonsterView();
        yield return new WaitForSeconds(0.5f);

        //Send particles from cauldron to monster fro 2 seconds
        yield return new WaitForSeconds(2); //Simulating for now

        //Summoning particles
        SummonSmokeParticle(MonsterController.instance.gameObject.transform.position, Color.white);
        MonsterController.instance.SetGraphicsShowState(true);

        MonsterController.instance.InvokeActionsInItems(1);
        StartCoroutine(CheckIfActionsAreDone());
    }

    IEnumerator CheckIfActionsAreDone()
    {
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => MonsterController.instance.IsInvokingActions() == false);
        OnActionsAreDone();
    }

    private void OnActionsAreDone()
    {
        roundIsDone = true;
        UserInterfaceController.instance.SetReportVisibleState(true);
        StartCoroutine(UserInterfaceController.instance.FillInReportIEnumerator());
    }

    public void SummonSmokeParticle(Vector3 position, Color color)
    {
        GameObject smokeParticles = Instantiate(smokeParticlesPrefab);
        smokeParticles.transform.position = new Vector3(position.x, position.y, smokeParticles.transform.position.z);
        var mainModule = smokeParticles.GetComponent<ParticleSystem>().main;
        mainModule.startColor = color;
        StartCoroutine(DestroyGameObjectAfterSeconds(smokeParticles, 1));
    }

    public IEnumerator DestroyGameObjectAfterSeconds(GameObject obj, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(obj);
    }
}
