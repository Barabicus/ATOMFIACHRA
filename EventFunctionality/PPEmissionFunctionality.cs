using UnityEngine;
using System.Collections;
/// <summary>
/// Functionality Library for handling Particle Playground Emission Events
/// </summary>
public static class PPEmissionFunctionality
{
    public static void HandleEmissionEvent(ParticlePlayground.PlaygroundParticlesC particles, ParticlePlaygroundEmissionEvent emissionEvent)
    {
        switch (emissionEvent)
        {
            case ParticlePlaygroundEmissionEvent.Play:
                particles.emit = true;
                break;
            case ParticlePlaygroundEmissionEvent.Stop:
                particles.emit = false;
                break;
        }
    }
}

public enum ParticlePlaygroundEmissionEvent
{
    Play,
    Stop
}
