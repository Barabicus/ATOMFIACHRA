using UnityEngine;
using System.Collections;
using ParticlePlayground;
[TransitionFxCategory("PPTransition Emission", TransitionFxCategory.Fx)]
public class PPTransitionEmission : ObjectTransitionStandard
{
    [SerializeField]
    private EventMethod _eventMethod;
    [SerializeField]
    [Tooltip("How much to emit when an emit event has been triggered")]
    private int emitAmount = 1;

    // pp cache
    private PlaygroundParticlesC _playground;
    /// <summary>
    ///  The initial emit value. This will be reset each time.
    /// </summary>
    private bool r_emit;

    public enum EventMethod
    {
        Start,
        Stop,
        Emit
    }

    protected override void OnInitialise()
    {
        base.OnInitialise();
        _playground = GetComponent<PlaygroundParticlesC>();
        r_emit = _playground.emit;
    }

    protected override void OnStart()
    {
        // Make sure we reset this first before calling start since it won't adhere to start events otherwise.
        _playground.emit = r_emit;
        base.OnStart();
    }

    protected override void OnEventTriggered(ObjectTransitionEventArgs args)
    {
        base.OnEventTriggered(args);
        switch (_eventMethod)
        {
            case EventMethod.Emit:
                _playground.Emit(emitAmount);
                break;
            case EventMethod.Start:
                _playground.emit = true;
                break;
            case EventMethod.Stop:
                _playground.emit = false;
                break;
        }
    }

}
