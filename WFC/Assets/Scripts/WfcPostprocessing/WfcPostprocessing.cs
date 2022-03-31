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

        var o = Instantiate(tile, new Vector3(pos.x + y, pos.y + x, 0), Quaternion.identity);
        wfc.rendering[y, x] = o;
        o.transform.SetParent(wfc.transform);
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
