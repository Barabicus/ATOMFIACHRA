using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Spell Game")]
public class EvaluateEntityHealth : EntityConditional {

    [BehaviorDesigner.Runtime.Tasks.Tooltip("The minimum entity health in terms of 0 - 1")]
    public SharedFloat minHealth;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("The maximum entity health in terms of 0 - 1")]
    public SharedFloat maxHealth;


    public override TaskStatus OnUpdate()
    {
        float h = Entity.CurrentHealthNormalised;

        return minHealth.Value <= h && maxHealth.Value >= h ? TaskStatus.Success : TaskStatus.Failure;
    }
}
