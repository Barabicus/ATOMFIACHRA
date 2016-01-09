using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Analytics;

/// <summary>
/// The player character class handles information regarding the player. This includes leveling up and experience as well as handling what spells the player currently has
/// unlocked or selected on their toolbar. 
/// </summary>
public class PlayerCharacter : EntityComponent
{
    private float _currentExperience;
    private PlayerGlobalInfo _playerGlobalInfo;
    private Spell[] _spellList;
    private List<Spell> _unlockedSpells;

    public const int SpellToolBarCount = 12;
    /// <summary>
    /// A list of the spells the player has unlocked. Use the UnlockSpell method to trigger a spell to unlock. Setting this to a new 
    /// list will override the spells list completely and this may not be desired unless loading from disk.
    /// </summary>
    public List<Spell> UnlockedSpells
    {
        get { return _unlockedSpells; }
        set { _unlockedSpells = value; }
    }

    #region Events
    /// <summary>
    /// An event that is fired when a spell on the toolbar has changed. This can be due to the player dragging a new spell in and replacing it.
    /// </summary>
    public event Action<int, Spell> OnSpellChanged;
    /// <summary>
    /// Fired when a spell has been unlocked
    /// </summary>
    public event Action<Spell> OnSpellUnlocked;
    /// <summary>
    /// Fired when the player levels up.
    /// </summary>
    public event Action<int> OnLevelUp;
    #endregion

    public override void Initialise()
    {
        base.Initialise();
        _unlockedSpells = new List<Spell>();
        _spellList = new Spell[SpellToolBarCount];
        _playerGlobalInfo = Resources.Load<PlayerGlobalInfo>(ConfigPathLocations.PLAYER_GLOBAL_INFO);
    }

    public float CurrentExperience
    {
        get { return _currentExperience; }
        set
        {
            _currentExperience = value;
            if (_currentExperience >= NextLevelExperience)
            {
                TriggerLevelUp();
                _currentExperience = 0f;
            }
        }
    }
    public float NextLevelExperience
    {
        get
        {
            return _playerGlobalInfo.GetNextLevelExperience(Entity);
        }
    }

    public override void OnStart()
    {
        base.OnStart();
        EntityManager.Instance.OnEntityKilled += OnEntityKilled;
    }

    protected override void Update()
    {
        base.Update();
        // Cheat
        if (Input.GetKeyDown(KeyCode.L))
        {
            TriggerLevelUp();
        }
    }

    private void OnEntityKilled(Entity entity)
    {
        // If the entity has been killed by the player make sure to register the experience reward.
        if (entity != Entity && entity.KilledBy != null && entity.KilledBy == Entity)
        {
            CurrentExperience += entity.LevelHandler.GetExperienceRewardForLevel(Entity.LevelHandler.CurrentLevel);
        }
        // Send Analytics events

    }
    /// <summary>
    /// Called when the player has been killed.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnKilled(Entity e)
    {
        base.OnKilled(e);
        string killedBy = "null";
        if (Entity.KilledBy != null)
        {
            killedBy = Entity.KilledBy.EntityName;
        }
        //Analytics.CustomEvent("PlayerKilled", new Dictionary<string, object>()
        //{
        //    { "level", Entity.LevelHandler.CurrentLevel },
        //    {"killedBy", killedBy }
        //});
    }

    private void TriggerLevelUp()
    {
        if (OnLevelUp != null)
        {
            OnLevelUp(Entity.LevelHandler.CurrentLevel);
        }
        Entity.LevelHandler.AdvanceLevel();
    }


    public void SetSpellAtIndex(int spellIndex, Spell spell)
    {
        if (spellIndex >= 0 && spellIndex < _spellList.Length)
        {
            _spellList[spellIndex] = spell;
            if (OnSpellChanged != null)
            {
                OnSpellChanged(spellIndex, spell);
            }
        }
    }

    public void SetSpellAtIndex(int spellIndex, string spellID)
    {
        SetSpellAtIndex(spellIndex, SpellList.Instance.GetSpell(spellID));
    }

    public Spell GetSpellAtIndex(int spellIndex)
    {
        if (spellIndex >= 0 && spellIndex < _spellList.Length)
        {
            return _spellList[spellIndex];
        }
        return null;
    }
    /// <summary>
    /// Unlocks a spell for the entity. Ensure the spell reference being passed in is a reference to the spell prefab rather than a new instance of the spell
    /// Use UnlockSpell and pass in the ID to ensure this is the case. 
    /// Return true if it has been unlocked, otherwise it returns false if it is already unlocked
    /// </summary>
    /// <param name="spell"></param>
    public bool UnlockSpell(Spell spell, bool quietUnlock = false)
    {
        if (!UnlockedSpells.Contains(spell))
        {
            UnlockedSpells.Add(spell);
            // If it is quiet unlock, unlock the spell without sending spell unlocked events
            if (!quietUnlock)
            {
                if (OnSpellUnlocked != null)
                    OnSpellUnlocked(spell);
                UIToast.Instance.DoSpellUnlocked(spell);
            }
            for (int i = 0; i < SpellToolBarCount; i++)
            {
                if (spell != null && GetSpellAtIndex(i) == null)
                {
                    SetSpellAtIndex(i, spell.SpellID);
                    break;
                }
            }
            return true;
        }
        return false;
    }
    /// <summary>
    /// Unlocks a spell for the entity. Ensure the spell reference being passed in is a reference to the spell prefab rather than a new instance of the spell
    /// Use UnlockSpell and pass in the ID to ensure this is the case. 
    /// Return true if it has been unlocked, otherwise it returns false if it is already unlocked
    /// </summary>
    /// <param name="spell"></param>
    public bool UnlockSpell(string spellID, bool quietUnlock = false)
    {
        return UnlockSpell(SpellList.Instance.GetSpell(spellID), quietUnlock);
    }
}
