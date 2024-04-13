using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddRemovePropertyAction : BaseAction
{
    public string property;
    public bool shouldRemove;
    public bool ableToIncrease;

    public override void InvokeAction()
    {
        base.InvokeAction();
        MonsterController.instance.ChangeProperty(property.ToLower(), shouldRemove, ableToIncrease);
    }

    public override string ActionText()
    {
        if (shouldRemove)
        {
            return $"If the creature is <b>{property.ToString().ToUpper()}</b>, remove that trait.";
        }
        else if (ableToIncrease)
        {
            return $"If the creature is already <b>{property.ToString().ToUpper()}</b>, increase the intensity of it. Othewise, make the creature <b>{property.ToString().ToUpper()}</b>";
        }
        else
        {
            return $"Will make the creature <b>{property.ToString().ToUpper()}</b>";
        }
    }
}
