using UnityEngine;
using System.Collections;
using System;

public abstract class TimedUpdateableSpellMotor : TimedUpdateableEffect, ISpellMotor
{
    [SerializeField]
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
        // Don't trigger with a mood box
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

        effectSetting.TriggerCollision(new ColliderEventArgs(), c);
    }
}
