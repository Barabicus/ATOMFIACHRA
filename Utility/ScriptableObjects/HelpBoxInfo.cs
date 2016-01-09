using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// The help box info is a collection of pages associated for helping the player in regards to a specific subject. 
/// For example it could show the player visual intructions and written instructions on how spells are cast and other 
/// areas of interest.
/// </summary>
public class HelpBoxInfo : ScriptableObject
{
    [SerializeField]
    private string _helpBoxID;
    [SerializeField]
    private string _helpBoxName;
    [SerializeField]
    private HelpInfoPage[] _pages;

    public string HelpBoxName { get { return _helpBoxName; } }
    public string HelpBoxID { get { return _helpBoxID; } }
    public HelpInfoPage[] Pages { get { return _pages; } }

}
[System.Serializable]
public struct HelpInfoPage
{
    [SerializeField]
    private string _pageText;
    [SerializeField]
    private Sprite _helpImage;

    public string PageText { get { return _pageText; } }
    public Sprite HelpImage { get { return _helpImage; } }
}
