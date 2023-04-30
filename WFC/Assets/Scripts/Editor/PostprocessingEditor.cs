using UnityEditor;
using UnityEngine;

namespace hwfc
{
/// <summary>
/// Simple editor that allows us to run the final postprocessing on an exported map
/// </summary>
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
