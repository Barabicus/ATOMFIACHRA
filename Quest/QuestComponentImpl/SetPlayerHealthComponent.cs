using UnityEngine;
using System.Collections;
[QuestCategory("Set Player Health", QuestCategory.Player)]
public class SetPlayerHealthComponent : StandardQuestComponent
{
    [SerializeField]
    private float _targetHealth;
    [SerializeField]
    [Tooltip("If true the target health will be set to the normal value. I.e. a percent ranging from 0 - 1")]
    private bool _isNormalisedValue;

    protected override void DoEventTriggered(QuestComponentStandardEventAgrs args)
    {
        base.DoEventTriggered(args);
        var player = GameMainReferences.Instance.PlayerCharacter.Entity;
        float hp = _isNormalisedValue ? player.StatHandler.MaxHp * _targetHealth : _targetHealth;
        player.CurrentHp = hp;
    }

}
