using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using hwfc;

/// <summary>
/// Used for benchmarking and generating many outputs at once
/// </summary>
public class IngameGenerator : MonoBehaviour
{
    private HierarchicalController hierarchicalController;

    private const int limit = 100;
    private int generatedCount;
    private float time;
    private int fails;

    // Status of the generation
    [SerializeField]
    private Text text;

    void Start()
    {
        CultureInfo ci = new CultureInfo("en-US");
        Thread.CurrentThread.CurrentCulture = ci;
        Thread.CurrentThread.CurrentUICulture = ci;

        hierarchicalController = FindObjectOfType<HierarchicalController>();
        hierarchicalController.generationDone.AddListener(OnGenerationDone);
    }

    // Update is called once per frame
    void Update()
    {
        // Start the generation
        if (Input.GetKeyDown(KeyCode.G))
        {
            GenerateNext();
        }
    }

    private void OnGenerationDone()
    {
        float delta = Time.time - time;
        Logger.Write(delta + " " + fails + "\n");

        GenerateNext();
    }

    private void GenerateNext()
    {
        if (generatedCount == limit)
            return;
        ++generatedCount;

        text.text = generatedCount + " / " + limit;
        hierarchicalController.StartGenerating(true);
        time = Time.time;
        fails = 0;
    }

    /// <summary>
    /// Need to set this callback in the OverlapWFC
    /// </summary>
    public void WfcFailure()
    {
        ++fails;
    }
}
