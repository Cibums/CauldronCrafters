using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public static MonsterController instance;
    private SpriteRenderer graphics;

    public MonsterState monsterState;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        monsterState = new MonsterState();
        graphics = gameObject.GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        SetGraphicsShowState(false);
    }

    public void SetColor(Color color)
    {
        graphics.color = color;
    }

    public void ResetMonster()
    {
        SetColor(Color.white);
        SetSize(1.0f);
        monsterState = new MonsterState();
        graphics.gameObject.SetActive(false);
    }

    public void SetSize(float size)
    {
        transform.localScale = new Vector2(size, size);
    }

    public void SetGraphicsShowState(bool state)
    {
        graphics.gameObject.SetActive(state);
    }

    public void InvokeActionsInItems(float seconds)
    {
        StartCoroutine(InvokeActionsInItemsIEnumerator(seconds));
    }

    public bool IsInvokingActions()
    {
        return isInvokingActions;
    }

    public int invokedItemsCount = 0;
    private bool isInvokingActions = false;

    private IEnumerator InvokeActionsInItemsIEnumerator(float seconds)
    {
        invokedItemsCount = 0;
        isInvokingActions = true;
        yield return new WaitForSeconds(seconds);

        DoActionsForItem(seconds);
        
        isInvokingActions = false;
    }

    private void DoActionsForItem(float seconds)
    {
        List<Item> items = monsterState.addedItems;

        int index = 0;
        foreach (Item item in items)
        {
            if (monsterState.itemsWereChanged)
            {
                monsterState.itemsWereChanged = false;
                DoActionsForItem(seconds);
            }

            if (invokedItemsCount > index)
            {
                index++;
                continue;
            }

            StartCoroutine(DoAction(item, seconds));

            if (monsterState.enhanceCountdown > 0)
            {
                monsterState.enhanceCountdown--;
            }

            invokedItemsCount++;
            index++;
        }
    }

    private IEnumerator DoAction(Item item, float seconds)
    {
        foreach (BaseAction action in item.actions)
        {
            action.InvokeAction();
            yield return new WaitForSeconds(seconds);
        }
    }
}
