using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class RMLoadSavedGame : MonoBehaviour {

    [SerializeField]
    private UICharacterSelect_List _listComponent;

    private void OnEnable()
    {
        RebuildCharacterList();
    }

    public void RebuildCharacterList()
    {
        // Clear all current saves.
        if (_listComponent != null)
        {
            var oldunits = _listComponent.GetComponentsInChildren<RMCharacterUnit>(true);
            for(int i = oldunits.Length -1; i >= 0; i--)
            {
                Destroy(oldunits[i].gameObject);
            }
        }

        var saves = SaveGameUtility.GetAllSaves();
        var newList = new List<RMCharacterUnit>();
        foreach (var save in saves)
        {
            newList.Add(_listComponent.AddCharacter(save.PlayerData.PlayerName, save.PlayerData.CostumeID, "", save.PlayerData.PlayerLevel).GetComponent<RMCharacterUnit>());
        }

        for(int i = 0; i < newList.Count; i++)
        {
            newList[i].GameData = saves[i];
        }
    }

}
