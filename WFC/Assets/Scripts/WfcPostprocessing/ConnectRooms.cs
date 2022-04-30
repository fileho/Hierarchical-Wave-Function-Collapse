using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Postprocessing/ConnectRooms")]
public class ConnectRooms : WfcPostprocessing
{
    private GameObject[,] tiles;
    public GameObject path;
    public GameObject wall;
    public int pathSize = 2;

    private Transform transform;

    public override void Run(OverlapWFC wfc)
    {
        tiles = wfc.rendering;
        transform = wfc.transform.GetChild(0).GetChild(0);

        AddPaths();
    }
    
    public void AddPaths()
    {
        Layer l = new Layer { type = new List<int>() { 2 } };
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

            foreach (var (x, y) in edges)
            {
                if (x == val)
                    if (!visited.Contains(y))
                        queue.Enqueue(y);
                if (y == val)
                    if (!visited.Contains(x))
                        queue.Enqueue(x);
            }
        }

        return false;
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

        var t = tiles[y, x];
        if (t == null)
            return;

        if (!predicate(t.GetComponent<TileType>().type))
            return;

        DestroyObject(tiles[y, x]);
        Vector3 pos = transform.position;
        var o = Instantiate(tile, new Vector3(pos.x + y, pos.y + x, -1), Quaternion.identity);
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
}
