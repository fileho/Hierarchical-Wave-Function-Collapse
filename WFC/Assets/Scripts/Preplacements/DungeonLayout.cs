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

        // Pick a pseudo-random number according to a seed
        // With a fixed seed, we need to generate the same layout every time
        var r = wfc.seed == 0 ? new System.Random() : new System.Random(wfc.seed);

        int x = r.Next(wfc.width / 8);
        int y = r.Next(wfc.depth / 8);

        var p = BossRoomCorner();
        wfc.predetermined.Add(new Predetermined(x + wfc.width * y, p));
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
