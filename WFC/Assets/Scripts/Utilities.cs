using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Utilities
{
    public static List<Layout> FindAllPatterns(GameObject[,] rendering, Layer layer)
    {
        List<Layout> layouts = new List<Layout>();

        for (int i = 0; i < rendering.GetLength(1); i++)
        {
            for (int j = 0; j < rendering.GetLength(0); j++)
            {
                if (!layer.Contains(rendering[j, i])) continue;

                if (layouts.Any(layout => layout.Contains(i, j)))
                    continue;

                // ADD new layout
                layouts.Add(TrackPattern(rendering, i, j, layer));
            }
        }

        return layouts;
    }

    // uses BFS
    private static Layout TrackPattern(GameObject[,] rendering, int x, int y, Layer layer)
    {
        Vector2Int pos = new Vector2Int(x, y);
        int width = rendering.GetLength(1);
        int height = rendering.GetLength(0);
        HashSet<Vector2Int> visited = new HashSet<Vector2Int> { pos };
        Vector2Int min = pos;
        Vector2Int max = pos;

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(pos);

        Vector2Int[] dirs = { Vector2Int.right, Vector2Int.down, Vector2Int.up, Vector2Int.left };

        while (queue.Count > 0)
        {
            pos = queue.Dequeue();
            foreach (var dir in dirs)
            {
                Vector2Int npos = pos + dir;

                if (visited.Contains(npos))
                    continue;
                if (npos.x < 0 || npos.x >= width || npos.y < 0 || npos.y >= height)
                    continue;
                if (!layer.Contains(rendering[npos.y, npos.x]))
                    continue;

                min = Vector2Int.Min(min, npos);
                max = Vector2Int.Max(max, npos);
                visited.Add(npos);
                queue.Enqueue(npos);
            }
        }

        return new Layout(min, max, visited);
    }
}
