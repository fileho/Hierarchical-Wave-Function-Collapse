using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Networking.PlayerConnection;
using UnityEditor.UIElements;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


[System.Serializable]
public class Layer
{
    public int type;
    public BaseWFC wfc;
    public List<int> children;
    [HideInInspector]
    public List<BaseWFC> instances;

    public bool Contains(GameObject go)
    {
        if (!go)
            return false;
        return type == go.GetComponent<TileType>().type;
    }

    public void Reset()
    {
        instances = new List<BaseWFC>();
    }
}





[ExecuteInEditMode]
public class HierarchicalController : MonoBehaviour
{
 //   [SerializeField] private Layer root;
    public List<Layer> root;

    public GameObject path;
    public GameObject wall;

    public static HierarchicalController instance;

    private Transform map;

    private int works = 0;
    private int generatedLayers = 0;

    private GameObject[,] tiles;

    private void Awake()
    {
        instance = this;
    }


    private void Start()
    {
        if (!EditorApplication.isPlaying)
            return;

        GenerateTopLayer();
    }

    // wfc done callback
    public void GenerationDone()
    {
        works--;

        if (works == 0)
        {
            UpdateLogic();
        }
    }

    // each wfc in one layer is done
    private void UpdateLogic()
    {
        if (!EditorApplication.isPlaying)
            return;

        if (generatedLayers == 0)
        {
            UpscaleMap();
            UpscaleMap();
            GenerateSecondLayer();
            ++generatedLayers;
        }
    }


    public void GenerateTopLayer()
    {
        generatedLayers = 0;

        foreach (var wfc in root)
        {
            wfc.Reset();
        }

        Clear();

        works++;
        var top = Instantiate(root[0].wfc);
        top.SetSize(40,25);
        top.Generate();
        top.transform.position = new Vector3(0, 0, 0);
        top.transform.parent = transform.GetChild(0);

        root[0].instances.Add(top);
    }

    public void UpscaleMap()
    {
        root[0].instances[0].Upscale(2);
    }

    public void Clear()
    {
        // create object to hold wfc instances
        if (transform.childCount == 0)
        {
            GameObject go = new GameObject("WFCs");
            go.transform.SetParent(transform);
            return;
        }

        Transform parent = transform.GetChild(0);

        while (parent.childCount > 0)
        {
        #if UNITY_EDITOR
            DestroyImmediate(parent.GetChild(0).gameObject);
        #else
            Destroy(parent.GetChild(0).gameObject);
        #endif
        }
    }

    public void GenerateSecondLayer()
    {
        GenerateLayer(root[0]);
    }


    private void GenerateLayer(Layer layer)
    {
        foreach (var instance in layer.instances)
        {
            foreach (var val in layer.children)
            {
                var secondLayer = root[val];

                foreach (var l in FindAllPatterns(instance.rendering, secondLayer))
                {
                    var wfc = Instantiate(secondLayer.wfc);
                    wfc.SetSize(l.size.y, l.size.x, l.fill);
                    l.FillWfc(wfc);
                    wfc.Generate();
                    wfc.transform.position = new Vector3(l.min.y, l.min.x, -1);
                    wfc.transform.parent = transform.GetChild(0);
                    secondLayer.instances.Add(wfc);
                }
            }
        }
    }

  

    private class Layout
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

        // Adds empty o to stop generation there
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


    private List<Layout> FindAllPatterns(GameObject[,] rendering, Layer layer)
    {
        List<Layout> layouts = new List<Layout>();

        for (int i = 0; i < rendering.GetLength(1); i++)
        {
            for (int j = 0; j < rendering.GetLength(0); j++)
            {
                if (!layer.Contains(rendering[j, i])) continue;
                
                if(layouts.Any(layout => layout.Contains(i, j)))
                    continue;

                // ADD new layout
                layouts.Add(TrackPattern(rendering, i, j, layer));
            }
        }

        return layouts;
    }

