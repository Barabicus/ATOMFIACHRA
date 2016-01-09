using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(HelpBoxInfo))]
public class HelpBoxInfoInspector : Editor
{
    private ReorderableList _list;

    private void OnEnable()
    {
        _list = new ReorderableList(serializedObject,
                serializedObject.FindProperty("_pages"),
                true, true, true, true);

        _list.drawElementCallback = DrawListCallBack;
        _list.elementHeight = EditorGUIUtility.singleLineHeight * 7f;
    }

    private void DrawListCallBack(Rect rect, int index, bool isActive, bool isFocused)
    {
        var element = _list.serializedProperty.GetArrayElementAtIndex(index);
        rect.y += 2;
        SerializedProperty pageText = element.FindPropertyRelative("_pageText");
        EditorStyles.textField.wordWrap = true;
        EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Page " + index));
        EditorGUI.PropertyField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("_helpImage"), GUIContent.none);
        pageText.stringValue = EditorGUI.TextArea(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 2f, rect.width, EditorGUIUtility.singleLineHeight * 4f), pageText.stringValue);

        //EditorGUI.PropertyField(
        //    new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight),
        //    element.FindPropertyRelative("Type"), GUIContent.none);
        //EditorGUI.PropertyField(
        //    new Rect(rect.x + 60, rect.y, rect.width - 60 - 30, EditorGUIUtility.singleLineHeight),
        //    element.FindPropertyRelative("Prefab"), GUIContent.none);
        //EditorGUI.PropertyField(
        //    new Rect(rect.x + rect.width - 30, rect.y, 30, EditorGUIUtility.singleLineHeight),
        //    element.FindPropertyRelative("Count"), GUIContent.none);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        _list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }

}
