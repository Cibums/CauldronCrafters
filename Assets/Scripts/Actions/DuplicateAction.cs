using System;
using System.Collections.Generic;
using UnityEngine;

public class DuplicateAction : BaseAction
{
    public override void InvokeAction()
    {
        base.InvokeAction();
        MonsterState oldState = MonsterController.instance.monsterState;
        MonsterController.instance.monsterState = UpdateStateFromAction(oldState); //Updates state
    }

    public override MonsterState UpdateStateFromAction(MonsterState oldState)
    {
        MonsterState newState = oldState;

        int invokedItemsCount = MonsterController.instance.invokedItemsCount;
        List<Item> items = newState.addedItems;

        // Check if there are enough items beyond the invokedItemsCount
        if (invokedItemsCount < items.Count - 1)
        {
            System.Random rand = new System.Random();

            // Get a random index for the item to move (ensuring it's beyond invokedItemsCount)
            int originalIndex = rand.Next(invokedItemsCount + 1, items.Count);

            // Get a random index to move the item to (also beyond invokedItemsCount)
            int newIndex = rand.Next(invokedItemsCount + 1, items.Count);

            // Move the item
            Item itemToMove = items[originalIndex];
            newState.itemsWereChanged = true;
            newState.addedItems.Insert(newIndex, itemToMove); // Insert it at the new position
        }
        else
        {
            Debug.LogWarning("Not enough items beyond invokedItemsCount to perform the operation");
        }

        return newState;
    }

    public override string ActionText()
    {
        return $"Duplicates one random item in the <b>cauldron</b>";
    }
}
