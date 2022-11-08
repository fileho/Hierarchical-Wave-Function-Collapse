using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hwfc
{
// The most important preplacement
// It placed tiles around the edges

[CreateAssetMenu(menuName = "SO/Preplacements/Borders")]
public class Borders : Preplacement
{
    // TopDown - place those tiles around only two edges and use the periodical output property
    // It will make borders smaller
    // All - place tiles around all 4 sides
    public enum FillArea
    {
        All,
        TopDown
    }

    [Tooltip("A value to fill the map (index of the tile in the TilePainter, starts at 1 as 0 is a null value)")]
    public byte value;

    [Tooltip("TopDown - place those tiles around only two edges and use the periodical output property\nAll - place tiles around all 4 sides")]
    public FillArea fillArea = FillArea.All;

    public override void Run(OverlapWFC wfc)
    {
        int n = wfc.N;
        byte[] pattern = new byte[n * n];

        for (int i = 0; i < n * n; i++)
            pattern[i] = value;
        Fill(wfc, pattern);
        FillEmpty(wfc);
    }

    public override void Run(SimpleTiledWFC wfc)
    {
        byte[] pattern = new byte[] { value };
        Fill(wfc, pattern);
    }

    private void Fill(BaseWFC wfc, byte[] pattern)
    {
        wfc.predetermined = new List<Predetermined>();

        for (int i = 0; i < wfc.width; i++)
            wfc.predetermined.Add(new Predetermined(wfc.width * (wfc.depth - 1) + i, pattern));

        // Add the remaining two edges
        if (fillArea == FillArea.All)
        {
            for (int i = 0; i < wfc.depth; i++)
                wfc.predetermined.Add(new Predetermined(i * wfc.width + wfc.width - 1, pattern));
        }
    }
}
}
