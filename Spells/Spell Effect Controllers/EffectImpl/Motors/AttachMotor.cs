using UnityEngine;
using System.Collections;

/// <summary>
/// Constantly causes the spell to trigger an apply spell update after the specified amount of time has passed.
/// The attach motor assumes it is operating on a spell that is of type Attach. When an entity casts an attach spell
/// they will automatically set themselves at the spellTarget. using this we get them as the target entity and apply
/// the spell to that entity.
/// </summary>
[SpellCategory("Attach Motor", SpellEffectCategory.Motor)]
[SpellEffectStandard(false, false, "The Attach motor is setup to trigger an apply spell on the target Entity. The spell will stay attached to the targeted entity so any visual effects will follow it. The spells can be attached to entities by means of other spells.")]
public class AttachMotor : SpellMotor
{
    private Entity targetEntity;

    protected override void OnSpellStart()
    {
        base.OnSpellStart();
        if(effectSetting.spell.SpellType != SpellType.Attached)
        {
            Debug.LogError("Spell: " + effectSetting.spell + " has an attach motor but is not classified as an Attach Spell");
        }
        targetEntity = effectSetting.spell.SpellTargetTransform.GetComponent<Entity>();
        if (targetEntity == null)
        {
            Debug.LogError("Target entity for " + name + " was not an entity. Parent: " + transform.parent.name);
            Destroy(gameObject);
            return;
        }
    }

    protected override void DoEventTriggered(SpellEffectStandardEventArgs args)
    {
        base.DoEventTriggered(args);
        DoApply();
    }

    protected override void Update()
    {
        base.Update();

        // Check if the current state of the Entity. This is done outside of update spell to ensure it is checked every frame
        // Rather than a specified tick amount of time.
        // If the Enity is null or if the Enity is not alive, destroy the spell. This ensures it wont persist into death.
        // Also destroy the spell if we go through a cinematic mode. This is to prevent the Entity dying or any other undesired effects
        if (targetEntity != null && targetEntity.LivingState != EntityLivingState.Alive || GameMainController.Instance.IsCinematic)
        {
            effectSetting.TriggerDestroySpell();
        }
    }

    private void DoApply()
    {
        if (targetEntity != null)
        {
            effectSetting.TriggerApplySpell(targetEntity);
        }
    }

}
