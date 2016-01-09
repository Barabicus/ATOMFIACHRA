using UnityEngine;
using System.Collections;
using ParticlePlayground;

public class ParticlePlaygroundQuestComponent : StandardQuestComponent
{
    [SerializeField]
    private PlaygroundEvent playgroundEvent = PlaygroundEvent.Stop;
    [SerializeField]
    private int emitAmountEvent = 500;

    private PlaygroundParticlesC playground;

    public enum PlaygroundEvent
    {
        Emit,
        Start,
        Stop
    }

    protected override void OnQuestAdded()
    {
        base.OnQuestAdded();
        playground = GetComponent<PlaygroundParticlesC>();
    }

    protected override void DoEventTriggered(QuestComponentStandardEventAgrs args)
    {
        base.DoEventTriggered(args);
        switch (playgroundEvent)
        {
            case PlaygroundEvent.Stop:
                playground.emit = false;
                break;
            case PlaygroundEvent.Start:
                playground.emit = true;
                break;
            case PlaygroundEvent.Emit:
                playground.Emit(emitAmountEvent);
                break;
        }
    }

}
