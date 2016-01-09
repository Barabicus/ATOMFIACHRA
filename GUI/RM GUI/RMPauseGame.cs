using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RMPauseGame : MonoBehaviour {

    public void OnTransitionChange(UIWindow window, UIWindow.VisualState state, bool instant)
    {
        if(state == UIWindow.VisualState.Shown)
        {
            // Pause on shown
            GameMainController.Instance.IsPaused = true;
        }
        else if(state == UIWindow.VisualState.Hidden)
        {
            GameMainController.Instance.IsPaused = false;
        }
    }

}
