using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Spell Game/Basic")]
public class GetPivotPoint : Action
{
    public SharedTransform storeValue;

    public override void OnStart()
    {
        storeValue.Value = transform.FindChild("PivotPoint");
    }
}
