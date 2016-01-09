using System;
using UnityEngine;
using System.Collections;

[QuestCategory("Entity Speech", QuestCategory.Entity)]
public class EntitySpeechObjective : QuestObjective
{
    [SerializeField]
    private Entity _targetEntity;
    [SerializeField]
    private SpeechEvent _speechEvent;

    public enum SpeechEvent
    {
        FinishedTalking,
        FinishedPlaying
    }

    protected override void OnQuestAdded()
    {
        base.OnQuestAdded();
        switch (_speechEvent)
        {
            case SpeechEvent.FinishedTalking:
                _targetEntity.SpeechBubbleProxy.OnFinishedTalking += OnFinishedTalking;
                break;
            case SpeechEvent.FinishedPlaying:
                _targetEntity.SpeechBubbleProxy.OnFinishedPlaying += OnFinishedPlaying;
                break;
        }
    }

    protected override void ObjectiveCompleted()
    {
        base.ObjectiveCompleted();
        switch (_speechEvent)
        {
            case SpeechEvent.FinishedTalking:
                _targetEntity.SpeechBubbleProxy.OnFinishedTalking -= OnFinishedTalking;
                break;
            case SpeechEvent.FinishedPlaying:
                _targetEntity.SpeechBubbleProxy.OnFinishedPlaying -= OnFinishedPlaying;
                break;
        }

    }

    private void OnFinishedPlaying()
    {
        TriggerObjectiveComplete();
    }

    private void OnFinishedTalking()
    {
        TriggerObjectiveComplete();
    }
}
