using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RMStartGame : MonoBehaviour {

    [SerializeField]
    private Button _button;

    public void StartGame()
    {
        if (GameDataManager.GameData != null)
        {
            GUILoad.LoadLevel(GameDataManager.GameData.level);
        }
    }

    private void Update()
    {
        if(GameDataManager.GameData == null)
        {
            _button.interactable = false;
        }
        else
        {
            _button.interactable = true;
        }
    }
}
