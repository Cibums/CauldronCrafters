using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class AddRemovePropertyAction : BaseAction
{
    public string trait;
    public bool shouldRemove;
    public bool ableToIncrease;

    public override void InvokeAction()
    {
        base.InvokeAction();
        MonsterState oldState = MonsterController.instance.monsterState;
        MonsterController.instance.monsterState = UpdateStateFromAction(oldState);
    }

    public override MonsterState UpdateStateFromAction(MonsterState oldState)
    {
        MonsterState newState = oldState;

        newState.ChangeProperty(trait.ToLower(), shouldRemove, ableToIncrease);

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
            return $"If the creature is already <b>{trait.ToString().ToUpper()}</b>, increase the intensity of it. Othewise, make the creature <b>{trait.ToString().ToUpper()}</b>";
        }
        else
        {
            return $"Will make the creature <b>{trait.ToString().ToUpper()}</b>";
        }
    }
}
