using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Preplacements/None")]
public class Preplacement : ScriptableObject
{
    public virtual void Run(OverlapWFC wfc, bool[,] fill) { wfc.predetermined = new List<Predetermined>(); }

    public virtual void Run(SimpleTiledWFC wfc, bool[,] fill) { wfc.predetermined = new List<Predetermined>(); }

    protected void FillEmpty(OverlapWFC wfc, bool[,] fill)
    {
        for (int i = 0; i < fill.GetLength(1); i++)
        {
            for (int j = 0; j < fill.GetLength(0); j++)
            {
                if (AllEmpty(fill, i, j))
                    wfc.predetermined.Add(new Predetermined(i * wfc.width + j, new byte[] { 1, 1, 1, 1, 1, 1, 1, 1, 1 }));
            }
        }
    }

    private bool AllEmpty(bool[,] fill, int x, int y)
    {
        int sizeX = fill.GetLength(1);
        int sizeY = fill.GetLength(0);

        for (int i = 0; i < 3; i++)
        {
            int k = (x + i) % sizeX;
            for (int j = 0; j < 3; j++)
            {
                int l = (y + j) % sizeY;
                if (fill[l, k]) return false;
            }
        }

        return true;
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
