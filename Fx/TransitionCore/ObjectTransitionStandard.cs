using UnityEngine;
using System.Collections;

/// <summary>
/// A standard way of handling events for use with Object Transitions and how it affects the component
/// </summary>
public class ObjectTransitionStandard : ObjectTransitionComponent
{
    [SerializeField]
    private ObjectTransitionEvent _triggerEvent;
    [SerializeField]
    private bool _singleShot;
    [SerializeField]
    private float _timeTrigger = 1f;

    private Timer _tickTimer;
    private bool _hasTicked;

    private bool r_singleShot;

    protected override void OnInitialise()
    {
        base.OnInitialise();
        _tickTimer = _timeTrigger;
        r_singleShot = _singleShot;
        _hasTicked = false;
    }

    protected override void OnStart()
    {
        base.OnStart();
        _singleShot = r_singleShot;
        _hasTicked = false;
        _tickTimer.Reset();
        if (_triggerEvent == ObjectTransitionEvent.Started)
        {
            OnEventTriggered(new ObjectTransitionEventArgs(ObjectTransitionEvent.Started));
        }
    }

    protected override void OnComplete()
    {
        base.OnComplete();
        if (_triggerEvent == ObjectTransitionEvent.Completed)
        {
            OnEventTriggered(new ObjectTransitionEventArgs(ObjectTransitionEvent.Completed));
        }
    }

    protected override void ComponentUpdate()
    {
        base.ComponentUpdate();
        if(!_hasTicked && _triggerEvent == ObjectTransitionEvent.Timed && _tickTimer.CanTickAndReset())
        {
            // prevent from reticking if it is a single shot
            if (_singleShot)
            {
                _hasTicked = true;
            }
            OnEventTriggered(new ObjectTransitionEventArgs(ObjectTransitionEvent.Timed));
        }
    }

    protected virtual void OnEventTriggered(ObjectTransitionEventArgs args) { }

}

public enum ObjectTransitionEvent
{
    Started,
    Completed,
    Timed,
    PrePool
}

public struct ObjectTransitionEventArgs
{
    public ObjectTransitionEvent EventArgs { get; set; }

    public ObjectTransitionEventArgs(ObjectTransitionEvent args)
    {
        EventArgs = args;
    }
}