    // uses BFS
    private Layout TrackPattern(GameObject[,] rendering, int x, int y, Layer layer)
    {
        Vector2Int pos = new Vector2Int(x, y);
        int width = rendering.GetLength(1);
        int height = rendering.GetLength(0);
        HashSet<Vector2Int> visited = new HashSet<Vector2Int> {pos};
        Vector2Int min = pos;
        Vector2Int max = pos;

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(pos);

        Vector2Int[] dirs = {Vector2Int.right, Vector2Int.down, Vector2Int.up, Vector2Int.left};

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

    public void ExportMap()
    {
        DestroyObject( GameObject.Find("Map"));

        map = new GameObject("Map").transform;
        var r = root[0].instances[0];
        tiles = new GameObject[r.rendering.GetLength(0), r.rendering.GetLength(1)];

        Queue<Layer> queue = new Queue<Layer>();
        queue.Enqueue(root[0]);

        while (queue.Count > 0)
        {
            Layer l = queue.Dequeue();
            foreach (var inst in l.instances)
            {
                ExportLayer(map, tiles, inst);
            }

            foreach (var c in l.children)
                queue.Enqueue(root[c]);
        }

    }

    private static void ExportLayer(Transform map, GameObject[,] tiles, BaseWFC wfc)
    {
        for (int i = 0; i < wfc.rendering.GetLength(1); i++)
        {
            for (int j = 0; j < wfc.rendering.GetLength(0); j++)
            {
                var t = wfc.rendering[j, i];
                if (t == null) continue;

                int x = i + (int)wfc.transform.position.y;
                int y = j + (int)wfc.transform.position.x;
                DestroyObject(tiles[y,x]);
                var tile = Instantiate(t, t.transform.position, Quaternion.identity);
                tile.transform.SetParent(map);
                tiles[y, x] = tile;
            }
        }
    }

    public void AddPaths()
    {
        if (tiles == null)
            ExportMap();

        Layer l = new Layer {type = 1};
        var layouts = FindAllPatterns(tiles, l);

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

            foreach (var (x,y) in edges)
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
        Vector2Int pos = l1.NearestPoint(l2);


        // while (pos.x <= l2.min.x)
        // {
        //     PlacePathTile(pos.x, pos.y);
        //     ++pos.x;
        // }
        // 
        // while (pos.x >= l2.min.x + l2.size.x - 1)
        // {
        //     PlacePathTile(pos.x, pos.y);
        //     --pos.x;
        // }
        // 
        // while (pos.y >= l2.min.y + l2.size.y - 1)
        // {
        //     PlacePathTile(pos.x, pos.y);
        //     --pos.y;
        // }
        // 
        // while (pos.y <= l2.min.y)
        // {
        //     PlacePathTile(pos.x, pos.y);
        //     ++pos.y;
        // }

        pos = l1.Middle();
        var goal = l2.Middle();

        // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
        while (pos != goal)
        {
            PlacePathTile(pos.x, pos.y);

            var dir = goal - pos;

            if (dir.x != 0)
            {
                pos.x += dir.x / Math.Abs(dir.x);
                continue;
            }

            pos.y += dir.y / Math.Abs(dir.y);
        }
    }    

    private void PlacePathTile(int x, int y)
    {
        DestroyObject(tiles[y, x]);
        var o = Instantiate(path, new Vector3(y ,x, 0), Quaternion.identity);
        o.transform.SetParent(map);
        tiles[y, x] = o;
        for (int i = -1; i <= 1; i++)
            for (int j = -1; j <= 1; j++)
                PlaceWalls(x + i, y + j);
    }

    private void PlaceWalls(int x, int y)
    {
        if (x < 0 || y < 0 || x >= tiles.GetLength(1) || y >= tiles.GetLength(0))
            return;

        if (tiles[y, x].GetComponent<TileType>().type != 0)
            return;

        DestroyObject(tiles[y, x]);
        var o = Instantiate(wall, new Vector3(y, x, 0), Quaternion.identity);
        o.transform.SetParent(map);
        tiles[y, x] = o;
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
[CustomEditor(typeof(HierarchicalController))]
public class HierarchicalControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        HierarchicalController generator = (HierarchicalController)target;
        if (GUILayout.Button("Clear"))
        {
            generator.Clear();
        }
        if (GUILayout.Button("Generate top layer"))
        {
            generator.GenerateTopLayer();
        }

        if (GUILayout.Button("Upscale Map"))
        {
            generator.UpscaleMap();
        }

        if (GUILayout.Button("Generate second layer"))
        {
            generator.GenerateSecondLayer();
        }

        if (GUILayout.Button("Export Map"))
        {
            generator.ExportMap();
        }

        if (GUILayout.Button("Add Paths"))
        {
            generator.AddPaths();
        }

        DrawDefaultInspector();
    }
}
#endif
