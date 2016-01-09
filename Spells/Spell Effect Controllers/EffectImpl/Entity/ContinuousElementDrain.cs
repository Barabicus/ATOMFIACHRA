using UnityEngine;
using System.Collections;

[SpellCategory("Continious Elemental Drain", SpellEffectCategory.Entity)]
public class ContinuousElementDrain : SpellEffectStandard
{
    public ElementalStats drainOnTick = ElementalStats.Zero;

    protected override void DoEventTriggered(SpellEffectStandardEventArgs args)
    {
        base.DoEventTriggered(args);
        effectSetting.spell.CastingEntity.SubtractElementCost(drainOnTick);
    }

}
