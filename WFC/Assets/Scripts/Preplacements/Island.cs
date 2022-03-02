using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Preplacements/Island")]
public class Island : Preplacement
{
    private byte value = 1;
    public override void Run(OverlapWFC wfc, bool[,] fill)
    {
        int n = wfc.N;
        byte[] pattern = new byte[n * n];

        for (int i = 0; i < n * n; i++)
            pattern[i] = value;
        Fill(wfc, pattern);

        wfc.predetermined.Add(new Predetermined(wfc.width * wfc.depth / 2 + wfc.width / 4, new byte[] {
            1,1,1,
            1,1,2,
            4,4,4
        }));
    }

    private void Fill(BaseWFC wfc, byte[] pattern)
    {
        wfc.predetermined = new List<Predetermined>();

        for (int i = 0; i < wfc.width; i++)
            wfc.predetermined.Add(new Predetermined(i, pattern));

        for (int i = 0; i < wfc.depth; i++)
            wfc.predetermined.Add(new Predetermined(i * wfc.width, pattern));
    }
}
