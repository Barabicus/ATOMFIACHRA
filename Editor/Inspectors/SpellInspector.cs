using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

[CustomEditor(typeof(Spell), true)]
public class SpellInspector : Editor
{
    private ElementalApply elementalApply;
    private AddStatModifier statMod;
    private Spell spellTarget;

    private ElementalStats _dpsEstimate;
    private ElementalStats _lifeTimeDamageEstimate;

    private SerializedProperty _spellID;
    private SerializedProperty _spellName;
    private SerializedProperty _spellKillMethod;
    private SerializedProperty _spellLiveTime;
    private SerializedProperty _spellCastDelay;
    private SerializedProperty _spellType;
    private SerializedProperty _refreshAttach;
    private SerializedProperty _spellDeathMarker;
    private SerializedProperty _castAudio;
    private SerializedProperty _spellIcon;
    private SerializedProperty _spellDescription;
    private SerializedProperty _elementalCost;
    private SerializedProperty _elementalType;
    private SerializedProperty _forceUpdateAmount;

    private List<SpellCategoryContainer> _entityCategoryItems;
    private List<SpellCategoryContainer> _spellCategoryItems;
    private List<SpellCategoryContainer> _motorCategoryItems;
    private List<SpellCategoryContainer> _fxCategoryItems;
    void OnEnable()
    {
        spellTarget = target as Spell;

        _entityCategoryItems = new List<SpellCategoryContainer>();
        _spellCategoryItems = new List<SpellCategoryContainer>();
        _motorCategoryItems = new List<SpellCategoryContainer>();
        _fxCategoryItems = new List<SpellCategoryContainer>();

        // Find all Spell Category attributes and sort them accordingly
        foreach (var a in SpellUtilityTools.GetSpellCategoryList())
        {
            switch (a.Attribute.Category)
            {
                case SpellEffectCategory.Motor:
                    _motorCategoryItems.Add(a);
                    break;
                case SpellEffectCategory.Entity:
                    _entityCategoryItems.Add(a);
                    break;
                case SpellEffectCategory.Spell:
                    _spellCategoryItems.Add(a);
                    break;
                case SpellEffectCategory.FX:
                    _fxCategoryItems.Add(a);
                    break;
            }
        }
        _lifeTimeDamageEstimate = CalculateLifeTimeDamage();
        _dpsEstimate = CalculateTickDamage();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();
        UpdateVariables();
        DrawDpsEstimate();
        DrawSpellHelperFunctions();

        serializedObject.ApplyModifiedProperties();
    }


    private void UpdateVariables()
    {
        elementalApply = GetChildComponent<ElementalApply>();
        statMod = GetChildComponent<AddStatModifier>();
    }

    #region Drawing

    private void DrawDpsEstimate()
    {
        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField(new GUIContent("LifeTime Damage Estimate: " + _lifeTimeDamageEstimate));
        EditorGUILayout.LabelField(new GUIContent("Magnitude: " + _lifeTimeDamageEstimate.Magnitude));
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField(new GUIContent("Tick Estimate: " + _dpsEstimate));
        EditorGUILayout.LabelField(new GUIContent("Magnitude: " + _dpsEstimate.Magnitude));
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndVertical();
    }

    private ElementalStats CalculateLifeTimeDamage()
    {
        ElementalStats overallDamage = ElementalStats.Zero;

        ElementalApply[] applies = spellTarget.GetComponentsInChildren<ElementalApply>(true);

        TimedUpdateableSpellMotor motor = GetChildComponent<TimedUpdateableSpellMotor>();

        if (motor != null && !motor.singleShot)
        {
            foreach (var apply in applies)
            {
                overallDamage += apply.ElementalPower*(spellTarget.SpellLiveTime/ ((motor.updateDelay)));
            }
        }
        else
        {
            foreach (var apply in applies)
            {
                overallDamage += apply.ElementalPower;
            }
        }

        return overallDamage;
    }

