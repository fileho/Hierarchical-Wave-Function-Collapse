using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Preplacements/WorldMapSingleWFC")]
public class WorldMapSinglewfc : Preplacement
{
    public override void Run(OverlapWFC wfc)
    {
        Fill(wfc, GeneratePattern(1, 3));

        var p = GeneratePattern(3, wfc.N);
        wfc.predetermined.Clear();

        wfc.predetermined.Add(new Predetermined(wfc.width * wfc.depth / 2 + wfc.width / 2, p));
    }

    byte[] GeneratePattern(int value, int N)
    {
        byte[] ret = new byte[N * N];

        for (var i = 0; i < ret.Length; i++) 
            ret[i] = (byte) value;

        return ret;
    }

    private void Fill(BaseWFC wfc, byte[] pattern)
    {
        wfc.predetermined = new List<Predetermined>();

        for (int i = 0; i < wfc.width; i++)
            wfc.predetermined.Add(new Predetermined(wfc.width * (wfc.depth - 1) + i, pattern));
    }
}
