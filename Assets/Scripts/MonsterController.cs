using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public static MonsterController instance;
    private SpriteRenderer graphics;

    public PaletteColor currentColor;
    public MonsterSizeGetter currentSize = MonsterSizeGetter.Medium;

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

    public Dictionary<string, int> ComparableList()
    {
        Dictionary<string, int> traitCounts = new Dictionary<string, int>();

        foreach (string s in otherProperties)
        {
            if (traitCounts.ContainsKey(s))
            {
                traitCounts[s]++;
            }
            else
            {
                traitCounts.Add(s, 1);
            }
        }

        return traitCounts;
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
