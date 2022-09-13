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
    public BaseWFC wfc;
    public List<int> type;
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

[System.Serializable]
public class LayerList
{
    public List<Layer> layer;

    public void ResetLayer()
    {
        foreach (var wfc in layer) 
            wfc.Reset();
    }
}


[ExecuteInEditMode]
public class HierarchicalController : MonoBehaviour
{
    public Vector2Int size = new Vector2Int(40, 30);

    [SerializeField] private Postprocessing postprocessing;

    private int works = 0;
    private int generatedLayers = 0;

    private GameObject[,] tiles;

    public List<LayerList> layers;
    private bool enableCallbacks = false;

    private bool upscaled = false;

    public void StartGenerating()
    {
        Debug.Log("GENERATING STARTED");
        enableCallbacks = true;
        GenerateLayer(0);
    }

    // wfc done callback
    private void OnGenerationDone()
    {
        works--;
        if (works == 0) 
            UpdateLogic();
    }

    // each wfc in one layer is done
    private void UpdateLogic()
    {
        if (!EditorApplication.isPlaying)
            return;

        if (!enableCallbacks)
            return;

        ++generatedLayers;

        if (generatedLayers == 1 && !upscaled)
        {
            ++works;
            --generatedLayers;
            upscaled = true;
            UpscaleMap();
            return;
        }


        if (generatedLayers < layers.Count) 
            GenerateLayer(generatedLayers);
        else
            Debug.Log("GENERATING DONE");
    }


    private void GenerateTopLayer()
    {
        generatedLayers = 0;
        works = 0;
        upscaled = false;

        foreach (var layer in layers) 
            layer.ResetLayer();

        Clear();

        var root = layers[0].layer[0];
        SetRootChildren(root);

        var top = Instantiate(root.wfc);
        top.SetSize(size.x,size.y);
        top.transform.position = new Vector3(0, 0, 0) + transform.position;
        WfcGeneration(top, root);
    }

    private void SetRootChildren(Layer root)
    {
        if (layers.Count < 2) return;

        if (root.children.Count == layers[1].layer.Count) return;
        
        root.children.Clear();
        for (int i = 0; i < layers[1].layer.Count; i++)
            root.children.Add(i);
    }

    public void UpscaleMap(int scale = 2)
    {
        // incorrect
        layers[0].layer[0].instances[0].Upscale(scale);
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

    public void GenerateLayer(int layerIndex)
    {
        if (layerIndex == 0)
        {
            GenerateTopLayer();
            return;
        }

        foreach (var prevLayer in layers[layerIndex - 1].layer) 
            GenerateLayer(prevLayer, layers[layerIndex]);
    }
    

    private void GenerateLayer(Layer layer, LayerList nextLayer)
    {
        foreach (var wfcInstance in layer.instances)
        {
            foreach (var val in layer.children)
            {
                var secondLayer = nextLayer.layer[val];
                foreach (var l in Utilities.FindAllPatterns(wfcInstance.rendering, secondLayer))
                {
                    var wfc = Instantiate(secondLayer.wfc);
                    wfc.SetSize(l.size.y, l.size.x, l.fill);
                    l.FillWfc(wfc);
                    wfc.transform.position = new Vector3(l.min.y, l.min.x, -1) + wfcInstance.transform.position;
                    WfcGeneration(wfc, secondLayer);
                }
            }
        }
    }

    private void WfcGeneration(BaseWFC wfc, Layer layer)
    {
        ++works;
        wfc.generationDone.AddListener(OnGenerationDone);
        wfc.Generate();
        wfc.transform.parent = transform.GetChild(0);
        layer.instances.Add(wfc);
    }

    public void ExportMap()
    {
        DestroyObject( GameObject.Find("Map"));

        Postprocessing map = Instantiate(postprocessing, Vector3.zero, Quaternion.identity);
        map.gameObject.name = "Map";
        var r = layers[0].layer[0].instances[0];
        tiles = new GameObject[r.rendering.GetLength(0), r.rendering.GetLength(1)];
        map.tiles = new GameObject[tiles.GetLength(0), tiles.GetLength(1)];

        // Queue<Layer> queue = new Queue<Layer>();
        // queue.Enqueue(root[0]);
        // 
        // while (queue.Count > 0)
        // {
        //     Layer l = queue.Dequeue();
        //     foreach (var inst in l.instances)
        //     {
        //         ExportLayer(map, tiles, inst);
        //     }
        // 
        //     foreach (var c in l.children)
        //         queue.Enqueue(root[c]);
        // }

        foreach (var layer in layers)
            foreach (var l in layer.layer)
                foreach (var wfcInstance in l.instances)
                    ExportLayer(map, tiles, wfcInstance);

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
