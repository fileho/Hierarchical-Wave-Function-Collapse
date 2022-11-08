using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using hwfc;

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

    // Uses BFS to track a connected area with specified value of TileType.type
    private static Layout TrackPattern(GameObject[,] rendering, int x, int y, Layer layer)
    {
        Vector2Int pos = new Vector2Int(x, y);
        int width = rendering.GetLength(1);
        int height = rendering.GetLength(0);
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Vector2Int min = pos;
        Vector2Int max = pos;
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(pos);
        
        while (queue.Count > 0)
        {
            pos = queue.Dequeue();
            if (visited.Contains(pos))
                continue;
            if (!IsValid(pos.x, width) || !IsValid(pos.y, height))
                continue;
            if (!layer.Contains(rendering[pos.y, pos.x]))
                continue;

            min = Vector2Int.Min(min, pos);
            max = Vector2Int.Max(max, pos);
            visited.Add(pos);
            AddToQueue(queue, visited, pos);
        }

        return new Layout(min, max, visited);
    }

    private static void AddToQueue(Queue<Vector2Int> queue, HashSet<Vector2Int> visited, Vector2Int pos)
    {
        Vector2Int[] dirs = { Vector2Int.right, Vector2Int.down, Vector2Int.up, Vector2Int.left };
        foreach (var dir in dirs)
        {
            var newPos = pos + dir;
            if (!visited.Contains(newPos))
                queue.Enqueue(newPos);
        }
    }


    public static bool IsValid(int value, int max)
    {
        return value >= 0 && value < max;
    }
}