    private ElementalStats CalculateTickDamage()
    {
        var motor = GetChildComponent<TimedUpdateableSpellMotor>();

        if (motor != null)
        {
            ElementalApply[] applies = spellTarget.GetComponentsInChildren<ElementalApply>(true);
            ElementalStats overallStats = ElementalStats.Zero;
            foreach (var apply in applies)
            {
                overallStats += apply.ElementalPower;
            }

            return overallStats;
        }
        return ElementalStats.Zero;

    }

    private void DrawSpellHelperFunctions()
    {
        //    DrawElementalPower();
        DrawSingleComponentCategory("Motor", _motorCategoryItems);
        DrawMultipleComponentCategory("Entity", _entityCategoryItems);
        DrawMultipleComponentCategory("Spell", _spellCategoryItems);
        DrawMultipleComponentCategory("FX", _fxCategoryItems);
    }

    private void DrawElementalPower()
    {
        DrawAddType<ElementalApply>(elementalApply, ElementalPowerCallBack, "Elemental Power");
        DrawAddType<AddStatModifier>(statMod, StatModCallBack, "Stat Modifier");
    }

    private void DrawMultipleComponentCategory(string titleName, List<SpellCategoryContainer> container)
    {
        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.LabelField(titleName, EditorStyles.boldLabel);

        foreach (var spellCategoryContainer in container)
        {
            DrawAddComponent(spellCategoryContainer.Attribute.ListingName, spellCategoryContainer.Type);
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawSingleComponentCategory(string titleName, List<SpellCategoryContainer> container)
    {
        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.LabelField(titleName, EditorStyles.boldLabel);

        foreach (var spellCategoryContainer in container)
        {
            DrawHasComponent(spellCategoryContainer.Attribute.ListingName, spellCategoryContainer.Type);
        }

        EditorGUILayout.EndVertical();
    }

    #endregion

    #region Call Backs

    private void StatModCallBack(AddStatModifier statMod)
    {
        EntityStats setStats = statMod.StatModifier;
        setStats.Speed = EditorGUILayout.FloatField("Speed Mod", setStats.Speed);
        statMod.StatModifier = setStats;
    }

    private void ElementalPowerCallBack(ElementalApply elemental)
    {
        ElementalStats setStats = elementalApply.ElementalPower;
        setStats.fire = EditorGUILayout.FloatField("Fire", setStats.fire);
        setStats.water = EditorGUILayout.FloatField("Water", setStats.water);
        setStats.air = EditorGUILayout.FloatField("Air", setStats.air);
        setStats.earth = EditorGUILayout.FloatField("Earth", setStats.earth);
        setStats.physical = EditorGUILayout.FloatField("Physical", setStats.physical);
        elementalApply.ElementalPower = setStats;
    }

    #endregion

    #region Creation & Get

    /// <summary>
    /// Unity's get child components is not working. Substitute with this
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private List<T> GetChildComponents<T>()
    {
        List<T> comps = new List<T>();
        foreach (Transform t in spellTarget.transform)
        {
            T comp = t.GetComponent<T>();
            if (comp != null)
                comps.Add(comp);
        }
        return comps;
    }

    /// <summary>
    /// Unity's get child component is not working. Substitute with this
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private T GetChildComponent<T>()
    {
        foreach (Transform t in spellTarget.transform)
        {
            T comp = t.GetComponent<T>();
            if (comp != null)
                return comp;
        }
        return default(T);
    }

    /// <summary>
    /// Draw an option to add more than one component. Will add the component and specify how many components of that time are added
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="title"></param>
    private void DrawAddComponent<T>(string title) where T : Component
    {
        EditorGUILayout.BeginHorizontal("box");
        EditorGUILayout.LabelField(title + " (" + GetChildComponents<T>().Count + ")", EditorStyles.boldLabel);

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Create Effect"))
        {
            GameObject go = new GameObject(title + " (" + GetChildComponents<T>().Count + ")");
            Undo.RegisterCreatedObjectUndo(go, "Created " + title);
            go.layer = LayerMask.NameToLayer("Spell");
            go.AddComponent<T>();
            go.transform.parent = spellTarget.transform;
            go.transform.localPosition = Vector3.zero;
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Draw an option to add more than one component. Will add the component and specify how many components of that time are added
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="title"></param>
    private void DrawAddComponent(string title, Type type)
    {
        EditorGUILayout.BeginHorizontal("box");
        EditorGUILayout.LabelField(title + " (" + spellTarget.gameObject.GetComponentsInChildren(type, true).Length + ")", EditorStyles.boldLabel);

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Create Effect"))
        {
            GameObject go = new GameObject(title + " (" + spellTarget.gameObject.GetComponentsInChildren(type, true).Length + ")");
            Undo.RegisterCreatedObjectUndo(go, "Created " + title);
            go.layer = LayerMask.NameToLayer("Spell");
            go.AddComponent(type);
            go.transform.parent = spellTarget.transform;
            go.transform.localPosition = Vector3.zero;
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Draw an option to add a single component of a specified type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="associatedObject"></param>
    /// <param name="title"></param>
    private void DrawHasComponent<T>(string title) where T : Component
    {
        EditorGUILayout.BeginHorizontal("box");
        // Rect rect = EditorGUILayout.GetControlRect();
        Color c;

        var associatedObject = GetChildComponent<T>();

        if (associatedObject == null)
            c = Color.white;
        else
            c = new Color(0, 0.93f, 0);
        GUI.color = c;
        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        GUI.color = Color.white;

        if (associatedObject == null)
        {
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Add Motor"))
            {
                GameObject go = new GameObject(title);
                Undo.RegisterCreatedObjectUndo(go, "Created " + title);
                go.layer = LayerMask.NameToLayer("Spell");
                go.AddComponent<T>();
                go.transform.parent = spellTarget.transform;
                go.transform.localPosition = Vector3.zero;
                UpdateVariables();
            }
            GUI.backgroundColor = Color.white;
        }
        EditorGUILayout.EndHorizontal();
    }
    /// <summary>
    /// Draw an option to add a single component of a specified type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="associatedObject"></param>
    /// <param name="title"></param>
    private void DrawHasComponent(string title, Type type)
    {
        EditorGUILayout.BeginHorizontal("box");
        // Rect rect = EditorGUILayout.GetControlRect();
        Color c;

        var associatedObject = spellTarget.gameObject.GetComponentsInChildren(type, true);
        if (associatedObject.Length == 0)
            c = Color.white;
        else
            c = new Color(0, 0.93f, 0);
        GUI.color = c;
        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        GUI.color = Color.white;

        if (associatedObject.Length == 0)
        {
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Add Motor"))
            {
                GameObject go = new GameObject(title);
                Undo.RegisterCreatedObjectUndo(go, "Created " + title);
                go.layer = LayerMask.NameToLayer("Spell");
                go.AddComponent(type);
                go.transform.parent = spellTarget.transform;
                go.transform.localPosition = Vector3.zero;
                UpdateVariables();
            }
            GUI.backgroundColor = Color.white;
        }
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Draw an option to add a component with a specific call back for when it is added
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="associatedObject"></param>
    /// <param name="drawAction"></param>
    /// <param name="title"></param>
    private void DrawAddType<T>(T associatedObject, Action<T> drawAction, string title) where T : UnityEngine.Component
    {
        if (associatedObject != null)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            drawAction(associatedObject);
            EditorUtility.SetDirty(associatedObject);
            EditorGUILayout.EndVertical();
        }
        else
            if (GUILayout.Button(new GUIContent("Add " + title)))
            {
                GameObject go = new GameObject(title);
                go.layer = LayerMask.NameToLayer("Spell");
                go.AddComponent<T>();
                go.transform.parent = spellTarget.transform;
                go.transform.localPosition = Vector3.zero;
                UpdateVariables();
            }
    }

    #endregion
}
