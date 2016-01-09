using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;
using UnityEditorInternal;

[CustomEditor(typeof(SpellEffectStandard), true)]
public class SpellStandardInspector : Editor
{

    private SerializedProperty _triggerEvent;
    private SerializedProperty _timeTrigger;
    private SerializedProperty _isSingleShot;
    private SerializedProperty _entityTargetMethod;
    private SerializedProperty _tickOnStart;
    private SerializedProperty _positionTarget;
    // private SerializedProperty _listenForEventID;

    private ReorderableList _eventIDList;

    private SpellEffectStandard _spellStand;
    private void OnEnable()
    {
        _spellStand = target as SpellEffectStandard;
        _triggerEvent = serializedObject.FindProperty("_triggerEvent");
        _timeTrigger = serializedObject.FindProperty("_timeTrigger");
        _isSingleShot = serializedObject.FindProperty("_isSingleShot");
        _entityTargetMethod = serializedObject.FindProperty("_entityTargetMethod");
        _tickOnStart = serializedObject.FindProperty("_tickOnStart");
        _positionTarget = serializedObject.FindProperty("_positionTarget");
        ///   _listenForEventID = serializedObject.FindProperty("_listenForEventID");

        _eventIDList = new ReorderableList(serializedObject,
             serializedObject.FindProperty("_listenForEventIDs"),
             true, true, true, true);
        _eventIDList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Listen For Event IDs");
        };
        _eventIDList.drawElementCallback = DrawEventIDsCallBack;
    }

    private void DrawEventIDsCallBack(Rect rect, int index, bool isActive, bool isFocused)
    {
        var element = _eventIDList.serializedProperty.GetArrayElementAtIndex(index);
        rect.y += 2;
        EditorGUI.PropertyField(
    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
    element, GUIContent.none);
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginVertical("box");

        serializedObject.Update();

        var attr = _spellStand.GetType().GetCustomAttributes(typeof(SpellEffectStandardAttribute), true).Cast<SpellEffectStandardAttribute>().FirstOrDefault();

        if (attr != null)
        {
            EditorGUILayout.HelpBox(attr.SpellInfo, MessageType.Info);
        }

        if (_spellStand.EntityTargetMethod == SpellEffectStandard.EntityTarget.ApplyEntity && _spellStand.TriggerEvent != SpellEffectTriggerEvent.SpellApply)
        {
            // Force spell apply event
            _spellStand.TriggerEvent = SpellEffectTriggerEvent.SpellApply;
        }

        if (_spellStand.EntityTargetMethod == SpellEffectStandard.EntityTarget.ApplyEntity)
        {
            EditorGUILayout.HelpBox("Apply Entity requires SpellApply event. Forcing event type to spell apply", MessageType.Warning);
        }

        EditorGUILayout.PropertyField(_triggerEvent, new GUIContent("Trigger Event", "The event that should cause this effect to be triggered"));
        if (_spellStand.TriggerEvent == SpellEffectTriggerEvent.Timed)
        {
            EditorGUILayout.PropertyField(_timeTrigger, new GUIContent("Time Trigger", "How long should this event take to trigger"));
            EditorGUILayout.PropertyField(_isSingleShot, new GUIContent("Is Single Shot", "should this spell effect be only triggered once"));
            EditorGUILayout.PropertyField(_tickOnStart, new GUIContent("Tick On Start", "Should the spell tick when it is initially started"));

        }
        if (_spellStand.TriggerEvent == SpellEffectTriggerEvent.SpecialEvent)
        {
            //  EditorGUILayout.PropertyField(_listenForEventID, new GUIContent("Listen For Event ID", "When a special event is called it is assigned a event ID. Spell effects can listen for a specific ID and only trigger when that ID is found"));
            _eventIDList.DoLayoutList();
        }

        if (attr != null)
        {
            if (attr.HasTargetEntity)
            {
                EditorGUILayout.PropertyField(_entityTargetMethod, new GUIContent("Entity Target", "What method should be used to retrieve the target entity. If using ApplyEntity it will use the Entity passed in from the SpellApply event. If using caster it will use the casting entity. If using LastApplyEnity it will use the entity the spell was last applied to (if it exists)"));
            }
            if (attr.HasTargetPosition)
            {
                EditorGUILayout.PropertyField(_positionTarget, new GUIContent("Target Position", "The position target is used as a means to determain a target position for a specific spell. Modifying this method will cause the spell to perform an action within the position of interest."));
            }
        }
        else if (_spellStand.EntityTargetMethod != SpellEffectStandard.EntityTarget.None)
        {
            // Ensure there is no target entity method set
            _spellStand.EntityTargetMethod = SpellEffectStandard.EntityTarget.None;
        }

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.EndVertical();
        base.OnInspectorGUI();
    }

}
