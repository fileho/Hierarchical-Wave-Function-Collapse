using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hwfc
{
/// <summary>
/// Preplace value tile only to corners. Currently this is unused.
/// </summary>

[CreateAssetMenu(menuName = "SO/Preplacements/Corners")]
public class Corners : Preplacement
{
    [SerializeField]
    private byte value;

    public override void Run(OverlapWFC wfc)
    {
        int n = wfc.N;
        byte[] pattern = new byte[n * n];

        for (int i = 0; i < n * n; i++)
            pattern[i] = value;
        Fill(wfc, pattern);
        FillEmpty(wfc);
    }

    private void Fill(OverlapWFC wfc, byte[] pattern)
    {
        wfc.predetermined = new List<Predetermined> { new Predetermined(wfc.width - 1, pattern),
                                                      new Predetermined(wfc.width * (wfc.depth - 1), pattern) };
    }
}
}
