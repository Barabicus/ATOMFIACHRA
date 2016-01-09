using UnityEngine;
using System.Collections;

[SpellCategory("Physical Motor", SpellEffectCategory.Motor)]
public class PhysicalMotor : SpellMotor
{
    public float range = 2f;

    protected override void DoEventTriggered(SpellEffectStandardEventArgs args)
    {
        base.DoEventTriggered(args);
        if (Vector3.Distance(effectSetting.spell.SpellTargetTransform.position, effectSetting.spell.CastingEntity.transform.position) <= range)
        {
            TryTriggerCollision(new ColliderEventArgs(), effectSetting.spell.SpellTargetTransform.GetComponent<Collider>());
        }
    }

    protected override void effectSetting_OnSpellCollision(ColliderEventArgs args, Collider obj)
    {
        base.effectSetting_OnSpellCollision(args, obj);
        Entity e = effectSetting.spell.SpellTargetTransform.GetComponent<Entity>();
        if (e != null)
            effectSetting.TriggerApplySpell(e);
    }
}
