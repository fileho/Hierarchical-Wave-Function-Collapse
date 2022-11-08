using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hwfc
{
/// <summary>
/// Base class for the tile preplacing.
/// Derive from this class and override Run(OverlapWFC) and Run(SimpleTiledWFC).
/// Overriding just one of them is sufficient.
/// </summary>

[CreateAssetMenu(menuName = "SO/Preplacements/None")]
public class Preplacement : ScriptableObject
{
    public virtual void Run(OverlapWFC wfc)
    {
        wfc.predetermined = new List<Predetermined>();
    }

    public virtual void Run(SimpleTiledWFC wfc)
    {
        wfc.predetermined = new List<Predetermined>();
    }

    // Preplace solid pattern to all areas that will be removed
    protected void FillEmpty(OverlapWFC wfc, int value = 1)
    {
        var pattern = CreatePattern(wfc.N, value);

        for (int i = 0; i < wfc.fill.GetLength(1); i++)
        {
            for (int j = 0; j < wfc.fill.GetLength(0); j++)
            {
                if (AnyEmpty(wfc.fill, i, j, wfc.N))
                    wfc.predetermined.Add(new Predetermined(i * wfc.width + j, pattern));
            }
        }
    }

    private static byte[] CreatePattern(int n, int value)
    {
        byte[] pattern = new byte[n * n];
        for (var i = 0; i < pattern.Length; i++)
            pattern[i] = (byte)value;
        return pattern;
    }

    // Checks whether the areas around the given point is all empty
    private static bool AnyEmpty(bool[,] fill, int x, int y, int size)
    {
        int sizeX = fill.GetLength(1);
        int sizeY = fill.GetLength(0);

        // N can cause artifacts
        size--;

        for (int i = 0; i < size; i++)
        {
            int k = (x + i + sizeX) % sizeX;
            for (int j = 0; j < size; j++)
            {
                int l = (y + j + sizeY) % sizeY;
                if (!fill[l, k])
                    return true;
            }
        }

        return false;
    }
}
}
