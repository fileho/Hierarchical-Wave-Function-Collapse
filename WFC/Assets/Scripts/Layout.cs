using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace hwfc
{
// Represent an areas on which we can run another wfc
public class Layout
{
    public Vector2Int min;
    public Vector2Int size;

    // Bitmap of the size of the whole areas
    // Important since it allows a pattern of any shape
    // True -> place to fill
    public readonly bool[,] fill;

    public bool Contains(int x, int y)
    {
        x -= min.x;
        y -= min.y;

        return IsInside(x, y);
    }

    public bool Contains(Vector2Int pos)
    {
        return Contains(pos.x, pos.y);
    }

    public Layout(Vector2Int min, Vector2Int max, HashSet<Vector2Int> tracked)
    {
        this.min = min;
        size = max - min + Vector2Int.one;
        fill = new bool[size.y, size.x];

        Fill(tracked);
    }

    private void Fill(HashSet<Vector2Int> tracked)
    {
        foreach (var p in tracked.Select(pos => pos - min))
            fill[p.y, p.x] = true;
    }

    // Adds empty block to stop generation there
    public void FillWfc(BaseWFC wfc)
    {
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                if (!fill[j, i])
                    wfc.AddBlocked(i * wfc.width + j);
            }
        }
    }

    public Vector2Int Middle()
    {
        return min + size / 2;
    }

    // Heuristic used for drill paths between two room
    // It returns some close point, not necessary the closest - it would make the tunnel look artificial
    public Vector2Int NearestPoint(Layout other)
    {
        int x = min.x > other.min.x ? min.x + 3 : Math.Min(min.x + size.x - 3, other.min.x + 3);
        int y = min.y > other.min.y ? min.y + 3 : Math.Min(min.y + size.y - 3, other.min.y + 3);

        var pos = new Vector2Int(x, y);
        if (Contains(pos))
            return pos;

        // Look into all directions until an inside point is not found
        Vector2Int[] offsets = { new Vector2Int(1, 1),   new Vector2Int(1, -1), new Vector2Int(-1, 1),
                                 new Vector2Int(-1, -1), Vector2Int.right,      Vector2Int.down,
                                 Vector2Int.up,          Vector2Int.left };
        int distance = 1;
        while (true)
        {
            foreach (var o in offsets)
            {
                var p = pos + o * distance;
                if (Contains(p))
                    return p;
            }

            ++distance;
        }
    }

    private bool IsInside(int x, int y)
    {
        if (x < 0 || x >= size.x || y < 0 || y >= size.y)
            return false;
        return fill[y, x];
    }
}
}
