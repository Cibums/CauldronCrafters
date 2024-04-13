using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Magical Item", menuName = "Item")]
public class Item : ScriptableObject, ISerializationCallbackReceiver
{
    public string itemName;

    public Sprite graphics;

    public List<BaseAction> actions = new List<BaseAction>();

    public string GetActionsText()
    {
        string ret = string.Empty;
        foreach (BaseAction action in actions)
        {
            string actionText = action.ActionText();
            Debug.Log($"Action:{actionText}!");

            if( actionText != string.Empty)
            {
                ret += actionText + "\n";
            }
        }

        if (string.IsNullOrEmpty(ret))
        {
            return "<i>This item has no actions</i>";
        }

        return ret;
    }

    [SerializeField] private List<string> actionData = new List<string>();
    [SerializeField] private List<string> actionTypes = new List<string>();

    public void OnBeforeSerialize()
    {
        actionData.Clear();
        actionTypes.Clear();

        foreach (var action in actions)
        {
            actionData.Add(JsonUtility.ToJson(action));
            actionTypes.Add(action.GetType().AssemblyQualifiedName);
        }
    }

    public void OnAfterDeserialize()
    {
        actions.Clear();
        for (int i = 0; i < actionData.Count; i++)
        {
            Type type = Type.GetType(actionTypes[i]);
            var action = JsonUtility.FromJson(actionData[i], type) as BaseAction;
            actions.Add(action);
        }
    }
}
