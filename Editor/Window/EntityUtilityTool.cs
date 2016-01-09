using UnityEngine;
using System.Collections;
using UnityEditor;

class EntityUtilityTool : EditorWindow
{
    private EntityUtilityMethod _utilityMethod;

    public enum EntityUtilityMethod
    {
        None,
        SetDeathFx
    }

    [MenuItem("Window/Entity Utility Tool")]
    public static void ShowWindow()
    {
        var window = EditorWindow.GetWindow(typeof(EntityUtilityTool)) as EntityUtilityTool;
      //  Selection.selectionChanged = window.SelectionChanged;
    }

    public void SelectionChanged()
    {
        Repaint();
        Debug.Log("reapinted");
    }

    void OnGUI()
    {
        EditorGUILayout.BeginVertical();

        _utilityMethod = (EntityUtilityMethod)EditorGUILayout.EnumPopup(new GUIContent("Utility Method"), _utilityMethod);

        EditorGUILayout.BeginVertical("box");
        switch (_utilityMethod)
        {
            case EntityUtilityMethod.None:
                DoNone();
                break;
            case EntityUtilityMethod.SetDeathFx:
                DoSetDeathFx();
                break;
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndVertical();
    }

    private void DoNone()
    {
        EditorGUILayout.LabelField(new GUIContent("Please select a Entity Utility Method"));
    }

    private void DoSetDeathFx()
    {
        EditorGUILayout.LabelField("Selected: " + Selection.objects.Length);
    }
}
