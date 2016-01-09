using System;
using UnityEngine;
using System.Collections;


/// <summary>
/// Applies the elemental stats to the target entity's health
/// </summary>
[SpellCategory("Elemental Apply", SpellEffectCategory.Entity)]
[SpellEffectStandard(true, false, "Apply Elemental power to the target Entity. Whether or not it heals or damages the Entity is based on the Entity's modifers and the value passed in.")]
public class ElementalApply : SpellEffectStandard
{
    [SerializeField]
    private ElementalStats elementalPower = ElementalStats.Zero;

    public ElementalStats ElementalPower
    {
        get { return elementalPower; }
        set { elementalPower = value; }
    }

    protected override void DoEventTriggered(SpellEffectStandardEventArgs args)
    {
        base.DoEventTriggered(args);

        if(args.TargetEntity != null)
        {
            if (args.TargetEntity.LivingState != EntityLivingState.Alive)
                return;

            // Apply the elemental properties
            args.TargetEntity.ApplyElementalSpell(this);
        }
        else
        {
            Debug.LogWarning("Effect: " + gameObject + " on spell: " + effectSetting.spell + " tried applying to null entity");
        }
    }

    private void Reset()
    {
        TriggerEvent = SpellEffectTriggerEvent.SpellApply;
        EntityTargetMethod = EntityTarget.ApplyEntity;
    }

}
