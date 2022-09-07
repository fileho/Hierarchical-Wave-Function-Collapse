using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Postprocessing : MonoBehaviour
{
    public GameObject[,] tiles;
    public GameObject path;
    public GameObject wall;
    public int pathSize = 3;

    public void AddPaths()
    {
        Layer l = new Layer { type = new List<int>(){2} };
        var layouts = Utilities.FindAllPatterns(tiles, l);

        List<(int, int)> edges = new List<(int, int)>();


        for (var i = 0; i < layouts.Count; i++)
        {
            var order = FindNearestLayout(layouts, i);
            foreach (var j in order)
            {
                if (!Connected(i, j, edges))
                {
                    CreatePath(layouts[i], layouts[j]);
                    edges.Add((i, j));
                    break;
                }
            }
        }
    }


    // Checks if those components are connected using bfs
    private static bool Connected(int i, int j, List<(int, int)> edges)
    {
        HashSet<int> visited = new HashSet<int>();
        Queue<int> queue = new Queue<int>();
        queue.Enqueue(i);

        while (queue.Count > 0)
        {
            int val = queue.Dequeue();
            if (val == j)
                return true;
            visited.Add(val);
            AddEdgesToQueue(queue, visited, edges, val);
        }

        return false;
    }

    private static void AddEdgesToQueue(Queue<int> queue, HashSet<int> visited, List<(int, int)> edges, int edgeValue)
    {
        foreach (var (x, y) in edges)
        {
            if (x == edgeValue)
                if (!visited.Contains(y))
                    queue.Enqueue(y);
            if (y == edgeValue)
                if (!visited.Contains(x))
                    queue.Enqueue(x);
        }
    }

    private List<int> FindNearestLayout(List<Layout> layouts, int index)
    {
        Layout src = layouts[index];

        List<int> indices = new List<int>();
        for (int i = 0; i < layouts.Count; i++)
            if (i != index)
                indices.Add(i);

        indices.Sort(((i, j) =>
            (src.Middle() - layouts[i].Middle()).magnitude.CompareTo((src.Middle() - layouts[j].Middle()).magnitude)));

        return indices;
    }

    private void CreatePath(Layout l1, Layout l2)
    {
        Vector2Int pos = l1.Middle();
        var goal = l2.Middle();

        // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
        while (pos != goal)
        {
            CreatePath(pos.x, pos.y);
            var dir = goal - pos;
            if (dir.x != 0)
            {
                pos.x += dir.x / Math.Abs(dir.x);
                continue;
            }
            pos.y += dir.y / Math.Abs(dir.y);
        }
    }

    private void CreatePath(int x, int y)
    {
        for (int i = 0; i < pathSize; i++)
            for (int j = 0; j < pathSize; j++)
                PlacePath(x - i, y - j);

        PlacePath(x - 1, y);
        PlacePath(x - 1, y - 1);
        PlacePath(x, y - 1);
       
        for (int i = -1; i <= pathSize; i++)
            for (int j = -1; j <= pathSize; j++)
                PlaceWalls(x - i, y - j);
    }

    private void PlaceTile(int x, int y, GameObject tile, Func<int, bool> predicate)
    {
        if (x < 0 || y < 0 || x >= tiles.GetLength(1) || y >= tiles.GetLength(0))
            return;

        if (!predicate(tiles[y, x].GetComponent<TileType>().type))
            return;

        DestroyObject(tiles[y, x]);
        Vector3 pos = transform.position;
        var o = Instantiate(tile, new Vector3(pos.x + y, pos.y + x, 0), Quaternion.identity);
        o.transform.SetParent(transform);
        tiles[y, x] = o;
    }


    private void PlacePath(int x, int y)
    {
        PlaceTile(x, y, path, v => v != 2);
    }

    private void PlaceWalls(int x, int y)
    {
        PlaceTile(x, y, wall, v => v == 0);
    }


    private static void DestroyObject(GameObject o)
    {
        if (o == null)
            return;
#if UNITY_EDITOR
        DestroyImmediate(o);
#else
        Destroy(o);
#endif
    }
}
