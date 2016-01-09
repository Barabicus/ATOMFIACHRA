using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

[TaskCategory("Spell Game/Movement")]
public class AdjustDirectionToTarget : EntityAction
{
    public SharedTransform target;
    public SharedFloat desiredDistance;
    public SharedFloat directionValue;
    public SharedBool randomDirection;
    public SharedFloat minDirection;
    public SharedFloat maxDirection;
    public SharedBool keepLookAtTarget;
    public RunMethod runMethod = RunMethod.ReachedDestination;
    public SharedFloat runTime = 1f;

    private Vector3 _direction;
    private Timer _runTimer;

    public enum RunMethod
    {
        ReachedDestination,
        Timed
    }

    public Vector3 TargetPosition
    {
        get
        {
            return target.Value.position + _direction;
        }
    }

    public override void OnStart()
    {
        base.OnStart();
        if (randomDirection.Value)
        {
            directionValue.Value = Random.Range(minDirection.Value, maxDirection.Value);
        }
        _direction = Quaternion.Euler(new Vector3(0, directionValue.Value, 0)) * (Owner.transform.position - target.Value.position).normalized;
        _direction *= desiredDistance.Value;
        _runTimer = runTime.Value;
    }

    public override TaskStatus OnUpdate()
    {
        if (target.Value == null)
        {
            return TaskStatus.Failure;
        }
        Entity.EntityPathFinder.SetDestination(TargetPosition);
        if (keepLookAtTarget.Value)
        {
            Entity.LookAt(target.Value.position);
        }
        if (Entity.EntityPathFinder.TargetReached)
        {
            return TaskStatus.Success;
        }
        if(runMethod == RunMethod.Timed && _runTimer.CanTick)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Running;
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        if (target != null && target.Value != null)
        {
            Debug.DrawLine(target.Value.position, TargetPosition, Color.red);
            Debug.DrawLine(Owner.transform.position, TargetPosition, Color.blue);
        }
    }

}
