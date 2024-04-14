using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class RequestChecker
{
    public static bool RequestIsPossibleWithUnlockedItems(CustomerRequest request)
    {
        Debug.Log($"<b>Testing the customer {request.name}</b>");
        HashSet<Item> itemsToTest = GetItemsToIterate(request);
        return CheckCombinations(new List<Item>(itemsToTest), request, new List<Item>());
    }

    private static bool RequestFulfilled(List<Item> currentSequence, CustomerRequest request)
    {
        MonsterState testState = new MonsterState();

        foreach (Item item in currentSequence)
        {
            Debug.Log($"Adding the effects of {item.itemName}");

            foreach (var action in item.actions)
            {
                //Invoke action on testState
                MonsterState oldState = testState;
                testState = action.UpdateStateFromAction(oldState);
            }
        }

        return testState.MatchesRequest(request);
    }

    private static bool CheckCombinations(List<Item> availableItems, CustomerRequest request, List<Item> currentSequence)
    {
        // Base case: If current sequence of items fulfills the request, return true
        if (RequestFulfilled(currentSequence, request))
        {
            string sequence = string.Join("\n- ", currentSequence.Select(item => item.itemName));
            if (currentSequence.Any()) sequence = "- " + sequence;

            Debug.Log($"<color=green>This sequence of items worked:\n {sequence}</color>");
            return true;
        }

        // Recursive case: Try adding each available item in sequence and recurse
        for (int i = 0; i < availableItems.Count; i++)
        {
            Item itemToAdd = availableItems[i];
            currentSequence.Add(itemToAdd);  // Add item to current sequence
            List<Item> remainingItems = new List<Item>(availableItems);  // Make a copy of remaining items
            remainingItems.RemoveAt(i);  // Remove the item added to the sequence

            // Recurse with the new sequence and remaining items
            if (CheckCombinations(remainingItems, request, currentSequence))
            {
                return true;
            }

            Debug.Log($"<color=red>No combination worked for the customer {request.name}</color>");
            // Backtrack: remove the item last added when returning from recursion
            currentSequence.RemoveAt(currentSequence.Count - 1);
        }

        // If no combination works, return false
        return false;
    }

    private static HashSet<Item> GetItemsToIterate(CustomerRequest request)
    {
        Debug.Log($"Getting items to iterate through");
        HashSet<Item> itemsToIterate = new HashSet<Item>();

        if (request.WantedMonsterProperties.ColorMatters)
        {
            //Gets all of the items that could in theory change the color to the rigth color
            var colorItems = GameController.instance.allItems.Where(item =>
                item.actions.Any(action =>
                    action is ColorAction colorAction && (colorAction.color == request.WantedMonsterProperties.PaletteColor || colorAction.color == PaletteColor.Random)))
                .ToList();

            itemsToIterate.UnionWith(colorItems);
        }

        if (request.WantedMonsterProperties.SizeMatters)
        {
            //Gets all of the items that could in theory change the size to the rigth size
            var sizeItems = GameController.instance.allItems.Where(item =>
                item.actions.Any(action =>
                    action is SizeAction sizeAction))
                .ToList();

            itemsToIterate.UnionWith(sizeItems);
        }

        foreach (MonsterProperySettings settings in request.WantedMonsterProperties.otherProperties)
        {
            //Gets all of the items that could in theory change the property that the customer wants changed
            var traitItems = GameController.instance.allItems.Where(item =>
                item.actions.Any(action =>
                    action is AddRemovePropertyAction propertyAction && propertyAction.trait.ToLower() == settings.Trait.ToLower()))
                .ToList();

            itemsToIterate.UnionWith(traitItems);
        }

        foreach (Item i in itemsToIterate)
        {
            Debug.Log($"- {i.itemName}");
        }

        return itemsToIterate;
    }
}
