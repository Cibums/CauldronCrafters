using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PaletteColor
{
    Blue,
    Green,
    Red,
    Orange,
    Yellow,
    Cyan,
    Pink,
    Purple,
    Random
}

public static class MonsterColor
{
    public static Color GetColor(PaletteColor color)
    {
        if (color == PaletteColor.Random)
        {
            // Exclude the 'Random' enum value by limiting the range
            int maxEnumIndex = System.Enum.GetValues(typeof(PaletteColor)).Length - 2;
            color = (PaletteColor)Random.Range(0, maxEnumIndex + 1);
        }

        switch (color)
        {
            case PaletteColor.Blue:
                return Color.blue;
            case PaletteColor.Green:
                return Color.green;
            case PaletteColor.Red:
                return Color.red;
            case PaletteColor.Orange:
                return new Color(1f, 0.64f, 0f);
            case PaletteColor.Yellow:
                return Color.yellow;
            case PaletteColor.Cyan:
                return Color.cyan;
            case PaletteColor.Pink:
                return new Color(1f, 0.75f, 0.8f); // RGB for pink
            case PaletteColor.Purple:
                return new Color(0.5f, 0f, 0.5f); // RGB for purple
            default:
                Debug.LogError("Color not defined in the palette!");
                return Color.white;
        }
    }
}