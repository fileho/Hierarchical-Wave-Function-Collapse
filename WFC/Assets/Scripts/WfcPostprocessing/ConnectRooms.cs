using System;
using System.Collections.Generic;
using UnityEngine;

namespace hwfc
{
// A postprocesing that will connect rooms
// Room is defined as a continues area will TileType.type = 1
// Used for connecting rooms fo Cave areas in dungeon
// It avoids areas that will be removed
// Some room might not be connectable
[CreateAssetMenu(menuName = "SO/Postprocessing/ConnectRooms")]
public class ConnectRooms : WfcPostprocessing
{
    private GameObject[,] tiles;
    public GameObject path;
    public GameObject wall;
    public int pathSize = 2;

    private Transform transform;
    private OverlapWFC wfc;

    public override void Run(OverlapWFC wfc)
    {
        tiles = wfc.rendering;
        transform = wfc.transform.GetChild(0).GetChild(0);
        this.wfc = wfc;

        AddPaths();
    }

    public void AddPaths()
    {
        var l = new Layer { type = new List<int> { 2 } };
        var layouts = Utilities.FindAllPatterns(tiles, l);
        var edges = new List<(int, int)>();

        // For each room, find a closest room that is not already connected
        // Drills a path between then if possible
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
            foreach (var (x, y) in edges)
            {
                if (x == val && !visited.Contains(y))
                    queue.Enqueue(y);
                if (y == val && !visited.Contains(x))
                    queue.Enqueue(x);
            }
        }

        return false;
    }

    // Finds all different rooms and sorts then by theirs distance

    private List<int> FindNearestLayout(List<Layout> layouts, int index)
    {
        Layout src = layouts[index];

        List<int> indices = new List<int>();
        for (int i = 0; i < layouts.Count; i++)
            if (i != index)
                indices.Add(i);

        // Determined by the distance from middle of rooms
        indices.Sort(((i, j) => (src.Middle() - layouts[i].Middle())
                                    .magnitude.CompareTo((src.Middle() - layouts[j].Middle()).magnitude)));

        return indices;
    }

    private int PositionIndex(Vector2Int pos)
    {
        return pos.y + pos.x * wfc.width;
    }

    private void CreatePath(Layout l1, Layout l2)
    {
        var pos = l1.NearestPoint(l2);
        var goal = l2.NearestPoint(l1);

        List<Vector2Int> drillArea = new List<Vector2Int>();

        // Avoid areas that will be later removed

        // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
        while (pos != goal)
        {
            drillArea.Add(pos);
            var dir = new Vector2Int(Math.Sign(goal.x - pos.x), Math.Sign(goal.y - pos.y));
            if (dir.x != 0 && !wfc.IsRemoved(PositionIndex(pos + new Vector2Int(dir.x, -1) * pathSize)) &&
                !wfc.IsRemoved(PositionIndex(pos + new Vector2Int(dir.x, 1) * pathSize)))
            {
                pos.x += dir.x;
                continue;
            }

            if (Math.Abs(dir.y) == 0)
                return;
            if (wfc.IsRemoved(PositionIndex(pos + new Vector2Int(1, dir.y) * pathSize)) ||
                wfc.IsRemoved(PositionIndex(pos + new Vector2Int(-1, dir.y) * pathSize)))
                return;
            pos.y += dir.y;
        }

        // Drill the tunnel
        foreach (var drill in drillArea)
        {
            CreatePath(drill.x, drill.y);
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
}
