using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameDataManager : GameController
{
    public static GameDataManager Instance { get; set; }

    public static GameData GameData { get; set; }

    public override void OnAwake()
    {
        base.OnAwake();
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Update player data into the game data
    public void UpdateGameData()
    {
        UpdatePlayerData();
        UpdateLevelData();
    }

    private void UpdateLevelData()
    {
        if (LevelMetaInfo.Instance.LevelName.Equals(LevelMetaInfo.DEFAULTNAME))
        {
            Debug.LogError("The level name was for scene level " + Application.loadedLevelName + " was not properly set. Ensure it is correctly set in the level meta info");
            return;
        }
        GameData.level = Application.loadedLevelName;
    }

    private void UpdatePlayerData()
    {
        var selectedSpells = new string[8];
        for (int i = 0; i < 8; i++)
        {
            var sp = GameMainReferences.Instance.PlayerCharacter.GetSpellAtIndex(i);
            if (sp != null)
            {
                selectedSpells[i] = sp.SpellID;
            }
            else
            {
                selectedSpells[i] = null;
            }
        }
        List<String> unlockedSpells = new List<string>();
        foreach (var sp in GameMainReferences.Instance.PlayerCharacter.UnlockedSpells)
        {
            unlockedSpells.Add(sp.SpellID);
        }
        GameData.PlayerData.UnlockedSpells = unlockedSpells.ToArray();
        GameData.PlayerData.UnlockedElements = GameMainReferences.Instance.PlayerController.PlayerUnlockedElements;
        GameData.PlayerData.SpellToolbar = selectedSpells;
        GameData.PlayerData.PlayerLevel = GameMainReferences.Instance.PlayerCharacter.Entity.LevelHandler.CurrentLevel;
    }
}
