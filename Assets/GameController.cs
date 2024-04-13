using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public CustomerRequest customerRequest;

    void Start()
    {
        UserInterfaceController.instance.SetCustomerText(customerRequest.GetMonsterDescription());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !MonsterController.instance.IsInvokingActions())
        {
            MonsterController.instance.InvokeActionsInItems(1);
            StartCoroutine(CheckIfActionsAreDone());
        }
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
}
