using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Postprocessing/DenseRoom")]
public class DenseRoomPost : WfcPostprocessing
{
    public GameObject wall;

    public override void Run(OverlapWFC wfc)
    {
        CheckBorders(wfc);
    }

    private void CheckBorders(OverlapWFC wfc)
    {
        for (int i = 0; i < wfc.fill.GetLength(1); i++)
        {
            for (int j = 0; j < wfc.fill.GetLength(0); j++)
            {
                if (!wfc.fill[j, i])
                    continue;

           //     if (wfc.rendering[j,i].GetComponent<TileType>())


                if (AnyEmpty(i, j, wfc.fill))
                {
                    ReplaceTile(wfc, j, i, wall);
                }
            }
        }
    }

    private bool AnyEmpty(int i, int j, bool[,] fill)
    {
        int w = fill.GetLength(1);
        int h = fill.GetLength(0);

        for (int k = -1; k <= 1; k++)
        {
            for (int l = -1; l <= 1; l++)
            {
                int x = (i + k + w) % w;
                int y = (j + l + h) % h;

                if (!fill[y, x])
                    return true;
            }
        }

        return false;
    }
}
