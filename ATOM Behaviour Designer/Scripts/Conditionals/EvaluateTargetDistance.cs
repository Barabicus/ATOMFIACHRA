using System;
using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Spell Game")]
public class EvaluateTargetDistance : EntityConditional
{
    public SharedTransform target;
    public SharedFloat minDistance;
    public SharedFloat maxDistance;
    public EvaluateMethod evaluateMethod = EvaluateMethod.Within;

    public enum EvaluateMethod
    {
        Within,
        Outside
    }

    public override TaskStatus OnUpdate()
    {
        float d = Vector3.Distance(target.Value.position, Entity.transform.position);
        switch (evaluateMethod)
        {
            case EvaluateMethod.Within:
                return minDistance.Value <= d && maxDistance.Value >= d ? TaskStatus.Success : TaskStatus.Failure;
            case EvaluateMethod.Outside:
                return minDistance.Value >= d || maxDistance.Value <= d ? TaskStatus.Success : TaskStatus.Failure;
        }
        return TaskStatus.Failure;
    }

}
