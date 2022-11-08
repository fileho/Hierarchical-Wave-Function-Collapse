using UnityEngine;
using UnityEditor;

namespace hwfc
{
/// <summary>
/// A custom editor to make working with the Hierarchical Controller more pleasant
/// Adds buttons for generating individual layers
/// </summary>
[CustomEditor(typeof(HierarchicalController))]
public class HierarchicalControllerEditor : Editor
{
    private SerializedProperty size;
    private SerializedProperty seed;
    private SerializedProperty layers;
    private SerializedProperty postprocessing;

    private void OnEnable()
    {
        size = serializedObject.FindProperty("size");
        seed = serializedObject.FindProperty("seed");
        layers = serializedObject.FindProperty("layers");
        postprocessing = serializedObject.FindProperty("postprocessing");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        // Draw control buttons
        DrawButtons(layers.arraySize);

        EditorGUILayout.PropertyField(size);

        // Show only the int value for the seed
        // Not the whole Seed class
        var seedValue = seed.FindPropertyRelative("seed");
        EditorGUILayout.PropertyField(seedValue, new GUIContent("Seed", "0 is random seed"));

        DrawLayers();

        EditorGUILayout.PropertyField(postprocessing);

        // DrawDefaultInspector();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawLayers()
    {
        // Ensure that we will always have at least one layer
        if (layers.arraySize == 0)
            layers.InsertArrayElementAtIndex(0);

        GUILayout.Space(10);
        {
            // Draw only the wfc parameter for the first layer
            // There can be only one wfc
            // Its children are all wfcs in the second layer
            GUILayout.Label("ROOT");
            var l = layers.GetArrayElementAtIndex(0).FindPropertyRelative("layer");
            if (l.arraySize == 0)
                l.InsertArrayElementAtIndex(0);
            l = l.GetArrayElementAtIndex(0).FindPropertyRelative("wfc");
            EditorGUILayout.PropertyField(l);
            GUILayout.Space(10);
        }

        GUILayout.Label("LAYERS");

        // Draw all layers
        for (int i = 1; i < layers.arraySize; i++)
        {
            var l = layers.GetArrayElementAtIndex(i).FindPropertyRelative("layer");
            EditorGUILayout.PropertyField(l, new GUIContent("Layer " + (i + 1)));
        }

        // Buttons for add and removing extra layers
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("ADD LAYER"))
            layers.InsertArrayElementAtIndex(layers.arraySize);
        if (GUILayout.Button("REMOVE LAYER"))
            layers.DeleteArrayElementAtIndex(layers.arraySize - 1);

        EditorGUILayout.EndHorizontal();
    }

    private void DrawButtons(int count)
    {
        var generator = target as HierarchicalController;
        if (generator == null)
            return;
        if (GUILayout.Button("Clear"))
            generator.Clear();

        // Show only buttons for specified layers
        for (int i = 0; i < count; i++)
        {
            if (GUILayout.Button("Generate layer " + (i + 1)))
                generator.GenerateLayer(i);
        }

        if (GUILayout.Button("Upscale Map"))
            generator.UpscaleMap();
        if (GUILayout.Button("Export Map"))
            generator.ExportMap();
    }
}
}
