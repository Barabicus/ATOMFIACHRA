using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RMCharacterUnit : MonoBehaviour
{
    /// <summary>
    /// The game data associated for this character. This should be set when the chracter list is being built.
    /// </summary>
    public GameData GameData { get; set; }

    /// <summary>
    /// Sets the game data in the game data manager when this unit has been selected.
    /// </summary>
    public void SetGameData()
    {
        GameDataManager.GameData = GameData;
    }

    public void TryDeleteCharacter()
    {
        UIWindow.GetWindowByCustomID(1).GetComponent<RMDeleteCharacter>().TryDeleteCharacter(GameData);
    }
}
