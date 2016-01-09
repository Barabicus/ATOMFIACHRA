using UnityEngine;
using System.Collections;

public class LoadScene : MonoBehaviour
{
    [SerializeField]
    private string _sceneID;
    [SerializeField]
    private bool _instantLoad;

    public void TriggerLoadScene()
    {
        if (!_instantLoad)
        {
            GUILoad.LoadLevel(_sceneID);
        }
        else
        {
            Application.LoadLevel(_sceneID);
        }
    }

}
