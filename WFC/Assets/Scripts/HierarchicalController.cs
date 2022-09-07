using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


[System.Serializable]
public class Layer
{
    public List<int> type;
    public BaseWFC wfc;
    public List<int> children;
    [HideInInspector]
    public List<BaseWFC> instances;

    public bool Contains(GameObject go)
    {
        return go && type.Contains(go.GetComponent<TileType>().type);
    }

    public void Reset()
    {
        instances = new List<BaseWFC>();
    }
}





[ExecuteInEditMode]
public class HierarchicalController : MonoBehaviour
{
    public Vector2Int size = new Vector2Int(40, 30);
    public List<Layer> root;

    [SerializeField] private Postprocessing postprocessing;

    public static HierarchicalController instance;

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
            wfc.Reset();

        Clear();

        works++;
        var top = Instantiate(root[0].wfc);
        top.SetSize(size.x,size.y);
        top.Generate();
        top.transform.position = new Vector3(0, 0, 0);
        top.transform.parent = transform.GetChild(0);

        root[0].instances.Add(top);
    }

    public void UpscaleMap(int scale = 2)
    {
        root[0].instances[0].Upscale(scale);
    }

    public void Clear()
    {
        if (CreateHoldingObject()) return;

        Transform parent = transform.GetChild(0);

        while (parent.childCount > 0) DestroyObject(parent.GetChild(0).gameObject);
    }

    private bool CreateHoldingObject()
    {
        if (transform.childCount != 0) 
            return false;

        GameObject go = new GameObject("WFCs");
        go.transform.SetParent(transform);
        return true;
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

                foreach (var l in Utilities.FindAllPatterns(instance.rendering, secondLayer))
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

    public void ExportMap()
    {
        DestroyObject( GameObject.Find("Map"));

        Postprocessing map = Instantiate(postprocessing, Vector3.zero, Quaternion.identity);
        map.gameObject.name = "Map";
        var r = root[0].instances[0];
        tiles = new GameObject[r.rendering.GetLength(0), r.rendering.GetLength(1)];
        map.tiles = new GameObject[tiles.GetLength(0), tiles.GetLength(1)];

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

    private static void ExportLayer(Postprocessing map, GameObject[,] tiles, BaseWFC wfc)
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
                tile.transform.SetParent(map.transform);
                tiles[y, x] = tile;
                map.tiles[y, x] = tile;
            }
        }
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

        DrawDefaultInspector();
    }
}
#endif
