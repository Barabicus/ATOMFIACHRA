using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(Quest), true)]
public class QuestInspector : StandardComponentInspector
{
    private List<QuestCategoryContainer> _entityCategoryItems;
    private List<QuestCategoryContainer> _locationCategoryItems;
    private List<QuestCategoryContainer> _miscCategoryItems;
    private List<QuestCategoryContainer> _uiCategoryItems;
    private List<QuestCategoryContainer> _specialCategoryItems;
    private List<QuestCategoryContainer> _playerCategoryItems;

    private List<Entity> _entityPrefabs;
    private Quest _questTarget;


    protected override void OnEnable()
    {
        base.OnEnable();
        _entityCategoryItems = new List<QuestCategoryContainer>();
        _locationCategoryItems = new List<QuestCategoryContainer>();
        _miscCategoryItems = new List<QuestCategoryContainer>();
        _uiCategoryItems = new List<QuestCategoryContainer>();
        _specialCategoryItems = new List<QuestCategoryContainer>();
        _playerCategoryItems = new List<QuestCategoryContainer>();

        _entityPrefabs = new List<Entity>();
        _questTarget = target as Quest;

        // Get all quests and sort them based on the respective quest category attribute
        foreach (var a in QuestUtilityTools.GetCategoryList())
        {
            switch (a.Attribute.Category)
            {
                case QuestCategory.Entity:
                    _entityCategoryItems.Add(a);
                    break;
                case QuestCategory.Location:
                    _locationCategoryItems.Add(a);
                    break;
                case QuestCategory.Misc:
                    _miscCategoryItems.Add(a);
                    break;
                case QuestCategory.UI:
                    _uiCategoryItems.Add(a);
                    break;
                case QuestCategory.Special:
                    _specialCategoryItems.Add(a);
                    break;
                case QuestCategory.Player:
                    _playerCategoryItems.Add(a);
                    break;
            }
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DrawCompletion();
        DrawSingleComponentCategory("Special", _specialCategoryItems);
        DrawMultipleComponentCategory("Entity", _entityCategoryItems);
        DrawMultipleComponentCategory("Location", _locationCategoryItems);
        DrawMultipleComponentCategory("Misc", _miscCategoryItems);
        DrawMultipleComponentCategory("UI", _uiCategoryItems);
        DrawMultipleComponentCategory("Player", _playerCategoryItems);
    }

    private void DrawCompletion()
    {
        if (Application.isPlaying)
        {
            EditorGUILayout.LabelField("Completion Status: " + _questTarget.IsQuestComplete);
        }
    }

    private void DrawMultipleComponentCategory(string titleName, List<QuestCategoryContainer> container)
    {
        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.LabelField(titleName, EditorStyles.boldLabel);

        foreach (var spellCategoryContainer in container)
        {
            DrawAddComponent(spellCategoryContainer.Attribute.ListingName, "Quest", spellCategoryContainer.Type);
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawSingleComponentCategory(string titleName, List<QuestCategoryContainer> container)
    {
        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.LabelField(titleName, EditorStyles.boldLabel);

        foreach (var spellCategoryContainer in container)
        {
            DrawHasComponent(spellCategoryContainer.Attribute.ListingName, spellCategoryContainer.Type);
        }

        EditorGUILayout.EndVertical();
    }

    [MenuItem("GameObject/Create Other/Quest", false, 1)]
    public static void CreateQuestMenu()
    {
        GameObject go = new GameObject("Quest");
        Undo.RegisterCreatedObjectUndo(go, "Undo Create Quest");
        go.AddComponent<Quest>();
        if (Selection.activeGameObject != null)
        {
            var count = Selection.activeGameObject.GetComponentsInChildren<Quest>().Length;
            if (count > 0)
            {
                go.name = "Quest(" + count + ")";
            }
            go.transform.parent = Selection.activeGameObject.transform;
            go.transform.localPosition = Vector3.zero;
        }
        Selection.activeObject = go;
    }

}
