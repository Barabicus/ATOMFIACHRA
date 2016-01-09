using UnityEngine;
using System.Collections;

public class StandardGeneralEvent : GeneralEventComponent
{
    [SerializeField]
    private GeneralTriggerEvent _triggerEvent = GeneralTriggerEvent.Awake;
    [SerializeField]
    private float _tickTime = 1f;
    [SerializeField]
    private bool _oneShot = false;
    [SerializeField]
    private bool _tickOnStart;

    private Timer _tickTimer;
    private bool _hasTicked;

    public GeneralTriggerEvent TriggerEvent { get { return _triggerEvent; } }

    protected override void OnAwake()
    {
        base.OnAwake();
        if(TriggerEvent == GeneralTriggerEvent.Timed)
        {
            _tickTimer = _tickTime;
            if (_tickOnStart)
            {
                _tickTimer.ForceTickTime();
            }
        }
        if(TriggerEvent == GeneralTriggerEvent.Awake)
        {
            DoEventTriggered(new GeneralEventArgs(GeneralTriggerEvent.Awake));
        }
    }
    protected override void OnStart()
    {
        base.OnStart();
        if (TriggerEvent == GeneralTriggerEvent.Start)
        {
            DoEventTriggered(new GeneralEventArgs(GeneralTriggerEvent.Start));
        }
    }
    protected override void OnPlayerEnter()
    {
        base.OnPlayerEnter();
        if (TriggerEvent == GeneralTriggerEvent.PlayerEnter)
        {
            DoEventTriggered(new GeneralEventArgs(GeneralTriggerEvent.PlayerEnter));
        }
    }
    protected override void OnPlayerExit()
    {
        base.OnPlayerExit();
        if (TriggerEvent == GeneralTriggerEvent.PlayerExit)
        {
            DoEventTriggered(new GeneralEventArgs(GeneralTriggerEvent.PlayerExit));
        }
    }
    protected override void OnUpdate()
    {
        base.OnUpdate();
        if(_triggerEvent == GeneralTriggerEvent.Update)
        {
            DoEventTriggered(new GeneralEventArgs(GeneralTriggerEvent.Update));
        }
        if(_triggerEvent == GeneralTriggerEvent.Timed && !(_oneShot && _hasTicked) && _tickTimer.CanTick)
        {
            if (_oneShot)
            {
                _hasTicked = true;
            }
            else
            {
                _tickTimer.Reset();
            }
            DoEventTriggered(new GeneralEventArgs(GeneralTriggerEvent.Timed));
        }
    }

    protected virtual void DoEventTriggered(GeneralEventArgs args) { }

}

public struct GeneralEventArgs
{
    public GeneralTriggerEvent TriggerEvent;

    public GeneralEventArgs(GeneralTriggerEvent triggerEvent)
    {
        this.TriggerEvent = triggerEvent;
    }
}

public enum GeneralTriggerEvent
{
    Awake,
    Start,
    PlayerEnter,
    PlayerExit,
    Update,
    Timed
}
