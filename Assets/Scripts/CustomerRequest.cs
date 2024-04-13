using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

    public (int rating, MonsterRatingReport report) GetComparisonRating()
    {
        int comparisonRating = 0;
        MonsterRatingReport report = new MonsterRatingReport();

        foreach (MonsterProperySettings property in otherProperties)
        {
            int count = MonsterController.instance.otherProperties.Count(item => item == property.Trait);

            if ((property.AmountRule == MonsterProperySettings.AmountSetting.Exact && property.Count != count))
            {
                report.wrongProperties.Add(property.Trait);
                continue;
            }

            if ((property.AmountRule == MonsterProperySettings.AmountSetting.Minimum && property.Count < count))
            {
                report.wrongProperties.Add(property.Trait);
                continue;
            }

            comparisonRating += count;
        }

        if (ColorMatters && PaletteColor == MonsterController.instance.currentColor)
        {
            comparisonRating += 1;
        }
        else
        {
            report.colorIsWrong = true;
        }

        if (ColorMatters && PaletteColor == MonsterController.instance.currentColor)
        {
            comparisonRating += 1;
        }
        else
        {
            report.sizeIsWrong = true;
        }

        return (comparisonRating, report);
    }

    public string AutomaticMonsterDescription()
    {
        return SimplifiedMonsterDescription();
    }

    public string SimplifiedMonsterDescription()
    {
        string ret = "The creature should have these traits: \n";

        if (ColorMatters)
        {
            ret += $"- Colored {PaletteColor.ToString().ToUpper()} \n";
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

    private string GetCountLocalization(int count)
    {
        switch (count)
        {
            case 0:
                return string.Empty;
            case 1:
                return "Kinda (1x)";
            case 2:
                return "Mighty (2x)";
            case 3:
                return "Hella (3x)";
            case 4:
                return "Ridiculously (4x)";
            case 5:
                return "Outrageously (5x)";
            case 6:
                return "Stupendously (6x)";
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
