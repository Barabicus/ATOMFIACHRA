using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class EffectSetting : MonoBehaviour
{

    public Spell spell;
    public bool destroyOnCollision;
    public float destroyTimeDelay = 0f;
    public event Action OnSpellStart;
    public event Action OnSpellDestroy;
    public event Action<ColliderEventArgs, Collider> OnSpellCollision;
    public event Action<Entity> OnSpellApply;
    public event Action OnEffectDestroy;
    public event Action OnSpellCast;
    public event Action OnSpellReset;
    public event Action<string> OnSpecialEvent;
    public event Action OnSpellRefresh;

    private SpellEffect[] _spellEffects;

    /// <summary>
    /// This is called once to setup initial references that will persist with the EffectSetting throughout the duration of the game.
    /// </summary>
    public void Initialize()
    {
        spell = GetComponent<Spell>();
        spell.OnSpellDestroy += spell_OnSpellDestroy;
        spell.OnSpellRecycle += OnSpellRecycle;
        spell.OnSpellRefresh += TriggerSpellRefresh;

        _spellEffects = transform.GetComponentsInChildren<SpellEffect>(true);

        foreach (var componentsInChild in _spellEffects)
        {
            componentsInChild.Initialize(this);
        }

    }

    #region Triggers
    /// <summary>
    /// Triggers a spell refresh event. The spell refresh indicates that the spells destroy timer has been refreshed. 
    /// Typically this will be an attached spell refreshing the timer.
    /// </summary>
    /// <param name="obj"></param>
    private void TriggerSpellRefresh(Spell obj)
    {
        if(OnSpellRefresh != null)
        {
            OnSpellRefresh();
        }
    }
    /// <summary>
    /// This is called just as the spell has been created. Just before the game object is set to active.
    /// </summary>
    public void TriggerSpellStart()
    {
        // Re-register spell destroy with this effect setting.
        spell.OnSpellDestroy += spell_OnSpellDestroy;

        // Ensure all spell effects are enabled
        foreach(var effect in _spellEffects)
        {
            effect.gameObject.SetActive(true);
        }

        if (OnSpellStart != null)
            OnSpellStart();
        if (OnSpellCast != null)
            OnSpellCast();
    }
    /// <summary>
    /// Calling this will trigger a collision in all of the spell effects
    /// </summary>
    /// <param name="args"></param>
    /// <param name="other"></param>
    public void TriggerCollision(ColliderEventArgs args, Collider other)
    {
        if (OnSpellCollision != null)
            OnSpellCollision(args, other);
        if (destroyOnCollision)
        {
            TriggerDestroySpell();
        }
    }
    /// <summary>
    /// Trigger a spell apply to be fired. This will tell all SpellEffects to apply what ever effects they have.
    /// </summary>
    /// <param name="entity"></param>
    public void TriggerApplySpell(Entity entity)
    {
        if (OnSpellApply != null)
            OnSpellApply(entity);
    }
    /// <summary>
    /// Trigger a spell Destroy to be fired. This will force the spell to destroy itself.
    /// </summary>
    public void TriggerDestroySpell()
    {
        spell.TriggerDestroySpell();
    }
    public void TriggerSpecialEvent(string eventID)
    {
        if(OnSpecialEvent != null)
        {
            OnSpecialEvent(eventID);
        }
    }
    /// <summary>
    /// This triggers an Effect Reset which will cause all SpellEffects to reset themselves. This is called just
    /// before the spell is returned to the pool.
    /// </summary>
    private void TriggerEffectReset()
    {
        if (OnSpellReset != null)
            OnSpellReset();
        ResetEffect();
    }
    #endregion

    #region Event Listeners
    /// <summary>
    /// This is called when the Spell itself has been destroyed. This will allow sometime for the effects to finish off
    /// any visual effects before the spell is removed from the world and returned to the pool.
    /// </summary>
    /// <param name="spell"></param>
    private void spell_OnSpellDestroy(Spell spell)
    {
        if (OnSpellDestroy != null)
            OnSpellDestroy();

        Invoke("DestroyGameObject", destroyTimeDelay);
    }

    private void OnSpellRecycle(Spell obj)
    {
        // Disable all spell effects when the spell is disabled
        // This is because the spell effects may not be children
        // to the spell object.
        foreach (var effect in _spellEffects)
        {
            effect.gameObject.SetActive(false);
        }
    }

    #endregion
    /// <summary>
    /// Called by invoke on spell destroy.
    /// </summary>
    private void DestroyGameObject()
    {
        CancelInvoke();
        if (OnEffectDestroy != null)
            OnEffectDestroy();

        TriggerEffectReset();
        SpellPool.Instance.PoolObject(spell);
    }
    /// <summary>
    /// This will return the spell to the pool.
    /// </summary>
    private void ResetEffect()
    {
        // If the spell is attached, detach it from the entity.
        transform.parent = null;
    }

}