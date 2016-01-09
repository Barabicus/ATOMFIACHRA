using UnityEngine;
using System.Collections;
[QuestCategory("Player Spell Cast", QuestCategory.Player)]
public class PlayerSpellCastObjective : QuestObjective
{
    [SerializeField]
    private Spell _targetSpell;
    [SerializeField]
    private SpellCastMethod _spellCastMethod = SpellCastMethod.Amount;
    [SerializeField]
    private int _spellCastAmount;

    private Entity _player;
    private int _currentSpellsCast;

    public override string QuestDescription
    {
        get
        {
            switch (_spellCastMethod)
            {
                case SpellCastMethod.Amount:
                    return string.Format(base.QuestDescription, _currentSpellsCast + " / " + _spellCastAmount);
                default:
                    return base.QuestDescription;
            }
        }
    }

    public enum SpellCastMethod
    {
        Amount, // The target spell is cast this amount of time
        Timed
    }

    protected override void OnQuestInitialise()
    {
        base.OnQuestInitialise();
        _player = GameMainReferences.Instance.PlayerCharacter.Entity;
    }

    protected override void OnQuestAdded()
    {
        base.OnQuestAdded();
        _player.OnSpellCast += OnPlayerSpellCast;
    }

    protected override void ObjectiveCompleted()
    {
        base.ObjectiveCompleted();
        // Clean up
        _player.OnSpellCast -= OnPlayerSpellCast;
    }

    private void OnPlayerSpellCast(Spell spell)
    {
        switch (_spellCastMethod)
        {
            case SpellCastMethod.Amount:
                _currentSpellsCast++;
                if(_currentSpellsCast >= _spellCastAmount)
                {
                    TriggerObjectiveComplete();
                }
                break;
        }
    }
}
