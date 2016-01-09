using System;
using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Action = BehaviorDesigner.Runtime.Tasks.Action;
using Random = UnityEngine.Random;

[TaskDescription("Wander using the Unity NavMesh.")]
[TaskCategory("Spell Game/Movement")]
[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}WanderIcon.png")]
public class WanderAroundPoint : EntityAction
{
    [UnityEngine.Tooltip("How far ahead of the current position to look ahead for a wander")]
    public SharedFloat wanderDistance = 20f;
    [UnityEngine.Tooltip("If the wander method is timed this is how long it will take to choose a new point")]
    public SharedFloat chooseNewPointTime = 2f;
    [UnityEngine.Tooltip("The pivot point that the entity will be wandering around")]
    public SharedTransform pivotPoint;
    public SharedFloat arriveDistance;

    public WanderAroundPointMethod wanderMethod;
    // A cache of the NavMeshAgent
    private Timer _chooseNewPointTimer;
    private Vector3 _pivotVector;

    public enum WanderAroundPointMethod
    {
        Timed,
        OnReached
    }

    public override void OnAwake()
    {
        base.OnAwake();
        // cache for quick lookup
        //   navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        _pivotVector = GetPivotPoint();
    }

    private Vector3 GetPivotPoint()
    {
        Vector3 v;
        if (pivotPoint == null || pivotPoint.Value == null)
        {
            v = Owner.gameObject.transform.position;
        }
        else
        {
            v = pivotPoint.Value.position;
        }
        return v;
    }

    public override void OnStart()
    {
        base.OnStart();
        _chooseNewPointTimer = new Timer(chooseNewPointTime.Value);
        var target = Target();
        if (Physics.Raycast(target + new Vector3(0, 200, 0), Vector3.down, 1000f, 1 << LayerMask.NameToLayer("Ground")))
        {
            Entity.EntityPathFinder.SetDestination(target);
        }
    }

    // There is no success or fail state with wander - the agent will just keep wandering
    public override TaskStatus OnUpdate()
    {
        base.OnUpdate();

        switch (wanderMethod)
        {
            case WanderAroundPointMethod.Timed:
                if (_chooseNewPointTimer.CanTick)
                {
                    return TaskStatus.Success;
                }
                break;
            case WanderAroundPointMethod.OnReached:
                if (Entity.EntityPathFinder.TargetReached)
                {
                    return TaskStatus.Success;
                }
                break;
        }

        return TaskStatus.Running;
    }

    // Return targetPosition if targetTransform is null
    private Vector3 Target()
    {
        return _pivotVector + Random.onUnitSphere * wanderDistance.Value;
    }

    // Reset the public variables
    public override void OnReset()
    {
        wanderDistance = 20;
        chooseNewPointTime = 2;
    }

    public override void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        var point = GetPivotPoint();
        Gizmos.DrawWireSphere(point, wanderDistance.Value);
    }
}
