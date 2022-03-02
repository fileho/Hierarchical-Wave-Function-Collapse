using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Preplacements/Borders")]
public class Borders : Preplacement
{
    public byte value;
    public override void Run(OverlapWFC wfc, bool[,] fill)
    {
        int n = wfc.N;
        byte[] pattern = new byte[n*n];

        for (int i = 0; i < n * n; i++)
            pattern[i] = value;
        Fill(wfc, pattern);
        FillEmpty(wfc, fill);
    }

    public override void Run(SimpleTiledWFC wfc, bool[,] fill)
    {
        byte[] pattern = new byte[] {value};
        Fill(wfc, pattern);
    }

    private void Fill(BaseWFC wfc, byte[] pattern)
    {
        wfc.predetermined = new List<Predetermined>();

        for (int i = 0; i < wfc.width; i++)
            wfc.predetermined.Add(new Predetermined(wfc.width * (wfc.depth - 1) + i, pattern));

        for (int i = 0; i < wfc.depth; i++)
            wfc.predetermined.Add(new Predetermined(i * wfc.width + wfc.width - 1, pattern));
    }
}
