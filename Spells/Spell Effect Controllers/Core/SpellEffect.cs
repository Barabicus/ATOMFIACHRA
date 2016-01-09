using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Spell effects modify how the spell behaves. For example the missile motor will control how the spell will move through space
///  while the beam motor will raycast a beam through world space. Spell effects are intended to be drivers for the actual spell
///  and will not execute spell logic.
/// </summary>
public abstract class SpellEffect : MonoBehaviour
{
    [HideInInspector]
    public EffectSetting effectSetting;
    [SerializeField]
    [Tooltip("Detached the gameobject from the spell when the spell is initialised")]
    private bool _detached = false;
    public bool onlyUpdateOnSpellEnabled = true;

    private float _currentLivingTime;

    public float CurrentLivingTime
    {
        get { return _currentLivingTime; }
        protected set { _currentLivingTime = value; }
    }

    /// <summary>
    /// A percent from 0-1 on how long this spell has been alive compared to its live time.
    /// </summary>
    public float CurrentLivingTimePercent
    {
        get
        {
            return CurrentLivingTime / effectSetting.spell.SpellLiveTime;
        }
    }

    protected bool OnlyUpdateOnSpellEnabled
    {
        get { return onlyUpdateOnSpellEnabled && !effectSetting.spell.enabled; }
    }

    public virtual void Initialize(EffectSetting effectSetting)
    {
        this.effectSetting = effectSetting;
        effectSetting.OnSpellDestroy += () => effectSetting_OnSpellDestroy();
        effectSetting.OnSpellCollision += effectSetting_OnSpellCollision;
        effectSetting.OnEffectDestroy += effectSetting_OnEffectDestroy;
        effectSetting.OnSpellApply += effectSetting_OnSpellApply;
        effectSetting.OnSpellCast += effectSetting_OnSpellCast;
        effectSetting.OnSpellReset += effectSetting_OnSpellReset;
        effectSetting.OnSpellStart += OnSpellStart;
        effectSetting.OnSpecialEvent += OnSpecialEvent;
        effectSetting.OnSpellRefresh += OnSpellRefresh;

        if (_detached)
        {
            gameObject.transform.parent = null;
        }
    }

    /// <summary>
    /// This is called when the spell start event is called from EffectSetting
    /// </summary>
    protected virtual void OnSpellStart()
    {
        CurrentLivingTime = 0f;
    }

    protected virtual void OnSpellRefresh()
    {
    }

    protected virtual void effectSetting_OnSpellReset()
    {
    }

    protected virtual void effectSetting_OnSpellCast()
    {
    }

    protected virtual void effectSetting_OnSpellApply(Entity entity)
    {
    }

    protected virtual void effectSetting_OnEffectDestroy()
    {
    }

    protected virtual void effectSetting_OnSpellCollision(ColliderEventArgs args, Collider obj)
    {
    }

    protected virtual void effectSetting_OnSpellDestroy()
    {
    }
    protected virtual void OnSpecialEvent(string eventID)
    {
    }

    /// <summary>
    /// This should control update flow and not update logic
    /// </summary>
    protected virtual void Update()
    {
        // If only update on spell enabled is checked, check to see if the spell is enabled
        // if not return
        if (OnlyUpdateOnSpellEnabled)
            return;
        UpdateSpell();

        _currentLivingTime += Time.deltaTime;
    }

    /// <summary>
    /// This should control spell update logic
    /// </summary>
    protected virtual void UpdateSpell() { }

}

public class ColliderEventArgs : EventArgs
{
    public Vector3 HitPoint { get; set; }
    public Vector3 HitNormal { get; set; }
    public Collider HitCollider { get; set; }
    public ColliderEventArgs()
    {
    }

}
