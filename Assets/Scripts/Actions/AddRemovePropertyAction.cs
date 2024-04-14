using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class AddRemovePropertyAction : BaseAction
{
    public string trait;
    public bool shouldRemove;
    public bool ableToIncrease;
    public int count = 1;

    public override void InvokeAction()
    {
        base.InvokeAction();
        MonsterState oldState = MonsterController.instance.monsterState;
        MonsterController.instance.monsterState = UpdateStateFromAction(oldState);
    }

    public override MonsterState UpdateStateFromAction(MonsterState oldState)
    {
        MonsterState newState = oldState;

        if (!ableToIncrease || count == 0)
        {
            count = 1;
        }

        for (int i = 0; i < count; i++)
        {
            if (newState.itemMultiplier > 1 && newState.enhanceCountdown != 0)
            {
                for (int i2 = 0; i2 < newState.itemMultiplier; i2++)
                {
                    newState.ChangeProperty(trait.ToLower(), shouldRemove, true);
                }
            }
            else
            {
                newState.ChangeProperty(trait.ToLower(), shouldRemove, ableToIncrease);
            }
        }

        return newState;
    }

    public override string ActionText()
    {
        if (shouldRemove)
        {
            return $"If the creature is <b>{trait.ToString().ToUpper()}</b>, remove that trait.";
        }
        else if (ableToIncrease)
        {
            return $"If the creature is already <b>{trait.ToString().ToUpper()}</b>, increase the intensity of it by {count}. Othewise, make the creature <b>{trait.ToString().ToUpper()}</b>";
        }
        else
        {
            return $"Will make the creature <b>{trait.ToString().ToUpper()}</b>";
        }
    }
}
