using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class DisperseAction : BaseAction
{
    public string replacementTrait = "funny";

    public override void InvokeAction()
    {
        base.InvokeAction();
        MonsterState oldState = MonsterController.instance.monsterState;
        MonsterController.instance.monsterState = UpdateStateFromAction(oldState); //Updates state
    }

    public override MonsterState UpdateStateFromAction(MonsterState oldState)
    {
        MonsterState newState = oldState;

        if (newState.otherProperties.Count == 0)
        {
            return newState; // Return as is if the list is empty
        }

        System.Random random = new System.Random();

        // Select a random string from the list
        int index = random.Next(newState.otherProperties.Count);
        string selectedString = newState.otherProperties[index];

        // Replace selected string and its duplicates
        for (int i = 0; i < newState.otherProperties.Count; i++)
        {
            if (newState.otherProperties[i] == selectedString)
            {
                newState.otherProperties[i] = replacementTrait;
            }
        }

        return newState;
    }

    public override string ActionText()
    {
        return $"Replaces one random trait and its intensity with <b>{replacementTrait.ToString().ToUpper()}</b>";
    }
}
