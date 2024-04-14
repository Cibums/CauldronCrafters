using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MonsterState
{
    public PaletteColor currentColor;
    public MonsterSizeGetter currentSize = MonsterSizeGetter.Medium;
    public List<string> otherProperties = new List<string>();

    public void ChangeProperty(string prop, bool shouldRemove, bool ableToIncrease)
    {
        if (shouldRemove)
        {
            otherProperties.RemoveAll(item => item == prop);
        }
        else if (ableToIncrease)
        {
            otherProperties.Add(prop);
        }
        else if (!otherProperties.Contains(prop))
        {
            otherProperties.Add(prop);
        }
    }

    public Dictionary<string, int> ComparableList()
    {
        Dictionary<string, int> traitCounts = new Dictionary<string, int>();

        foreach (string s in otherProperties)
        {
            if (traitCounts.ContainsKey(s))
            {
                traitCounts[s]++;
            }
            else
            {
                traitCounts.Add(s, 1);
            }
        }

        return traitCounts;
    }

    public bool MatchesRequest(CustomerRequest request)
    {
        MonsterProperties monsterProperties = request.WantedMonsterProperties;
        List<MonsterProperySettings> settings = monsterProperties.otherProperties;
        Dictionary<string, int> currentMonsterProperties = ComparableList();

        //Checking size
        if (request.WantedMonsterProperties.SizeMatters)
        {
            if (currentSize != request.WantedMonsterProperties.MonsterSize)
            {
                return false;
            }
        }

        //Checking color
        if (request.WantedMonsterProperties.ColorMatters)
        {
            if (currentColor != request.WantedMonsterProperties.PaletteColor)
            {
                return false;
            }
        }

        foreach (MonsterProperySettings setting in settings)
        {
            if (setting.AmountRule == MonsterProperySettings.AmountSetting.Exact)
            {
                if (!currentMonsterProperties.ContainsKey(setting.Trait) || currentMonsterProperties[setting.Trait] != setting.Count)
                {
                    return false;
                }
            }
            else if (setting.AmountRule == MonsterProperySettings.AmountSetting.Minimum)
            {
                if (!currentMonsterProperties.ContainsKey(setting.Trait) || currentMonsterProperties[setting.Trait] < setting.Count)
                {
                    return false;
                }
            }
            else if (setting.AmountRule == MonsterProperySettings.AmountSetting.None)
            {
                if (!currentMonsterProperties.ContainsKey(setting.Trait))
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        return true;
    }
}
