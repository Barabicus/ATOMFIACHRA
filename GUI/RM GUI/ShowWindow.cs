using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShowWindow : MonoBehaviour
{
    [SerializeField]
    private UIWindowID _windowID;
    [SerializeField]
    [UnityEngine.Serialization.FormerlySerializedAs("_hideaAllOtherWindows")]
    private bool _hideAllOtherWindows;

    public void TriggerShowWindow()
    {
        if (_hideAllOtherWindows)
        {
            foreach (var window in UIWindow.GetWindows())
            {
                window.Hide();
            }
        }
        var targetWindow = UIWindow.GetWindow(_windowID);
        if(targetWindow == null)
        {
            Debug.LogErrorFormat("Window ID {0} could not be found", _windowID);
        }
        else
        {
            targetWindow.Show();
        }
    }

}
