using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Timeline.TimelinePlaybackControls;

[CreateAssetMenu(fileName = "New Customer Request", menuName = "Customer")]
public class CustomerRequest : ScriptableObject
{
    [TextArea]
    public string RichMonsterDescription;
    public MonsterProperties WantedMonsterProperties;

    public string GetMonsterDescription()
    {
        if (string.IsNullOrEmpty(RichMonsterDescription))
        {
            return WantedMonsterProperties.AutomaticMonsterDescription();
        }

        return RichMonsterDescription;
    }
}

[Serializable]
public class MonsterProperySettings
{
    public string Trait;
    public AmountSetting AmountRule;
    public int Count;

    public enum AmountSetting
    {
        None,
        Minimum,
        Exact
    }
}

[Serializable]
public class MonsterProperties
{
    public bool ColorMatters = false;
    public PaletteColor PaletteColor = new PaletteColor();
    public bool SizeMatters = false;
    public MonsterSizeGetter MonsterSize = new MonsterSizeGetter();

    public List<MonsterProperySettings> otherProperties = new List<MonsterProperySettings>();

    public string AutomaticMonsterDescription()
    {
        return SimplifiedMonsterDescription();
    }

    public string SimplifiedMonsterDescription()
    {
        string ret = "<b>The customer wants:</b> \n";

        if (ColorMatters)
        {
            string colorStr = MonsterColor.GetColor(PaletteColor).color.ToHexString();
            ret += $"- Colored <color=#{colorStr}>{PaletteColor.ToString().ToUpper()}</color> \n";
        }

        if (SizeMatters)
        {
            ret += $"- Size to be {MonsterSize.ToString().ToUpper()} \n";
        }

        foreach (MonsterProperySettings property in otherProperties)
        {
            string exactCountText = GetCountLocalization(property.Count);
            string minimumCountText = GetCountLocalization(property.Count);

            if (property.AmountRule == MonsterProperySettings.AmountSetting.Exact)
            {
                ret += $"- {exactCountText} {property.Trait.ToUpper()} exactly \n";
                continue;
            }

            if (property.AmountRule == MonsterProperySettings.AmountSetting.Minimum)
            {
                ret += $"- At least {exactCountText} {property.Trait.ToUpper()} \n";
                continue;
            }

            if (property.AmountRule == MonsterProperySettings.AmountSetting.None)
            {
                ret += $"- At least {GetCountLocalization(1)} {property.Trait.ToUpper()} \n";
                continue;
            }

            ret += $"- {property.Trait.ToUpper()} \n";
        }

        return ret;
    }

    public string GetCountLocalization(int count)
    {
        switch (count)
        {
            case 0:
                return string.Empty;
            case 1:
                return "Kinda (1x)";
            case 2:
                return "Hella (2x)";
            case 3:
                return "Ridiculously (3x)";
            case 4:
                return "Outrageously (4x)";
            case 5:
                return "Stupendously (5x)";
            default:
                return string.Empty;
        }
    }
}

public class MonsterRatingReport
{
    public bool colorIsWrong;
    public bool sizeIsWrong;
    public HashSet<string> wrongProperties;

    public bool HadAtLeastOneWrong()
    {
        return colorIsWrong || sizeIsWrong || wrongProperties.Count > 0;
    }

    public MonsterRatingReport()
    {
        this.colorIsWrong = false;
        this.sizeIsWrong = false;
        wrongProperties = new HashSet<string>();
    }

    public MonsterRatingReport(bool colorIsWrong, bool sizeIsWrong, HashSet<string> wrongProperties)
    {
        this.colorIsWrong = colorIsWrong;
        this.sizeIsWrong = sizeIsWrong;
        this.wrongProperties = wrongProperties;
    }
}
