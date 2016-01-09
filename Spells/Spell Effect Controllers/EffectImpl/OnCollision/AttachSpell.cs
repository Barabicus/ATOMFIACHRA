using UnityEngine;
using System.Collections;
[SpellCategory("Attach Spell", SpellEffectCategory.Spell)]
[SpellEffectStandard(true, false, "Attaches an attach type spell to the target Entity.")]
public class AttachSpell : SpellEffectStandard
{
    public Spell[] attachSpells;

    protected override void DoEventTriggered(SpellEffectStandardEventArgs args)
    {
        base.DoEventTriggered(args);

        if (args.TargetEntity != null)
        {
            foreach (Spell atch in attachSpells)
            {
                if (atch.SpellType != SpellType.Attached)
                {
                    Debug.LogError("Trying to attach spell: " + atch.SpellID + " which is of type: " + atch.SpellType);
                    continue;
                }
                if (!args.TargetEntity.HasSingleInstanceSpell(atch))
                {
                    Spell sp = SpellList.Instance.GetNewSpell(atch);
                    // Set the spell properties. The caster should be whoever cast the initial spell, but the spell
                    // target will be that of the target entity.
                    sp.CastSpell(effectSetting.spell.CastingEntity, args.TargetEntity.transform, null, args.TargetEntity.transform);
                    // Actually attach the spell to the target entity.
                    args.TargetEntity.AttachSpell(sp);
                    args.TargetEntity.AddSingleInstanceSpell(sp);
                }
                else
                {
                    args.TargetEntity.RefreshSingleInstanceSpell(atch);
                }
            }
        }
    }

    private void Reset()
    {
        TriggerEvent = SpellEffectTriggerEvent.SpellApply;
    }

}
