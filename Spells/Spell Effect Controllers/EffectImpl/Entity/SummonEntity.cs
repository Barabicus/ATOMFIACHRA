using UnityEngine;
using System.Collections;
[SpellCategory("Summon Entity", SpellEffectCategory.Entity)]
[SpellEffectStandard(false, true, "Summons the specified Entity at the target position using the specified transition fx")]
public class SummonEntity : SpellEffectStandard
{
    [SerializeField]
    private Entity _targetEntity;
    [SerializeField]
    private ObjectTransitionFx _transitionFx;

    protected override void DoEventTriggered(SpellEffectStandardEventArgs args)
    {
        base.DoEventTriggered(args);

        var ent = EntityPool.Instance.GetObjectFromPool(_targetEntity.PoolID, (e) =>
        {
            // Set new Entity to the level of the Entity that cast the spell
            e.LevelHandler.CurrentLevel = effectSetting.spell.CastingEntity.LevelHandler.CurrentLevel;
            e.transform.position = args.TargetPosition;
        });

        var fx = ObjectTransitionPool.Instance.GetObjectFromPool(_transitionFx.PoolID, (o) =>
        {
            o.FXTransitionMethod = ObjectTransitionFx.TransitionMethod.Activate;
            o.TargetObject = ent.gameObject;
        });
        fx.gameObject.SetActive(true);
    }

}
