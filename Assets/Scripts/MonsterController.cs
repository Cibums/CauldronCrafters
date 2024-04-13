using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public static MonsterController instance;
    private SpriteRenderer graphics;

    [HideInInspector] public PaletteColor? currentColor;
    [HideInInspector] public MonsterSizeGetter currentSize = MonsterSizeGetter.Medium;

    public List<Item> addedItems = new List<Item>();
    public List<string> otherProperties = new List<string>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        graphics = gameObject.GetComponentInChildren<SpriteRenderer>();
    }

    public void SetColor(Color color)
    {
        graphics.color = color;
    }

    public void SetSize(float size)
    {
        transform.localScale = new Vector2(size, size);
    }

    public void ChangeProperty(string prop, bool shouldRemove, bool ableToIncrease)
    {
        if (shouldRemove)
        {
            otherProperties.RemoveAll(item => item == prop);
        }
        else if (ableToIncrease)
        {
            otherProperties.Add(prop);
        }
        else if(!otherProperties.Contains(prop))
        {
            otherProperties.Add(prop);
        }
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
