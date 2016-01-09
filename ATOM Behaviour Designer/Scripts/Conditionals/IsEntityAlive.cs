using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

[TaskDescription("Check to see if the Entity is alive.")]
[TaskCategory("Spell Game")]
public class IsEntityAlive : EntityConditional
{
    public override TaskStatus OnUpdate()
    {
        if (Entity.LivingState == EntityLivingState.Alive)
            return TaskStatus.Success;
        else
            return TaskStatus.Failure; 
    }

}

