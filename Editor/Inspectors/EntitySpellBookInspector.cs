using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(EntitySpellBook))]
public class EntitySpellBookInspector : Editor
{
    private ReorderableList _spellBookList;
    private SerializedProperty _spellBook; 

    void OnEnable()
    {

        _spellBook = serializedObject.FindProperty("_spellBook");

        // Setup Patrol List
        _spellBookList = new ReorderableList(serializedObject, _spellBook, true, true, true, true);

        _spellBookList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Spell Book");
        };

        _spellBookList.drawElementCallback = PatrolPointsCallBack;

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        _spellBookList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();

        DrawDefaultInspector();
    }

    private void PatrolPointsCallBack(Rect rect, int index, bool isActive, bool isFocused)
    {
        //var element = _spellBookList.serializedProperty.GetArrayElementAtIndex(index);

        //element.objectReferenceValue = EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent(""), element.objectReferenceValue, typeof(Spell), false);
    }


}
