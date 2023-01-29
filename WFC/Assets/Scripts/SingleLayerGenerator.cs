using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class SingleLayerGenerator : MonoBehaviour
{
    private OverlapWFC wfc;

    [SerializeField] private int limit = 100;
    private int generatedCount;
    private float time;
    private float timeSinceFail;
    private int fails;

    private Text text;

    // Start is called before the first frame update
    void Start()
    {
        CultureInfo ci = new CultureInfo("en-US");
        Thread.CurrentThread.CurrentCulture = ci;
        Thread.CurrentThread.CurrentUICulture = ci;
        text = FindObjectOfType<Text>();
        
        wfc = FindObjectOfType<OverlapWFC>();
        wfc.generationDone.AddListener(OnGenerationDone);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            wfc.seed = 0;
            wfc.iterations = 50;
            GenerateNext();
        }
    }

    private void OnGenerationDone()
    {
        float delta = Time.time - time;
        float last = Time.time - timeSinceFail;
        Logger.Write(delta + " " + last + " " + fails + "\n");

        GenerateNext();
    }

    private void GenerateNext()
    {
        if (generatedCount == limit)
            return;
        ++generatedCount;

        text.text = generatedCount + " / " + limit;
        wfc.seed++;
        wfc.Generate();
        time = Time.time;
        timeSinceFail = Time.time;
        fails = 0;
    }

    public void WfcFailure()
    {
        ++fails;
        timeSinceFail = Time.time;
    }
}
