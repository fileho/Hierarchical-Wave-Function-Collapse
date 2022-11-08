using UnityEditor;
using UnityEngine;

namespace hwfc
{

[CustomEditor(typeof(Postprocessing))]
public class PostprocessingEditor : Editor
{
    private Postprocessing p;

    private void OnEnable()
    {
        p = (Postprocessing)target;
    }

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Run postprocessing"))
        {
            p.Run();
        }

        base.OnInspectorGUI();
    }
}
}
