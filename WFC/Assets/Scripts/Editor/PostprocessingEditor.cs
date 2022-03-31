using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Postprocessing))]
public class PostprocessingEditor : Editor
{
    private Postprocessing p;
    private bool show;

    private void OnEnable()
    {
        p = (Postprocessing) target;
    }

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Add paths"))
        {
            p.AddPaths();
        }
        
        base.OnInspectorGUI();
    }
}
