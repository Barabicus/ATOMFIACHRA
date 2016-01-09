using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SavedGameGUI : MonoBehaviour
{
    [SerializeField]
    private Text _name;
    [SerializeField]
    private Text _level;

    public GameData GameData { get; set; }

    private void Start()
    {
        _name.text = GameData.PlayerData.PlayerName;
        _level.text = GameData.level;
    }

    public void LoadLevel()
    {
        GameDataManager.GameData = GameData;
        GUILoad.LoadLevel(GameData.level);
    }

    public void Delete()
    {
        SaveGameUtility.DeleteSaveFile(GameData.PlayerData.PlayerName);
        Destroy(gameObject);
    }

}
