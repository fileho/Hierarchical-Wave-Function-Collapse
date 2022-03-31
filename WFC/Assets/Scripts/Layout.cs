using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Layout
{
    public Vector2Int min;
    public Vector2Int size;

    public readonly bool[,] fill;

    public bool Contains(int x, int y)
    {
        x -= min.x;
        y -= min.y;

        if (x < 0 || x >= size.x || y < 0 || y >= size.y)
            return false;
        return fill[y, x];
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

    public Vector2Int NearestPoint(Layout other)
    {
        int x = min.x > other.min.x ? min.x + 1 : Math.Min(min.x + size.x - 2, other.min.x + 1);
        int y = min.y > other.min.y ? min.y + 1 : Math.Min(min.y + size.y - 2, other.min.y + 1);
        return new Vector2Int(x, y);
    }
}
