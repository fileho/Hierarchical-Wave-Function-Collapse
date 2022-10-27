using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleLayerGenerator : MonoBehaviour
{
    [SerializeField]
    private OverlapWFC wfc;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            wfc.Generate();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            wfc.seed++;
            wfc.Generate();
        }
    }
}
