using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Spell Game")]
public class WaitConditional : Conditional
{
    public SharedFloat waitTime;
    private Timer _timer;

    public override void OnAwake()
    {
        base.OnAwake();
        _timer = new Timer(waitTime.Value);
    }

    public override void OnStart()
    {
        base.OnStart();
        _timer.Reset();
    }

    public override TaskStatus OnUpdate()
    {
        if (_timer.CanTick)
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }

}
