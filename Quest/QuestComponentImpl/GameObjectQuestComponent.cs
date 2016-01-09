using UnityEngine;
using System.Collections;

public class GameObjectQuestComponent : QuestComponent
{
    [SerializeField]
    private float _activationTime = 1f;
    [SerializeField]
    private float _deactivationTime = 1f;

    private Timer _activationTimer;
    private Timer _deactivationTimer;
    private TransitionState _questState;

    private enum TransitionState
    {
        Started,
        Completed,
        Waiting
    }

    protected override void OnQuestAdded()
    {
        base.OnQuestAdded();
        _activationTimer = new Timer(_activationTime);
        _questState = TransitionState.Started;
        // Ensure all the transforms start off disabled
        foreach(Transform t in gameObject.transform)
        {
            t.gameObject.SetActive(false);
        }
    }

    protected override void OnQuestCompleted()
    {
        base.OnQuestCompleted();
        _questState = TransitionState.Completed;
        _deactivationTimer = new Timer(_deactivationTime);
    }

    protected override void OnQuestUpdate()
    {
        base.OnQuestUpdate();
        switch (_questState)
        {
            case TransitionState.Started:
                if (_activationTimer.CanTick)
                {
                    foreach (Transform t in gameObject.transform)
                    {
                        t.gameObject.SetActive(true);
                    }
                    _questState = TransitionState.Waiting;
                }
                break;
            case TransitionState.Completed:
                if (_deactivationTimer.CanTick)
                {
                    foreach (Transform t in gameObject.transform)
                    {
                        t.gameObject.SetActive(false);
                    }
                    _questState = TransitionState.Waiting;
                }
                break;
        }
    }

}
