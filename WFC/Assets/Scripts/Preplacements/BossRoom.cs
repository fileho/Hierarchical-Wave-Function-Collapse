using System.Collections.Generic;
using UnityEngine;

namespace hwfc
{
/// <summary>
/// Used a the boss room generation in the dungeon.
/// It Places wall around central parts of edges.
/// This forces to generate a connected room over most of the area.
/// </summary>
[CreateAssetMenu(menuName = "SO/Preplacements/BossRoom")]
public class BossRoom : Preplacement
{
    public override void Run(OverlapWFC wfc)
    {
        wfc.predetermined = new List<Predetermined>(2);

        // Wall tiles outside, ground inside
        byte[] bottom = { 3, 3, 3, 2, 2, 2, 1, 1, 1 };
        byte[] left = { 1, 2, 3, 1, 2, 3, 1, 2, 3 };

        for (int i = wfc.width / 3; i < wfc.width * 2 / 3; i++)
        {
            wfc.predetermined.Add(new Predetermined(i, bottom));
        }

        for (int i = wfc.depth / 3; i < wfc.depth * 2 / 3; i++)
        {
            wfc.predetermined.Add(new Predetermined(wfc.width * i, left));
        }
    }
}
}
