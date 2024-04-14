using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public static MonsterController instance;
    private SpriteRenderer graphics;

    public MonsterState monsterState;

    public List<Item> addedItems = new List<Item>();

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

    private bool isInvokingActions = false;
    private IEnumerator InvokeActionsInItemsIEnumerator(float seconds)
    {
        isInvokingActions = true;
        yield return new WaitForSeconds(seconds);

        foreach (Item item in addedItems) 
        {
            foreach (BaseAction action in item.actions)
            {
                action.InvokeAction();
                yield return new WaitForSeconds(seconds);
            }
        }
        isInvokingActions = false;
    }
}
