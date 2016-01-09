using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Wander using the Unity NavMesh.")]
[TaskCategory("Spell Game/Movement")]
[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}SeekIcon.png")]
public class EntitySeek : EntityAction
{
    public SharedTransform target;

    public override void OnStart()
    {
        base.OnStart();
        if (target.Value != null)
            Entity.EntityPathFinder.SetDestination(target.Value.position);
    }

    // Seek the destination. Return success once the agent has reached the destination.
    // Return running if the agent hasn't reached the destination yet
    public override TaskStatus OnUpdate()
    {
        base.OnUpdate();
        if (Entity.EntityPathFinder.TargetReached)
        {
            return TaskStatus.Success;
        }

        if (target.Value != null)
            Entity.EntityPathFinder.SetDestination(target.Value.position);
        else
        {
            return TaskStatus.Failure;
        }
        return TaskStatus.Running;
    }

}
