using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Preplacements/DenseRoom")]
public class DenseRoom : Preplacement
{
    public override void Run(OverlapWFC wfc, bool[,] fill)
    {
        wfc.predetermined.Clear();

        byte[] bottomLeft = new byte[]
        {
            1, 2, 3,
            1, 2, 2,
            1, 1, 1
        };

        byte[] bottomRight = new byte[]
        {
            3, 2, 1,
            2, 2, 1,
            1, 1, 1
        };

        byte[] topLeft = new byte[]
        {
            1, 1, 1,
            1, 2, 2,
            1, 2, 3
        };

        byte[] topRight = new byte[]
        {
            1, 1, 1,
            2, 2, 1,
            3, 2, 1
        };


        byte[][] corners = new byte[][] {bottomLeft, bottomRight, topLeft, topRight};
        (int, int)[] offsets = new (int, int)[]
        {
            (0, 0),
            (-2, 0),
            (0, -2),
            (-2, -2)
        };

        for (int i = 0; i < fill.GetLength(0); i++)
        {
            for (int j = 0; j < fill.GetLength(1); j++)
            {
                var p = GetEmpty(fill, j, i);

                for (var index = 0; index < corners.Length; index++)
                {
                    var pattern = corners[index];
                    if (IsSame(p, ToBoolArray(pattern)))
                    {
                        int wfcIndex = i + offsets[index].Item1 + (j + offsets[index].Item2) * wfc.width;
                        if (wfcIndex >= 0)
                            wfc.predetermined.Add(new Predetermined(wfcIndex, pattern));
                        break;
                    }
                }
            }
        }
    }

    private bool IsSame(bool[] a, bool[] b)
    {
        for (int i = 0; i < a.Length; i++)
        {
            if (a[i] != b[i]) return false; 
        }

        return true;
    }

    private bool[] ToBoolArray(byte[] pattern)
    {
        bool[] ret = new bool[pattern.Length];
        for (int i = 0; i < pattern.Length; i++) ret[i] = pattern[i] > 1;

        return ret;
    }
}
