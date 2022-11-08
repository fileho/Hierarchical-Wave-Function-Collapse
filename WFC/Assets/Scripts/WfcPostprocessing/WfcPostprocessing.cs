using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WfcPostprocessing : ScriptableObject
{
    public virtual void Run(OverlapWFC wfc) {}

    protected void ReplaceTile(OverlapWFC wfc, int y, int x, GameObject tile)
    {
        DestroyObject(wfc.rendering[y, x]);
        Vector3 pos = wfc.transform.position;
        Transform parent = wfc.transform.GetChild(0).GetChild(0);
        var o = Instantiate(tile, new Vector3(pos.x + y, pos.y + x, 0), Quaternion.identity, parent);
        wfc.rendering[y, x] = o;
    }

    protected static void DestroyObject(GameObject o)
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
