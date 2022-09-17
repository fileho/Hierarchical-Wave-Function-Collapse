using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Preplacements/None")]
public class Preplacement : ScriptableObject
{
    public virtual void Run(OverlapWFC wfc) { wfc.predetermined = new List<Predetermined>(); }

    public virtual void Run(SimpleTiledWFC wfc) { wfc.predetermined = new List<Predetermined>(); }

    protected void FillEmpty(OverlapWFC wfc)
    {
        var pattern = CreatePattern(wfc.N);

        for (int i = 0; i < wfc.fill.GetLength(1); i++)
        {
            for (int j = 0; j < wfc.fill.GetLength(0); j++)
            { 
                if (AnyEmpty(wfc.fill, i, j, wfc.N))
                    wfc.predetermined.Add(new Predetermined(i * wfc.width + j, pattern));
            }
        }
    }

    private static byte[] CreatePattern(int n)
    {
        byte[] pattern = new byte[n * n];
        for (var i = 0; i < pattern.Length; i++)
            pattern[i] = 1;
        return pattern;
    }

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
                if (!fill[l, k]) return true;
            }
        }

        return false;
    }

    protected bool[] GetEmpty(bool[,] fill, int x, int y)
    {
        bool[] ret = new bool[9];
        int index = 0;

        int sizeX = fill.GetLength(1);
        int sizeY = fill.GetLength(0);

        for (int i = 1; i >= -1; i--)
        {
            int k = x + i;
            for (int j = -1; j <= 1; j++)
            {
                int l = y + j;
                if (k < 0 || l < 0 || k >= sizeX || l >= sizeY) ret[index] = false;
                else ret[index] = fill[l, k];
                ++index;
            }
        }

        return ret;
    }
}
