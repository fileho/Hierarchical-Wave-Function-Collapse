using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


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
        DrawButtons(layers.arraySize);
        

        EditorGUILayout.PropertyField(size);

        var seedValue = seed.FindPropertyRelative("seed");

        EditorGUILayout.PropertyField(seedValue, new GUIContent("Seed", "0 is random seed"));


        DrawLayers();

        EditorGUILayout.PropertyField(postprocessing);

       // DrawDefaultInspector();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawLayers()
    {
        if (layers.arraySize == 0)
            layers.InsertArrayElementAtIndex(0);

        GUILayout.Space(10);
        {
            GUILayout.Label("ROOT");
            var l = layers.GetArrayElementAtIndex(0).FindPropertyRelative("layer");
            if (l.arraySize == 0)
                l.InsertArrayElementAtIndex(0);
            l = l.GetArrayElementAtIndex(0).FindPropertyRelative("wfc");
            EditorGUILayout.PropertyField(l);
            GUILayout.Space(10);
        }

        GUILayout.Label("LAYERS");



        for (int i = 1; i < layers.arraySize; i++)
        {
            var l = layers.GetArrayElementAtIndex(i).FindPropertyRelative("layer");
            EditorGUILayout.PropertyField(l, new GUIContent("Layer " + (i + 1)));
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("ADD LAYER")) 
            layers.InsertArrayElementAtIndex(layers.arraySize);
        if (GUILayout.Button("REMOVE LAYER")) 
            layers.DeleteArrayElementAtIndex(layers.arraySize - 1);

        EditorGUILayout.EndHorizontal();
        //   EditorGUILayout.PropertyField(layers);
    }

    private void DrawButtons(int count)
    {
        var generator = target as HierarchicalController;
        if (generator == null) return;
        if (GUILayout.Button("Clear")) 
            generator.Clear();

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
