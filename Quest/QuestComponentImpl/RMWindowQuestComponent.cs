using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[QuestCategory("RM Window", QuestCategory.UI)]
public class RMWindowQuestComponent : StandardQuestComponent
{
    [SerializeField]
    private UIWindowID _windowID;
    [SerializeField]
    private int _customID;
    [SerializeField]
    private UIWindow.VisualState _targetState;

    protected override void DoEventTriggered(QuestComponentStandardEventAgrs args)
    {
        base.DoEventTriggered(args);
        UIWindow window = null;
        if (_windowID == UIWindowID.Custom)
        {
            window = UIWindow.GetWindowByCustomID(_customID);
        }
        else
        {
            window = UIWindow.GetWindow(_windowID);
        }

        switch (_targetState)
        {
            case UIWindow.VisualState.Shown:
                window.Show();
                break;
            case UIWindow.VisualState.Hidden:
                window.Hide();
                break;
        }
    }
}
