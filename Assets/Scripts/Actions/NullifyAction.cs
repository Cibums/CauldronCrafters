using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NullifyAction : BaseAction
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

        newState.canChangeColor = false;

        return newState;
    }

    public override string ActionText()
    {
        return $"After this, the creature can no longer change color";
    }
}
