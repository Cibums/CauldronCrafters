using System;
using UnityEngine;

[Serializable]
public class BaseAction
{
    public virtual void InvokeAction()
    {
        //Animations and cool effects that will happen for each item
        GameController.instance.SummonSmokeParticle(MonsterController.instance.gameObject.transform.position, Color.white);
    }

    public virtual string ActionText()
    {
        return "Unknown action o_O";
    }
}
