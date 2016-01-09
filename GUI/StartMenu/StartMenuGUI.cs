using UnityEngine;
using System.Collections;

public class StartMenuGUI : MonoBehaviour {

    public void LoadLevel(string level)
    {
        GUILoad.LoadLevel(level);
    }

    public void NewGame(string name)
    {
        SaveGameUtility.CreateNewSave(name);
    }

    public void Quit()
    {
        Application.Quit();
    }

}
