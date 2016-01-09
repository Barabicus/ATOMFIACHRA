using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// The spell motor is class that should be used by spell motors to trigger collisions with objects. By attempting to trigger a collision
/// it will work out if it should or shouldn't collider with a specific Entity depending on various factors such as it's living state or it's faction flags.
/// </summary>
public abstract class SpellMotor : SpellEffectStandard, ISpellMotor
{

    [SerializeField]
    [Tooltip("The spell will only be applied to the Entity if the Entity's living state is set to this")]
    private MotorEntityTriggerState _entityTriggerState = MotorEntityTriggerState.Living;
    [SerializeField]
    private MotorTargetEntityMethod _targetEntityMethod = MotorTargetEntityMethod.Enemy;

    public MotorEntityTriggerState EntityTriggerState
    {
        get { return _entityTriggerState; }
    }

    public MotorTargetEntityMethod TargetEntityMethod
    {
        get
        {
            return _targetEntityMethod;
        }
    }

    public void TryTriggerCollision(ColliderEventArgs args, Collider c)
    {
        if (c.gameObject.layer == LayerMask.NameToLayer("MoodBox"))
            return;

        if (c.gameObject.layer == LayerMask.NameToLayer("Entity"))
        {
            Entity e = c.gameObject.GetComponent<Entity>();

            if (e == null || effectSetting.spell.IgnoreEntities.Contains(e))
                return;

            switch (TargetEntityMethod)
            {
                case MotorTargetEntityMethod.Enemy:
                    if (!effectSetting.spell.CastingEntity.IsEnemy(e))
                    {
                        return;
                    }
                    break;
                case MotorTargetEntityMethod.Friendly:
                    if (effectSetting.spell.CastingEntity.IsEnemy(e))
                    {
                        return;
                    }
                    break;
            }

            switch (EntityTriggerState)
            {
                case MotorEntityTriggerState.Dead:
                    if (e.LivingState != EntityLivingState.Dead)
                        return;
                    break;
                case MotorEntityTriggerState.Living:
                    if (e.LivingState != EntityLivingState.Alive)
                        return;
                    break;
            }
        }

        effectSetting.TriggerCollision(args, c);
    }

    protected virtual void Reset()
    {
        // Set all motor events to default to spell update
        TriggerEvent = SpellEffectTriggerEvent.SpellUpdate;
    }
}
