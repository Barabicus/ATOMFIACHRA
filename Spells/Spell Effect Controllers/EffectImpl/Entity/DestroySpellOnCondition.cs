using UnityEngine;
using System.Collections;

[SpellCategory("Destroy Spell On Condition", SpellEffectCategory.Spell)]
public class DestroySpellOnCondition : SpellEffect
{
    public bool fireIsZero;
    public bool waterIsZero;
    public bool airIsZero;
    public bool earthIsZero;

    protected override void UpdateSpell()
    {
        base.UpdateSpell();
        if (fireIsZero && effectSetting.spell.CastingEntity.CurrentElementalCharge.fire == 0)
            DoTrigger();

        if (waterIsZero && effectSetting.spell.CastingEntity.CurrentElementalCharge.water == 0)
            DoTrigger();

        if (airIsZero && effectSetting.spell.CastingEntity.CurrentElementalCharge.air == 0)
            DoTrigger();

        if (earthIsZero && effectSetting.spell.CastingEntity.CurrentElementalCharge.earth == 0)
            DoTrigger();

    }

    private void DoTrigger()
    {
        effectSetting.TriggerDestroySpell();
    }

}
