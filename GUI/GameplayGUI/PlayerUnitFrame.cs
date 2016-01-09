using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerUnitFrame : MonoBehaviour
{
    [SerializeField]
    private Text _playerName;
    [SerializeField]
    private Text _playerLevel;
    [SerializeField]
    private UIUnitFrame_Bar _healthBar;

    private Entity _player;
    private float _lastPlayerCurrentHealth;
    private float _lastPlayerMaxHealth;

    private void Start()
    {
        _player = GameMainReferences.Instance.PlayerCharacter.Entity;
        // Ensure health is set
        UpdateHealthVariables();
        UpdatePlayerHealthBar();
        UpdatePlayerLevel(_player.LevelHandler.CurrentLevel);
        UpdatePlayerName();

        GameMainReferences.Instance.PlayerCharacter.OnLevelUp += UpdatePlayerLevel;
    }

    private void Update()
    {
        if (HasPlayerHealthChanged())
        {
            UpdatePlayerHealthBar();
        }
    }

    private void UpdatePlayerHealthBar()
    {
        _healthBar.SetMaxValue((int)_lastPlayerMaxHealth);
        _healthBar.SetValue((int)_lastPlayerCurrentHealth);
    }

    private bool HasPlayerHealthChanged()
    {
        if(_lastPlayerCurrentHealth != _player.CurrentHp ||
            _lastPlayerMaxHealth != _player.StatHandler.MaxHp)
        {
            _lastPlayerCurrentHealth = _player.CurrentHp;
            _lastPlayerMaxHealth = _player.StatHandler.MaxHp;
            return true;
        }
        return false;
    }

    private void UpdateHealthVariables()
    {
        _lastPlayerCurrentHealth = _player.CurrentHp;
        _lastPlayerMaxHealth = _player.StatHandler.MaxHp;
    }

    private void UpdatePlayerLevel(int level)
    {
        _playerLevel.text = level.ToString();
    }

    private void UpdatePlayerName()
    {
        if (GameDataManager.GameData != null)
        {
            _playerName.text = GameDataManager.GameData.PlayerData.PlayerName;
        }
        else
        {
            _playerName.text = "Debug";
        }
    }

}
