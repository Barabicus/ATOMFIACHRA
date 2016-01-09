using UnityEngine;
using System.Collections;

[SpellCategory("Teleport", SpellEffectCategory.Entity)]
public class SpellTeleport : SpellEffectStandard
{

    protected override void DoEventTriggered(SpellEffectStandardEventArgs args)
    {
        if (effectSetting.spell.SpellTargetPosition.HasValue)
            effectSetting.spell.CastingEntity.EntityPathFinder.SetPosition(effectSetting.spell.SpellTargetPosition.Value);
    }
}
