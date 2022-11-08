using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace hwfc
{
// A postprocessing that will add paths at the end
// Used for the dungeon
public class AddPathsPostprocessing : Postprocessing
{
    public GameObject path;
    public GameObject wall;
    public int pathSize = 3;

    public override void Run()
    {
        if (tiles == null)
            return;
        AddPaths();
    }

    public void AddPaths()
    {
        var l = new Layer { type = new List<int> { 2 } };
        var layouts = Utilities.FindAllPatterns(tiles, l);
        var edges = new List<(int, int)>();

        for (var i = 0; i < layouts.Count; i++)
        {
            var order = FindNearestLayout(layouts, i);
            int j = order.Find(j => !Connected(i, j, edges));
            CreatePath(layouts[i], layouts[j]);
            edges.Add((i, j));
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
            if (x == edgeValue && !visited.Contains(y))
                queue.Enqueue(y);
            if (y == edgeValue && !visited.Contains(x))
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

        indices.Sort(((i, j) => (src.Middle() - layouts[i].Middle())
                                    .magnitude.CompareTo((src.Middle() - layouts[j].Middle()).magnitude)));

        return indices;
    }

    private void CreatePath(Layout l1, Layout l2)
    {
        var pos = l1.NearestPoint(l2);
        var goal = l2.NearestPoint(l2);

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
        int min = -pathSize / 2;
        int max = (pathSize + 1) / 2;

        for (int i = min; i < max; i++)
            for (int j = min; j < max; j++)
                PlacePath(x - i, y - j);

        for (int i = min - 1; i < max + 1; i++)
            for (int j = min - 1; j < max + 1; j++)
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
        PlaceTile(x, y, wall, v => v != 2);
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

#if UNITY_EDITOR
[CustomEditor(typeof(AddPathsPostprocessing))]
public class AddPathsPostprocessingEditor : Editor
{
    private AddPathsPostprocessing p;

    private void OnEnable()
    {
        p = (AddPathsPostprocessing)target;
    }

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Run postprocessing"))
        {
            p.Run();
        }

        base.OnInspectorGUI();
    }
}
#endif
}
