using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(ObjectTransitionFx), true)]
public class ObjectTransitionFxInspector : StandardComponentInspector
{

    private List<TransitionFxCategoryContainer> _entityItems;
    private List<TransitionFxCategoryContainer> _fxItems;


    private ObjectTransitionFx _target;


    protected override void OnEnable()
    {
        base.OnEnable();
        _entityItems = new List<TransitionFxCategoryContainer>();
        _fxItems = new List<TransitionFxCategoryContainer>();


        _target = target as ObjectTransitionFx;

        // Get all quests and sort them based on the respective quest category attribute
        foreach (var a in TransitionFxTools.GetCategoryList())
        {
            switch (a.Attribute.Category)
            {
                case TransitionFxCategory.Entity:
                    _entityItems.Add(a);
                    break;
                case TransitionFxCategory.Fx:
                    _fxItems.Add(a);
                    break;
            }
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DrawMultipleComponentCategory("Entity", _entityItems);
        DrawMultipleComponentCategory("Fx", _fxItems);

    }


    private void DrawMultipleComponentCategory(string titleName, List<TransitionFxCategoryContainer> container)
    {
        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.LabelField(titleName, EditorStyles.boldLabel);

        foreach (var spellCategoryContainer in container)
        {
            DrawAddComponent(spellCategoryContainer.Attribute.ListingName, "Fx", spellCategoryContainer.Type);
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawSingleComponentCategory(string titleName, List<TransitionFxCategoryContainer> container)
    {
        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.LabelField(titleName, EditorStyles.boldLabel);

        foreach (var spellCategoryContainer in container)
        {
            DrawHasComponent(spellCategoryContainer.Attribute.ListingName, spellCategoryContainer.Type);
        }

        EditorGUILayout.EndVertical();
    }

}
