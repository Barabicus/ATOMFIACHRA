using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NewGameGUI : MonoBehaviour
{
    [SerializeField]
    private InputField _playerName;

    /// <summary>
    /// 0 is adventure, 1 is action pew pew
    /// </summary>
    public int GameMode { get; set; }
    /// <summary>
    /// Keys:
    /// fire
    /// </summary>
    public string Loadout { get; set; }

    private void Start()
    {
        // Set defaults
        GameMode = 0;
        Loadout = "destro";
    }

    public void CreateNewGame()
    {
        var gameData = SaveGameUtility.CreateNewSave(_playerName.text);
        gameData.PlayerData.CostumeID = "player_elementalist";
        switch (GameMode)
        {
            case 0:
                gameData.level = "Birth";
                AdventureLoadout(gameData);
                break;
        }
        SaveGameUtility.SaveToDisk(gameData);
        // Load Game
        GameDataManager.GameData = gameData;
        GUILoad.LoadLevel(gameData.level);
    }

    private void AdventureLoadout(GameData data)
    {
        data.PlayerData.UnlockedElements[0] = Element.Fire;
        data.PlayerData.UnlockedElements[1] = Element.Water;
        data.PlayerData.UnlockedElements[2] = Element.Air;
        data.PlayerData.UnlockedElements[3] = Element.Earth;
    }

    private void UnlockSpells(Spell[] spells, GameData data)
    {
        data.PlayerData.UnlockedSpells = new string[spells.Length];
        for (int i = 0; i < spells.Length; i++)
        {
            if(spells[i] == null)
            {
                continue;
            }
            data.PlayerData.UnlockedSpells[i] = spells[i].SpellID;
            data.PlayerData.SpellToolbar[i] = spells[i].SpellID;
        }
    }

}
