using System;
using UnityEngine;

[Serializable]
public class BaseAction
{
    public virtual void InvokeAction()
    {
        //Animations and cool effects that will happen for each item
        GameController.instance.SummonSmokeParticle(MonsterController.instance.gameObject.transform.position, Color.white);
        AudioController.instance.PlaySound(1); //explosion
    }

    public virtual MonsterState UpdateStateFromAction(MonsterState oldState)
    {
        Debug.LogWarning("One of the sub-classes to BaseAction uses the implementation of BaseAction, instead of its own. It should not");
        return oldState;
    }

    public virtual string ActionText()
    {
        return "Unknown action o_O";
    }
}
