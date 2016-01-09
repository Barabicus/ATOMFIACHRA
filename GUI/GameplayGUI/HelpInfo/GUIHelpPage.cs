using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUIHelpPage : MonoBehaviour
{
    [SerializeField]
    private Text _textArea;
    [SerializeField]
    private Image _helpImage;

    private HelpInfoPage _helpInfoPage;

    /// <summary>
    /// The page associated with the help box.
    /// </summary>
    public HelpInfoPage HelpInfoPage
    {
        get { return _helpInfoPage; }
        set
        {
            _helpInfoPage = value;
        }
    }

    private void Start()
    {
        _textArea.text = HelpInfoPage.PageText;
        _helpImage.overrideSprite = HelpInfoPage.HelpImage;
    }

}
