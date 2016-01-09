using UnityEngine;
using System.Collections;
using ParticlePlayground;

public class PPEmissionGeneralEvent : StandardGeneralEvent
{
    [SerializeField]
    private ParticlePlaygroundEmissionEvent _emissionEvent;

    private PlaygroundParticlesC _particles;

    protected override void OnAwake()
    {
        base.OnAwake();
        _particles = GetComponent<PlaygroundParticlesC>();
    }

    protected override void DoEventTriggered(GeneralEventArgs args)
    {
        base.DoEventTriggered(args);
        PPEmissionFunctionality.HandleEmissionEvent(_particles, _emissionEvent);
    }
}
