using UnityEngine;
using System.Collections;
using ParticlePlayground;
public class ParticlePlaygroundBarrierComponent : BarrierComponent
{

    [SerializeField]
    private PlaygroundEvent activedEvent = PlaygroundEvent.Start;
    [SerializeField]
    private PlaygroundEvent deactivatedEvent = PlaygroundEvent.Stop;

    private PlaygroundParticlesC playground;

    public enum PlaygroundEvent
    {
        Start,
        Stop
    }

    protected override void OnInitialise()
    {
        base.OnInitialise();
        playground = GetComponent<PlaygroundParticlesC>();
    }

    protected override void OnActiveStateChange(bool value)
    {
        base.OnActiveStateChange(value);
        switch (value ? activedEvent : deactivatedEvent)
        {
            case PlaygroundEvent.Stop:
                playground.emit = false;
                break;
            case PlaygroundEvent.Start:
                playground.emit = true;
                break;
        }
    }
}
