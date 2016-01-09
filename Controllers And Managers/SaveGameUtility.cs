using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveGameUtility {

    /// <summary>
    /// This is the path where we will be storing our saved file.
    /// </summary>
    private static string SaveStatePath
    {
        get { return Application.persistentDataPath; }
    }

    public static GameData CreateNewSave(string characterName)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(GetFullPath(characterName));
        GameData data = new GameData();

        PlayerData pData = new PlayerData();
        pData.SpellToolbar = new string[12];
        pData.UnlockedSpells = new string[0];
        pData.PlayerName = characterName;
        pData.UnlockedElements = new Element[4];
        pData.PlayerLevel = 1;

        data.PlayerData = pData;

        bf.Serialize(file, data);

        // Ensure the file is closed
        file.Close();
        Debug.Log("New Game Created");
        return data;
    }

    public static void SaveToDisk(GameData data)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(GetFullPath(data.PlayerData.PlayerName));

        bf.Serialize(file, data);

        // Ensure the file is closed
        file.Close();

        Debug.Log("Game Saved");
    }

    public static void DeleteSaveFile(string characterName)
    {
        try
        {
            File.Delete(GetFullPath(characterName));
        }
        catch (IOException e)
        {
            Debug.LogError(e.Message);
        }
    }

    public static List<GameData> GetAllSaves()
    {
        List<GameData> data = new List<GameData>();
        string[] filePaths = Directory.GetFiles(@SaveStatePath, "*.sav");
        foreach (var filePath in filePaths)
        {
            data.Add(GetSaveData(filePath));
        }
        return data;
    }

    public static GameData GetSaveData(string filePath)
    {
        if (File.Exists(filePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(filePath, FileMode.Open);
            GameData data = (GameData)bf.Deserialize(file);
            // Ensure the file is closed
            file.Close();
            return data;
        }
        else
        {
            Debug.Log("File was not found");
            return null;
        }
    }

    public static string GetFullPath(string characterName)
    {
        return SaveStatePath + "/" + characterName + ".sav";
    }

}

[Serializable]
public class GameData
{
    public string level = "Echo";
    public PlayerData PlayerData { get; set; }
}

[Serializable]
public class PlayerData
{
    /// <summary>
    /// The character name associated with this save files
    /// </summary>
    public string PlayerName { get; set; }
    public string CostumeID { get; set; }
    /// <summary>
    /// The spells the player has selected on their toolbar in ID form
    /// </summary>
    public string[] SpellToolbar { get; set; }
    /// <summary>
    /// The spells the player has unlocked to use in ID form
    /// </summary>
    public string[] UnlockedSpells { get; set; }
    public Element[] UnlockedElements { get; set; }
    public int PlayerLevel { get; set; }
}