using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace hwfc
{
/// <summary>
/// Used for the abstract layer in the dungeon. Places a boss room close to the edge. It forces the dungeon to always
/// have a boss room.
/// </summary>

[CreateAssetMenu(menuName = "SO/Preplacements/DungeonLayout")]
public class DungeonLayout : Preplacement
{
    // places the BossRoom to the corner of the generated map
    public override void Run(OverlapWFC wfc)
    {
        Assert.AreEqual(wfc.N, 3, "DungeonLayout works only with N = 3");

        wfc.predetermined = new List<Predetermined>(1);

        // Input is too small
        if (wfc.width < 10 || wfc.depth < 10)
            return;

        // Pick a pseudo-random number according to a seed
        // With a fixed seed, we need to generate the same layout every time
        var r = wfc.seed == 0 ? new System.Random() : new System.Random(wfc.seed);

        // How close it should be towards a corner
        const int cornerFactor = 8;
        int x = r.Next(wfc.width / cornerFactor) + 1;
        int y = r.Next(wfc.depth / cornerFactor) + 1;

        var p = BossRoomCorner();

        // The boss room can be is each corner, 4 values for swaps, the max value is exclusive
        // Need to swap the pattern too since it is not symmetrical and start from a corner
        int swaps = r.Next(4);
        if (swaps % 2 == 1)
            FlipX(wfc, ref x, p);
        if (swaps >= 2)
            FlipY(wfc, ref y, p);

        wfc.predetermined.Add(new Predetermined(x + wfc.width * y, p));
    }

    private void FlipX(OverlapWFC wfc, ref int x, byte[] pattern)
    {
        // Offset since the pattern for overlap WFC is not central
        x = wfc.width - x - 2;

        for (int i = 0; i < 3; i++)
        {
            Swap(pattern, 3 * i, 3 * i + 2);
        }
    }

    private void FlipY(OverlapWFC wfc, ref int y, byte[] pattern)
    {
        // Offset since the pattern for overlap WFC is not central
        y = wfc.depth - y - 2;

        for (int i = 0; i < 3; i++)
        {
            Swap(pattern, i, 6 + i);
        }
    }

    private void Swap(byte[] pattern, int p1, int p2)
    {
        var tmp = pattern[p1];
        pattern[p1] = pattern[p2];
        pattern[p2] = tmp;
    }

    private byte[] BossRoomCorner()
    {
        // The corner of the boss room
        // Contains tile indices
        var ret = new byte[] { 5, 9, 10, 5, 9, 9, 5, 5, 5 };

        return ret;
    }
}
}
