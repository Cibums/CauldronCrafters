using System;
using UnityEngine;

[Serializable]
public class ColorAction : BaseAction
{
    public PaletteColor color;

    public override void InvokeAction()
    {
        base.InvokeAction();
        Color monsterColor = MonsterColor.GetColor(color);
        MonsterController.instance.SetColor(monsterColor);
    }

    public override string ActionText()
    {
        return $"Will set the color of the creature to <b>{color.ToString().ToUpper()}</b>";
    }
}
