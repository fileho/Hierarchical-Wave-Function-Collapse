using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hwfc
{
[CreateAssetMenu(menuName = "SO/Postprocessing/RemoveSmallRooms")]
public class RemoveSmallRooms : WfcPostprocessing
{
    public GameObject defaultTile;

    public override void Run(OverlapWFC wfc)
    {
        const int minSize = 4;

        Layer l = new Layer { type = new List<int>() { 1, 2 } };
        var layouts = Utilities.FindAllPatterns(wfc.rendering, l);

        foreach (var layout in layouts)
        {
            if (layout.size.x < minSize || layout.size.y < minSize)
                DeleteLayout(wfc, layout);
        }
    }

    private void DeleteLayout(OverlapWFC wfc, Layout layout)
    {
        for (int i = 0; i < layout.size.x; i++)
        {
            for (int j = 0; j < layout.size.y; j++)
            {
                int x = layout.min.x + i;
                int y = layout.min.y + j;
                ReplaceTile(wfc, y, x, defaultTile);
            }
        }
    }
}
}
