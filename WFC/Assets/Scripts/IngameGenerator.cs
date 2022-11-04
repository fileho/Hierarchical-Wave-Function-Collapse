using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class IngameGenerator : MonoBehaviour
{
    private HierarchicalController hierarchicalController;

    private const int limit = 100;
    private int generatedCount;
    private float time;
    private int fails;

    [SerializeField]
    private Text text;

    // Start is called before the first frame update
    void Start()
    {
        // CultureInfo ci = new CultureInfo("en-US");
        // Thread.CurrentThread.CurrentCulture = ci;
        // Thread.CurrentThread.CurrentUICulture = ci;
        // 
        hierarchicalController = FindObjectOfType<HierarchicalController>();
        // hierarchicalController.generationDone.AddListener(OnGenerationDone);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            hierarchicalController.StartGenerating();
            // GenerateNext();
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

    public void WfcFailure()
    {
        ++fails;
    }
}
