using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameGanerator : MonoBehaviour
{
    private HierarchicalController hierarchicalController;
    // Start is called before the first frame update
    void Start()
    {
        hierarchicalController = FindObjectOfType<HierarchicalController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
            hierarchicalController.StartGenerating();
    }
}
