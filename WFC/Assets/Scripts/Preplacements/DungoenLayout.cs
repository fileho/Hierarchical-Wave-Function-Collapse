using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Preplacements/DungeonLayout")]
public class DungoenLayout : Preplacement
{
    public override void Run(OverlapWFC wfc, bool[,] fill)
    {

        wfc.predetermined = new List<Predetermined>();
        byte[] pattern = new byte[]
        {
            4, 2, 2,
            4, 2, 2,
            4, 4, 4
        };

        wfc.predetermined.Add(new Predetermined(0, pattern));
        //     wfc.predetermined.Add(new Predetermined(wfc.width - 3, GeneratePattern(wfc, 3)));

        pattern = new byte[]
        {
            5, 5, 5,
            3, 3, 5,
            3, 3, 5
        };

        wfc.predetermined.Add(new Predetermined(wfc.width *(wfc.depth - 3) + wfc.width - 5, pattern));

      //  wfc.predetermined.Add(new Predetermined(wfc.width * wfc.depth - 5, value));
    }

    private byte[] GeneratePattern(OverlapWFC wfc, byte value)
    {
        int N = wfc.N;
        N *= N;
        byte[] ret = new byte[N];

        for (int i = 0; i < N; i++)
            ret[i] = value;

        return ret;
    }
}
