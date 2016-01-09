using UnityEngine;
using System.Collections;

public class SaveGame : MonoBehaviour
{
    public void TriggerSaveGame()
    {
        GameDataManager.Instance.UpdateGameData();
        SaveGameUtility.SaveToDisk(GameDataManager.GameData);
    }
}
