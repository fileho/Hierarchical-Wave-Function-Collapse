using System.Collections.Generic;
using hwfc;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Stores predetermined tiles that will be given to the wfc for an early collapse. 
/// </summary>
[System.Serializable]
public class Predetermined
{
    public int index;
    [Tooltip("Single tile index for Tiled, list of indexes for overlap")]
    public byte[] tiles;

    public Predetermined(int index, byte[] tiles)
    {
        this.index = index;
        this.tiles = TransformTiles(tiles);
    }


    // Transforms tiles from human readable into the wfc format (swapped rows)
    private byte[] TransformTiles(byte[] t)
    {
        int n = 1;
        while (n * n < t.Length)
            ++n;

        byte[] newTiles = new byte[n * n];

        int row = t.Length / n;

        for (int i = 0; i < t.Length; i++)
        {
            int col = i % n;
            if (col == 0)
                --row;
            newTiles[n * row + col] = t[i];
        }

        return newTiles;
    }
}


public class BaseWFC : MonoBehaviour
{
    public bool debug;

    public Preplacement preplacement;
    public WfcPostprocessing postprocessing;

    public int width = 20;
    public int depth = 20;
    public GameObject[,] rendering;
    [HideInInspector]
    public List<Predetermined> predetermined = new List<Predetermined>();
    [HideInInspector]
    public List<int> removed = new List<int>();

    public bool[,] fill;

    public UnityEvent generationDone = new UnityEvent();


    public virtual void Generate() { }

    public virtual void SetSize(int w, int h, bool[,] fill, int seed = 0) { }
    public void SetSize(int w, int h, int seed = 0) { SetSize(w, h, new bool[0,0], seed); }

    public virtual void Upscale(int scale) {}

    public bool IsRemoved(int index)
    {
        return removed.Contains(index);
        // return predetermined.Any(p => p.index == index && p.removeTile);
    }

    public virtual void AddBlocked(int index){}

    private void OnDestroy()
    {
        generationDone.RemoveAllListeners();
    }
}
