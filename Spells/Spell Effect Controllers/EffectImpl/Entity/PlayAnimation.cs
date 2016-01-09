using UnityEngine;
using System.Collections;

[SpellCategory("Play Animation", SpellEffectCategory.Entity)]
[SpellEffectStandard(true, false, "Plays an animation on the target Entity")]
public class PlayAnimation : SpellEffectStandard
{
    [SerializeField]
    private EntityAnimation _animation;

    public EntityAnimation EntityAnimtion
    {
        get { return _animation; }
        set { _animation = value; }
    }

    protected override void DoEventTriggered(SpellEffectStandardEventArgs args)
    {
        if (args.TargetEntity != null)
        {
            if (args.TargetEntity.EntityAnimator != null)
                args.TargetEntity.EntityAnimator.PlayAnimation(_animation);
        }
    }

    public void Reset()
    {
        TriggerEvent = SpellEffectTriggerEvent.Cast;
        EntityTargetMethod = EntityTarget.CastingEntity;
    }

}


