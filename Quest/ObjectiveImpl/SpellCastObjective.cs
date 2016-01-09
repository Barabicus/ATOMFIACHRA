using UnityEngine;
using System.Collections;
[QuestCategory("Spell Cast", QuestCategory.Player)]
public class SpellCastObjective : QuestObjective {

    [SerializeField]
    private Spell _targetSpell;
    [SerializeField]
    private int _castAmount;

    private int _currentCastAmount;
    private Entity _player;

    public override string QuestDescription
    {
        get
        {
            return string.Format(base.QuestDescription, _currentCastAmount + "/ " + _castAmount);
        }
    }

    protected override void OnQuestInitialise()
    {
        base.OnQuestInitialise();
        _player = GameMainReferences.Instance.PlayerCharacter.Entity;
    }

    protected override void OnQuestAdded()
    {
        base.OnQuestAdded();
        _player.OnSpellCast += OnPlayerCastSpell;
    }

    protected override void OnQuestCompleted()
    {
        base.OnQuestCompleted();
        _player.OnSpellCast -= OnPlayerCastSpell;
    }

    private void OnPlayerCastSpell(Spell obj)
    {
        _currentCastAmount++;
        if(_currentCastAmount >= _castAmount)
        {
            TriggerObjectiveComplete();
            return;
        }
        // Update quest description listeners.
        TriggerDescriptionChanged();
    }
}
