using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu(menuName = "SO/Preplacements/DungeonLayout")]
public class DungeonLayout : Preplacement
{
    // places the BossRoom to the corner of the generated map
    public override void Run(OverlapWFC wfc)
    {
        Assert.AreEqual(wfc.N, 3, "DungeonLayout works only with N = 3");

        wfc.predetermined = new List<Predetermined>(1);

        // pick a pseudo-random number according to a seed
        // with a fixed seed, we need to generate the same layout every time
        var r = wfc.seed == 0 ? new System.Random() : new System.Random(wfc.seed);

        int x = r.Next(wfc.width / 5);
        int y = r.Next(wfc.depth / 5);

        var p = BossRoomCorner();
        wfc.predetermined.Add(new Predetermined(x + wfc.width * y, p));
    }

    private byte[] BossRoomCorner()
    {
        // the corner of the boss room
        // contains tile indices
        var ret = new byte[] 
        { 
            5, 9, 10,
            5, 9, 9,
            5, 5, 5 
        };

        return ret;
    }
}
