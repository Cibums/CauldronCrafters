using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

[CustomEditor(typeof(Item))]
public class ItemEditor : Editor
{
    private Item item;
    private SerializedObject serializedItem;
    private Type[] actionTypes;

    private void OnEnable()
    {
        item = target as Item;
        serializedItem = new SerializedObject(item);

        // Load subclasses of BaseAction
        actionTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsSubclassOf(typeof(BaseAction)) && !type.IsAbstract)
            .ToArray();
    }

    public override void OnInspectorGUI()
    {
        serializedItem.Update();

        EditorGUI.BeginChangeCheck();

        // Manually draw the properties you excluded, or use serialized properties
        SerializedProperty itemNameProp = serializedItem.FindProperty("itemName");
        EditorGUILayout.PropertyField(itemNameProp);

        SerializedProperty graphicsProp = serializedItem.FindProperty("graphics");
        EditorGUILayout.PropertyField(graphicsProp);

        DrawPropertiesExcluding(serializedObject, "itemName", "graphics", "actions", "actionData", "actionTypes");

        EditorGUILayout.LabelField("Item Actions", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if (item.actions != null)
        {
            for (int i = item.actions.Count - 1; i >= 0; i--)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(item.actions[i].GetType().Name, EditorStyles.boldLabel);

                if (GUILayout.Button("Remove"))
                {
                    item.actions.RemoveAt(i);
                    EditorUtility.SetDirty(item);
                    continue; // Skip drawing properties if the item is removed
                }
                EditorGUILayout.EndHorizontal();

                DrawActionProperties(item.actions[i]);
            }
        }

        if (GUILayout.Button("Add New Action"))
        {
            ShowAddActionMenu();
        }

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, "Item Changed");
            serializedItem.ApplyModifiedProperties();
            EditorUtility.SetDirty(item);
        }
    }

    private void DrawActionProperties(BaseAction action)
    {
        var fields = action.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (var field in fields)
        {
            object value = field.GetValue(action);
            Type type = field.FieldType;

            EditorGUI.BeginChangeCheck();
            value = DrawField(type, value, field.Name);

            if (EditorGUI.EndChangeCheck())
            {
                field.SetValue(action, value);
                EditorUtility.SetDirty(item);
            }
        }
    }

    private object DrawField(Type type, object value, string fieldName)
    {
        if (type == typeof(int))
        {
            return EditorGUILayout.IntField(fieldName, (int)value);
        }
        else if (type == typeof(float))
        {
            return EditorGUILayout.FloatField(fieldName, (float)value);
        }
        else if (type == typeof(string))
        {
            return EditorGUILayout.TextField(fieldName, (string)value);
        }
        else if (type == typeof(bool))
        {
            return EditorGUILayout.Toggle(fieldName, (bool)value);
        }
        else if (type == typeof(Color))
        {
            return EditorGUILayout.ColorField(fieldName, (Color)value);
        }
        else if (type == typeof(PaletteColor))
        {
            return EditorGUILayout.EnumPopup(fieldName, (PaletteColor)value);
        }
        else if (type == typeof(MonsterSize))
        {
            return EditorGUILayout.EnumPopup(fieldName, (MonsterSize)value);
        }
        // Add more types as needed
        else
        {
            return null;
        }
    }

    private void ShowAddActionMenu()
    {
        GenericMenu menu = new GenericMenu();
        foreach (Type actionType in AppDomain.CurrentDomain.GetAssemblies()
             .SelectMany(assembly => assembly.GetTypes())
             .Where(type => type.IsSubclassOf(typeof(BaseAction)) && !type.IsAbstract))
        {
            menu.AddItem(new GUIContent(actionType.Name), false, () => AddAction(actionType));
        }
        menu.ShowAsContext();
    }

    private void AddAction(Type actionType)
    {
        BaseAction instance = (BaseAction)Activator.CreateInstance(actionType);
        if (instance != null)
        {
            item.actions.Add(instance);
            EditorUtility.SetDirty(item);
        }
    }
}
