using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Spell Game/Basic")]
public class EntityLookAt : EntityAction
{
    public SharedTransform target;

    public override TaskStatus OnUpdate()
    {
        Entity.LookAt(target.Value.position);
        return TaskStatus.Success;
    }
}
