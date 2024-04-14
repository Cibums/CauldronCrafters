using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnhanceAction : BaseAction
{
    public int enhanceForce = 2;
    public int itemCount = 1;

    public override void InvokeAction()
    {
        base.InvokeAction();
        MonsterState oldState = MonsterController.instance.monsterState;
        MonsterController.instance.monsterState = UpdateStateFromAction(oldState); //Updates state
    }

    public override MonsterState UpdateStateFromAction(MonsterState oldState)
    {
        MonsterState newState = oldState;

        newState.EnhanceNextItem(enhanceForce, itemCount);

        return newState;
    }

    public override string ActionText()
    {
        string s = itemCount > 1 ? "s" : string.Empty;
        return $"The effects on <b>{itemCount}</b> item{s} invoked after this will be dubbled";
    }
}
