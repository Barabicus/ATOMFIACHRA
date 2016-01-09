using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// Handles the adding and removing of stat modifiers to the Enity. The applied stats will remain as long as the spell persists.
/// </summary>
[SpellCategory("Stat Modifier", SpellEffectCategory.Entity)]
[SpellEffectStandard(true, false, "Applies stat modifiers to the target Entity. The stat modifiers will be removed on spell destroy")]
public class AddStatModifier : SpellEffectStandard
{

    [SerializeField]
    /// <summary>
    /// How much each apply will increment the entity stat
    /// </summary>
    private EntityStats _statModifier;
    /// <summary>
    /// The entity the stats are applied to. Keep this value in case
    /// </summary>
    private List<Entity> _affectedEntities;

    public EntityStats StatModifier
    {
        get { return _statModifier; }
        set { _statModifier = value; }
    }

    public override void Initialize(EffectSetting effectSetting)
    {
        base.Initialize(effectSetting);
        _affectedEntities = new List<Entity>();
    }

    protected override void OnSpellStart()
    {
        base.OnSpellStart();
        _affectedEntities.Clear();
    }

    protected override void DoEventTriggered(SpellEffectStandardEventArgs args)
    {
        base.DoEventTriggered(args);

        if (args.TargetEntity != null)
        {
            _affectedEntities.Add(args.TargetEntity);
        }
        else
        {
            return;
        }
        args.TargetEntity.StatHandler.ApplyStatModifier(effectSetting.spell.SpellID, _statModifier);
    }

    protected override void effectSetting_OnSpellDestroy()
    {
        base.effectSetting_OnSpellDestroy();
        foreach (var ent in _affectedEntities)
        {
            ent.StatHandler.RemoveAllStatModifiers(effectSetting.spell.SpellID);
        }
    }

    private void Reset()
    {
        TriggerEvent = SpellEffectTriggerEvent.SpellApply;
        EntityTargetMethod = EntityTarget.ApplyEntity;
    }

}
