using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class SizeAction : BaseAction
{
    public MonsterSize size;

    public override void InvokeAction()
    {
        base.InvokeAction();
        float monsterSize = GetSize(size);
        MonsterController.instance.SetSize(monsterSize);
    }

    public override string ActionText()
    {
        if (size == MonsterSize.Random)
        {
            // Exclude the 'Random' and 'Increase' and 'Decrease' enum value by limiting the range
            int maxEnumIndex = System.Enum.GetValues(typeof(MonsterSize)).Length - 4;
            size = (MonsterSize)Random.Range(0, maxEnumIndex + 1);
        }

        if (size == MonsterSize.Increase || size == MonsterSize.Decrease)
        {
            return $"Will <b>{size.ToString().ToUpper()}</b> the size of the creature.";
        }

        return $"Will set the size of the creature to <b>{size.ToString().ToUpper()}</b>";
    }

    private float GetSize(MonsterSize size)
    {
        switch (size)
        {
            case MonsterSize.Small:
                return 0.5f;
            case MonsterSize.Medium:
                return 1f;
            case MonsterSize.Large:
                return 1.5f;
            case MonsterSize.Increase:
                return Mathf.Clamp(MonsterController.instance.gameObject.transform.localScale.x + 0.5f, 0.5f, 1.5f);
            case MonsterSize.Decrease:
                return Mathf.Clamp(MonsterController.instance.gameObject.transform.localScale.x - 0.5f, 0.5f, 1.5f);
            default:
                return 1f;
        }
    }
}

public enum MonsterSize
{
    Small,
    Medium,
    Large,
    Decrease,
    Increase,
    Random
}

public enum MonsterSizeGetter
{
    Small,
    Medium,
    Large
}
