using UnityEngine;
using System.Collections;

[SpellCategory("Spawn Spell", SpellEffectCategory.Spell)]
public class SpawnSpell : SpellEffect
{
    public Spell spawnSpell;

    protected override void OnSpellStart()
    {
        base.OnSpellStart();
        Spell sp = SpellList.Instance.GetNewSpell(spawnSpell);
        sp.CastSpell(effectSetting.spell.CastingEntity, transform, transform.position, effectSetting.spell.SpellTargetTransform, effectSetting.spell.SpellTargetPosition);
        sp.transform.rotation = transform.rotation;
        enabled = false;
    }

}
