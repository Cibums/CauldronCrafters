using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public CustomerRequest customerRequest;
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

    public void GoToMonsterView()
    {
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
        cauldronParticleSystem.gameObject.SetActive(false);
        UserInterfaceController.instance.SetCustomerText(customerRequest.GetMonsterDescription());
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
        if (MonsterController.instance.IsInvokingActions() || roundIsDone)
        {
            return;
        }

        roundIsDone = true;
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
        (int rating, MonsterRatingReport report) = customerRequest.WantedMonsterProperties.GetComparisonRating();
    }

    private void SummonSmokeParticle(Vector3 position, Color color)
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
