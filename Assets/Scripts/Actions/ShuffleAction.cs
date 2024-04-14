using System.Collections.Generic;
using UnityEngine;

public class ShuffleAction : BaseAction
{
    public int count = 1;
    public override void InvokeAction()
    {
        base.InvokeAction();

        if (count < 1)
        {
            count = 1;
        }

        MonsterState oldState = MonsterController.instance.monsterState;
        MonsterController.instance.monsterState = UpdateStateFromAction(oldState); //Updates state

        
    }

    public override MonsterState UpdateStateFromAction(MonsterState oldState)
    {
        MonsterState newState = oldState;

        for (int i = 0; i < count; i++)
        {
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
                newState.addedItems.RemoveAt(originalIndex); // Remove the item from the original position
                newState.addedItems.Insert(newIndex, itemToMove); // Insert it at the new position

                Debug.Log($"Moved item from index {originalIndex} to {newIndex}");
            }
            else
            {
                Debug.LogWarning("Not enough items beyond invokedItemsCount to perform the operation");
            }
        }

        return newState;
    }

    public override string ActionText()
    {
        string s = count > 1 ? "s" : string.Empty;
        return $"<b>{count}</b> random item{s} in the <b>cauldron</b> will be invoked at a random time, instead of in the order it was put in";
    }
}
