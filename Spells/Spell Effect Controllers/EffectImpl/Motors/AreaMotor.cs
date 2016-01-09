using UnityEngine;
using System.Collections;

[SpellCategory("Area Motor", SpellEffectCategory.Motor)]
[SpellEffectStandard(false, false, "The area motor will attempt to trigger a collision and subsequent spell apply events to targets within range. The trigger event should typically be set to timed.")]
public class AreaMotor : SpellMotor
{
    public float radius = 5f;
    [SerializeField]
    [Tooltip("If this is true the area motor will keep it's position inlined with the casting Entity")]
    private bool _followCaster;
    [SerializeField]
    private bool _setToTargetPosition;
    [SerializeField]
    [Tooltip("The ID for the special event when an explosion has occured.")]
    private string _areaCheckedID;

    protected override void effectSetting_OnSpellCast()
    {
        base.effectSetting_OnSpellCast();
        if (_setToTargetPosition)
        {
            effectSetting.transform.position = effectSetting.spell.SpellTargetPosition.Value;
        }
    }

    protected override void Update()
    {
        base.Update();
        // Follow the caster if the option is checked.
        if (_followCaster)
            effectSetting.transform.position = effectSetting.spell.CastingEntity.transform.position;
    }

    protected override void DoEventTriggered(SpellEffectStandardEventArgs args)
    {
        base.DoEventTriggered(args);
        CheckForTargets();
    }

    /// <summary>
    /// On Spell update check for targets around the spell and try to trigger a collision. 
    /// A special area trigger event will also be triggered.
    /// </summary>
    private void CheckForTargets()
    {
        Collider[] colls = Physics.OverlapSphere(effectSetting.transform.position, radius, 1 << 9);
        foreach (Collider c in colls)
        {
            if (c.gameObject != effectSetting.spell.CastingEntity.gameObject)
            {
                TryTriggerCollision(new ColliderEventArgs(), c);
            }
        }
        // Trigger a motor event as per the area check
        effectSetting.TriggerSpecialEvent(_areaCheckedID);
    }

    protected override void effectSetting_OnSpellCollision(ColliderEventArgs args, Collider obj)
    {
        base.effectSetting_OnSpellCollision(args, obj);
        if (obj.gameObject.layer == LayerMask.NameToLayer("Entity"))
        {
            Entity e = obj.gameObject.GetComponent<Entity>();
            if (e != null)
                effectSetting.TriggerApplySpell(e);
        }
    }

    protected override void Reset()
    {
        TriggerEvent = SpellEffectTriggerEvent.Timed;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.parent.position, radius);
    }
}
