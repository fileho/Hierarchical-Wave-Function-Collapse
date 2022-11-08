using UnityEditor;

namespace hwfc
{
/// <summary>
/// Shows the auto-tilling tile based on the option for auto-tilling type.
/// </summary>
[CustomEditor(typeof(Autotilling))]
public class AutotillingEditor : Editor
{
    private SerializedProperty tillingType;

    // Tiles 4-way
    private SerializedProperty northWest;
    private SerializedProperty northEast;
    private SerializedProperty southWest;

    private SerializedProperty southEast;

    // Tiles 8-way
    private SerializedProperty north;
    private SerializedProperty south;
    private SerializedProperty east;

    private SerializedProperty west;

    // Tiles 12-way
    private SerializedProperty northWestDiag;
    private SerializedProperty northEastDiag;
    private SerializedProperty southWestDiag;
    private SerializedProperty southEastDiag;

    private void OnEnable()
    {
        tillingType = serializedObject.FindProperty("tillingType");

        northWest = serializedObject.FindProperty("northWest");
        northEast = serializedObject.FindProperty("northEast");
        southWest = serializedObject.FindProperty("southWest");
        southEast = serializedObject.FindProperty("southEast");

        north = serializedObject.FindProperty("north");
        south = serializedObject.FindProperty("south");
        east = serializedObject.FindProperty("east");
        west = serializedObject.FindProperty("west");

        northWestDiag = serializedObject.FindProperty("northWestDiag");
        northEastDiag = serializedObject.FindProperty("northEastDiag");
        southWestDiag = serializedObject.FindProperty("southWestDiag");
        southEastDiag = serializedObject.FindProperty("southEastDiag");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(tillingType);

        EditorGUILayout.PropertyField(northWest);
        EditorGUILayout.PropertyField(northEast);
        EditorGUILayout.PropertyField(southWest);
        EditorGUILayout.PropertyField(southEast);

        if (tillingType.enumValueIndex > 0)
        {
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(north);
            EditorGUILayout.PropertyField(south);
            EditorGUILayout.PropertyField(east);
            EditorGUILayout.PropertyField(west);
        }

        if (tillingType.enumValueIndex > 1)
        {
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(northWestDiag);
            EditorGUILayout.PropertyField(northEastDiag);
            EditorGUILayout.PropertyField(southWestDiag);
            EditorGUILayout.PropertyField(southEastDiag);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
}
