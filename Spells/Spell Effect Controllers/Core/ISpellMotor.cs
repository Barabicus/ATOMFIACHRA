using UnityEngine;
using System.Collections;

public interface ISpellMotor
{
    MotorEntityTriggerState EntityTriggerState
    {
        get;
    }
    MotorTargetEntityMethod TargetEntityMethod { get; }
    void TryTriggerCollision(ColliderEventArgs args, Collider c);
}

public enum MotorEntityTriggerState
{
    Always,
    Living,
    Dead,
}
public enum MotorTargetEntityMethod
{
    Enemy,
    Friendly,
    Both
}