using System;
using UnityEngine;

[Serializable]
public class ColorAction : BaseAction
{
    public PaletteColor color;
    private Color _color;

    public override void InvokeAction()
    {
        MonsterState oldState = MonsterController.instance.monsterState;
        MonsterController.instance.monsterState = UpdateStateFromAction(oldState); //Updates state

        GameController.instance.SummonSmokeParticle(MonsterController.instance.gameObject.transform.position, _color);
        AudioController.instance.PlaySound(1); //explosion
        if (MonsterController.instance.monsterState.canChangeColor)
        {
            MonsterController.instance.SetColor(_color);
        }
    }

    public override MonsterState UpdateStateFromAction(MonsterState oldState)
    {
        MonsterState newState = oldState;

        (Color monsterColor, PaletteColor generatedPaletteColor) = MonsterColor.GetColor(color);
        _color = monsterColor;

        if (newState.canChangeColor)
        {
            newState.currentColor = generatedPaletteColor;
        }

        return newState;
    }

    public override string ActionText()
    {
        return $"Will set the color of the creature to <b>{color.ToString().ToUpper()}</b>";
    }
}
