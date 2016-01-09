using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Spell Game/Basic")]
public class GetEntitySpeed : EntityAction
{
    public SharedFloat storeValue;

    public override void OnStart()
    {
        storeValue.Value = Entity.StatHandler.Speed;
    }
}
