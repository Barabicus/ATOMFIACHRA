using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Entity))]
public class EntityInspector : Editor
{
    private Entity _targetEntity;

    private float _baseStatMult = 50;
    private bool _autoSpeed = false;
    private bool _autoHealth = true;
    private bool _autoElemModifer = true;
    private bool _autoElemRecharge = false;
    private bool _autoElemResistance = false;
    private bool _autoElemChargeamount = false;

    private void OnEnable()
    {
        _targetEntity = target as Entity;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DrawEntityState();
        DrawLevelHandler();
    }

    private void DrawEntityState()
    {
        if (_targetEntity.GUIHealthPoint == null && GUILayout.Button(new GUIContent("Create Health Point")))
        {
            GameObject point = new GameObject("GuiHealthPoint");
            point.transform.parent = _targetEntity.transform;
            point.transform.localPosition = new Vector3(0, 3f, 0);
            _targetEntity.GUIHealthPoint = point.transform;
        }
        if (_targetEntity.GUISpeechPoint == null && GUILayout.Button(new GUIContent("Create Speech Point")))
        {
            GameObject point = new GameObject("GuiSpeechPoint");
            point.transform.parent = _targetEntity.transform;
            point.transform.localPosition = new Vector3(0, 3f, 0);
            _targetEntity.GUISpeechPoint = point.transform;
        }
        if (_targetEntity.EntityCastPoint == null && GUILayout.Button(new GUIContent("Create Entity Cast Point")))
        {
            GameObject point = new GameObject("EntityCastPoint");
            point.transform.parent = _targetEntity.transform;
            point.transform.localPosition = new Vector3(0, 0f, 0);
            _targetEntity.EntityCastPoint = point.AddComponent<EntityCastPoint>();
        }
    }

    private void DrawLevelHandler()
    {
        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.LabelField(new GUIContent("Auto Level Handler"));
        _baseStatMult = EditorGUILayout.FloatField(new GUIContent("Increment Rule"), _baseStatMult);

        _autoSpeed = EditorGUILayout.Toggle(new GUIContent("Speed"), _autoSpeed);
        _autoHealth = EditorGUILayout.Toggle(new GUIContent("Health"), _autoHealth);
        _autoElemModifer = EditorGUILayout.Toggle(new GUIContent("Elem Modifier"), _autoElemModifer);
        _autoElemRecharge = EditorGUILayout.Toggle(new GUIContent("Elem Recharge"), _autoElemRecharge);
        _autoElemResistance = EditorGUILayout.Toggle(new GUIContent("Elem Resistance"), _autoElemResistance);
        _autoElemChargeamount = EditorGUILayout.Toggle(new GUIContent("Charge Amount"), _autoElemChargeamount);

        if (GUILayout.Button("Apply"))
        {
            DoStatApply();
        }

        EditorGUILayout.EndHorizontal();
    }

    private void DoStatApply()
    {
        Undo.RecordObject(_targetEntity, "Auto Level Handler");

        if (_autoSpeed)
        {
            _targetEntity.LevelHandler.MaxSpeed = _targetEntity.LevelHandler.MinSpeed*_baseStatMult;
        }
        if (_autoHealth)
        {
            _targetEntity.LevelHandler.MaxHealth = _targetEntity.LevelHandler.MinHealth*_baseStatMult;
        }
        if (_autoElemModifer)
        {
            _targetEntity.LevelHandler.MaxElementalModifier = _targetEntity.LevelHandler.MinElementalModifier*
                                                              _baseStatMult;
        }
        if (_autoElemResistance)
        {
            _targetEntity.LevelHandler.MaxElementalResistance = _targetEntity.LevelHandler.MinElementalResistance *
                                                              _baseStatMult;
        }
        if (_autoElemChargeamount)
        {
            _targetEntity.LevelHandler.MaxElementalCharge = _targetEntity.LevelHandler.MinElementalCharge *
                                                              _baseStatMult;
        }
        if (_autoElemRecharge)
        {
            _targetEntity.LevelHandler.MaxRechargeRate = _targetEntity.LevelHandler.MinRechargeRate *
                                                              _baseStatMult;
        }
    }
}
