using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Gets the Entity component on the gameobject.")]
[TaskCategory("Spell Game/Basic")]
public class GetEntity : Action
{
    public SharedEntity storeValue;

    public override void OnStart()
    {
        storeValue.Value = gameObject.GetComponent<Entity>();
    }
}
