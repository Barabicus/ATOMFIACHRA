using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Spell Game/Movement")]
public class KeepDistance : EntityAction
{
    public SharedTransform target;
    public SharedFloat keepDistance;
    public SharedBool keepLookAt = true;
    public RunMethod runMethod = RunMethod.Continious;
    public SharedFloat runTime;

    private Timer _runTimer;

    public enum RunMethod
    {
        Continious,
        Timed
    }

    public override void OnStart()
    {
        base.OnStart();
        _runTimer = runTime.Value;
    }

    public override TaskStatus OnUpdate()
    {
        if (target.Value == null)
        {
            return TaskStatus.Failure;
        }
        var direction = (Owner.transform.position - target.Value.position).normalized * keepDistance.Value;
        Entity.EntityPathFinder.SetDestination(target.Value.position + direction);

        Debug.DrawRay(target.Value.position, direction, Color.red);

        if (keepLookAt.Value && Vector3.Distance(target.Value.position, Owner.transform.position) <= keepDistance.Value + Entity.EntityPathFinder.StoppingDistance + 1f)
            Entity.LookAt(target.Value.position);
        if(runMethod == RunMethod.Timed && _runTimer.CanTick)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Running;
    }
}
