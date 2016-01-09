using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RMDeleteCharacter : MonoBehaviour
{
    [SerializeField]
    private Text _descriptionText;

    private string _descriptionTextBaseCached;
    private GameData _targetDeleteGameData;

    private void Awake()
    {
        // save the description text so we can keep reformattig it.
        _descriptionTextBaseCached = _descriptionText.text;
    }

    /// <summary>
    /// Tries to delete the character associated with the object.
    /// The Gameobject passed in should have a RMCharacterUnit component attached to it.
    /// </summary>
    /// <param name="obj"></param>
    public void TryDeleteCharacter(GameData data)
    {
        _targetDeleteGameData = data;
        _descriptionText.text = string.Format(_descriptionTextBaseCached, _targetDeleteGameData.PlayerData.PlayerName);
        // Finally show the window
        UIWindow.GetWindowByCustomID(1).Show();
    }

    public void ConfirmDelete()
    {
        SaveGameUtility.DeleteSaveFile(_targetDeleteGameData.PlayerData.PlayerName);
        UIWindow.GetWindowByCustomID(1).Hide();
    }

}
