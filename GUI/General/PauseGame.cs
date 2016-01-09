using UnityEngine;
using System.Collections;

public class PauseGame : MonoBehaviour {

	public void SetPause(bool value)
    {
        GameMainController.Instance.IsPaused = value;
    }
}
