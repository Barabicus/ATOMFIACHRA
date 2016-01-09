using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Deployment.Internal;
using System.Linq;

public class LevelLoadManager : GameController
{
    private const string DEBUG_COSTUME_ID = "player_debug";

    public override void OnStart()
    {
        base.OnStart();
        LoadLevel();
    }

    private void LoadLevel()
    {
        LoadPlayer();
    }

    private void LoadPlayer()
    {
        // Create the player
        // If game data is null this means we have started a level directly without passing through the approriate menu level.
        // When the game is first loaded the GameDataManager is created. Between loads it will update the game data. If a game data
        // Does not exist it means it was not loaded properly. This could be because the scene was started directly i.e. in editor mode.
        // If this is the case we will substitue the costume as the debug costume.
        string costumeID = GameDataManager.GameData != null ? GameDataManager.GameData.PlayerData.CostumeID : DEBUG_COSTUME_ID;
        // In the case of a costume not being a debug costume, ensure the level meta info is not set to debug mode
        if (!costumeID.Equals(DEBUG_COSTUME_ID))
        {
           // LevelMetaInfo.Instance.IsDebugging = false;
        }

        PlayerController playerCont = EntityPool.Instance.GetObjectFromPool(costumeID).GetComponent<PlayerController>();

        if (playerCont == null)
        {
            Debug.LogError("Player Costume not found: " + costumeID);
            Debug.Break();
            return;
        }

        GameMainReferences.Instance.MiniMap.m_Target = playerCont.gameObject;
        GameMainReferences.Instance.PlayerController = playerCont;
        GameMainReferences.Instance.DepthOfField.focalTransform = playerCont.transform;
        // Get the player character from the main references once the player controller has been set
        PlayerCharacter playerCharacter = GameMainReferences.Instance.PlayerCharacter;

        // Set Player as camera follow target;
        GameMainReferences.Instance.RTSCamera.followTarget = playerCont.transform;

        // Spawn the player
        GameplayManager.Instance.RespawnPlayer();

        if (GameDataManager.GameData != null)
        {
            // Load player spell toolbar.
            // Even when a new game is first created this list will have been created. Typically this will be empty
            for (int i = 0; i < GameDataManager.GameData.PlayerData.SpellToolbar.Length; i++)
            {
                // By default when saving a spell to disk that is null it's ID will be set to null
                // No spell should have an ID of null. Check if the ID is null and if so continue
                // To the next spell.
                if (GameDataManager.GameData.PlayerData.SpellToolbar[i] == null)
                {
                    continue;
                }
                playerCharacter.SetSpellAtIndex(i,
                    SpellList.Instance.GetSpell(GameDataManager.GameData.PlayerData.SpellToolbar[i]));
            }
            // Load the unlocked spells list
            // Even when a new game is first created this list will have been created. Typically this will be empty
            List<Spell> spells = new List<Spell>();
            foreach (var unlockedSpell in GameDataManager.GameData.PlayerData.UnlockedSpells)
            {
                spells.Add(SpellList.Instance.GetSpell(unlockedSpell));
            }
            // Set the unlocked spells list
            playerCharacter.UnlockedSpells = spells;
        }
    }
}
