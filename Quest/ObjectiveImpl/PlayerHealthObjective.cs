using UnityEngine;
using System.Collections;
[QuestCategory("Player Health", QuestCategory.Player)]
public class PlayerHealthObjective : QuestObjective
{
    [SerializeField]
    private HealthCheckMethod _healthCheckMethod;
    [SerializeField]
    private bool _isHealthNormalised;
    [SerializeField]
    private float _healthTarget;

    private Entity _player;

    public enum HealthCheckMethod
    {
        LessThanOrEqualTo,
        GreaterThanOrEqualTo
    }

    protected override void OnQuestInitialise()
    {
        base.OnQuestInitialise();
        _player = GameMainReferences.Instance.PlayerCharacter.Entity;
    }

    protected override void OnQuestAdded()
    {
        base.OnQuestAdded();
        DoCheck();
    }

    protected override void OnObjectiveUpdate()
    {
        base.OnObjectiveUpdate();
        DoCheck();
    }

    private void DoCheck()
    {
        float health = 0f;

        if (!_isHealthNormalised)
        {
            health = _healthTarget;
        }
        else
        {
            health = _player.StatHandler.MaxHp * _healthTarget;
        }

        switch (_healthCheckMethod)
        {
            case HealthCheckMethod.GreaterThanOrEqualTo:
                if (_player.CurrentHp >= health)
                {
                    TriggerObjectiveComplete();
                }
                break;
            case HealthCheckMethod.LessThanOrEqualTo:
                if (_player.CurrentHp <= health)
                {
                    TriggerObjectiveComplete();
                }
                break;
        }
    }

}
