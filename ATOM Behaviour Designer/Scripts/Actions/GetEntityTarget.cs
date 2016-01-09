using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Spell Game/Basic")]
public class GetEntityTarget : Action
{
    public SharedEntity entityTarget;
    public SharedTransform targetOutput;

    public override void OnStart()
    {
        base.OnStart();
        targetOutput.Value = entityTarget.Value.transform;
    }
}
