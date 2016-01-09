using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class StandardHelpBoxInstruction : HelpBoxInstruction
{
    [SerializeField]
    private HelpMethod _helpMethod;
    [SerializeField]
    private int _mouseButton;
    [SerializeField]
    private KeyCode _keyCode;
    [SerializeField]
    [Tooltip("The time to hold the mouse button or key. 0 acts like a click")]
    private float _buttonHoldTime;
    [SerializeField]
    private Spell _checkSpell;
    [SerializeField]
    private int _spellCastAmount = 1;
    [SerializeField]
    private Entity _entityKilled;
    [SerializeField]
    private float _playerHealthNormalised;
    [SerializeField]
    private Quest _targetQuest;

    [SerializeField]
    private Text _infoText;

    private Entity _player;
    private int _currentSpellCastAmount;
    private Timer _buttonHeldTimer;

    public enum HelpMethod
    {
        MouseButtonHeld, // Mouse button held
        KeyHeld, // Key Held
        SpellCast, // When a player casts a spell
        PlayerHealthGreaterThanOrEqualTo, // When the players health is a certain value
        EntityKilled, // When a specific Entity has been killed
        CloseEvent, // Triggered by some event, like a button
        QuestCompleted
    }

    public string OutputText
    {
        get
        {
            switch (_helpMethod)
            {
                case HelpMethod.CloseEvent:
                    return "";
                case HelpMethod.EntityKilled:
                    return "";
                case HelpMethod.KeyHeld:
                    return Mathf.Ceil(_buttonHeldTimer.RemainingTickTime).ToString();
                case HelpMethod.MouseButtonHeld:
                    return Mathf.Ceil(_buttonHeldTimer.RemainingTickTime).ToString();
                case HelpMethod.PlayerHealthGreaterThanOrEqualTo:
                    return String.Format("{0} / {1}", Mathf.Floor(_player.CurrentHp), Mathf.Ceil(_player.StatHandler.MaxHp * _playerHealthNormalised));
                case HelpMethod.QuestCompleted:
                    return "";
                case HelpMethod.SpellCast:
                    return _currentSpellCastAmount + " / " + _spellCastAmount;
                default:
                    return "";
            }
        }
    }

    protected override void OnDisplay()
    {
        base.OnDisplay();
        _buttonHeldTimer = _buttonHoldTime;
        _player = GameMainReferences.Instance.PlayerController.Entity;
        switch (_helpMethod)
        {
            case HelpMethod.EntityKilled:
                _entityKilled.OnKilled += _entityKilled_OnKilled;
                    break;
            case HelpMethod.SpellCast:
                _player.OnSpellCast += _player_OnSpellCast;
                if(_infoText != null)
                {
                    _infoText.text = OutputText;
                }
                break;
            case HelpMethod.QuestCompleted:
                _targetQuest.OnQuestCompleted += TargetQuestCompleted;
                break;
        }
    }

    private void TargetQuestCompleted(Quest obj)
    {
        _targetQuest.OnQuestCompleted -= TargetQuestCompleted;
        IsHelpBoxCompleted = true;
    }

    protected override void CheckUpdate()
    {
        base.CheckUpdate();
        switch (_helpMethod)
        {
            case HelpMethod.KeyHeld:
                if (Input.GetKey(_keyCode))
                {
                    if (_buttonHeldTimer.CanTick)
                    {
                        IsHelpBoxCompleted = true;
                    }
                }
                else
                {
                    _buttonHeldTimer.Reset();
                }
                if(_infoText != null)
                {
                    _infoText.text = OutputText;
                }
                break;
            case HelpMethod.MouseButtonHeld:
                if (Input.GetMouseButton(_mouseButton))
                {
                    if (_buttonHeldTimer.CanTick)
                    {
                        IsHelpBoxCompleted = true;
                    }
                }
                else
                {
                    _buttonHeldTimer.Reset();
                }
                if (_infoText != null)
                {
                    _infoText.text = OutputText;
                }
                break;
            case HelpMethod.PlayerHealthGreaterThanOrEqualTo:
                if (_player.CurrentHealthNormalised >= _playerHealthNormalised) IsHelpBoxCompleted = true;
                if(_infoText != null)
                {
                    _infoText.text = OutputText;
                }
                break;              
        }
    }

    /// <summary>
    /// Only ever called if the Help Method is looking for a specific Entity to be killed.
    /// If so trigger a completion
    /// </summary>
    /// <param name="obj"></param>
    private void _entityKilled_OnKilled(Entity obj)
    {
        IsHelpBoxCompleted = true;
        // unsubscribe
        obj.OnKilled -= _entityKilled_OnKilled;
    }
    /// <summary>
    /// Only ever called if the Help Method is checking if the player cast the specific spell.
    /// Will Also take into consideration the amount of times the spell has been cast.
    /// </summary>
    /// <param name="spell"></param>
    private void _player_OnSpellCast(Spell spell)
    {
        if (spell.SpellID.Equals(_checkSpell.SpellID))
        {
            _currentSpellCastAmount++;
            if(_currentSpellCastAmount >= _spellCastAmount)
            {
                IsHelpBoxCompleted = true;
                _player.OnSpellCast -= _player_OnSpellCast;
            }
            if(_infoText != null)
            {
                _infoText.text = (_spellCastAmount - _currentSpellCastAmount).ToString();
            }
        }
    }
    /// <summary>
    /// Completes the help box. Only if the help method is CloseEvent
    /// </summary>
    public void TriggerCloseEvent()
    {
        if(_helpMethod == HelpMethod.CloseEvent)
        {
            IsHelpBoxCompleted = true;
        }
    }
}
