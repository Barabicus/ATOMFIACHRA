using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class HelpInfoWindow : MoveableGameWindow
{
    [SerializeField]
    private RectTransform _container;
    [SerializeField]
    private RectTransform _helpBoxContainer;
    [SerializeField]
    private GUIHelpPage _helpPagePrefab;
    [SerializeField]
    private Text _titleText;
    [SerializeField]
    private Text _currentPageText;
    [SerializeField]
    private Text _maxPageText;
    [SerializeField]
    private Button _prevButton;
    [SerializeField]
    private Button _nextButton;

    private Text _nextbuttonText;
    /// <summary>
    /// This is a collection of all help boxes available and stored using the specified ID.
    /// </summary>
    private Dictionary<string, HelpBoxInfo> _helpInfoCollection;
    private HelpBoxInfo _currentHelpBox;
    /// <summary>
    /// This is a collection of all the GUI elements associated with each individual page of the HelpBoxInfo.
    /// </summary>
    private List<GUIHelpPage> _loadedGUIPages;
    /// <summary>
    /// The index of the page we are currently on.
    /// </summary>
    private int _currentPageIndex;
    /// <summary>
    /// This is the Currently loaded help box. Use LoadHelpBox to load a new help box. Or use CloseHelpBox to close the current Help box.
    /// </summary>
    public HelpBoxInfo CurrentHelpBox
    {
        get
        {
            return _currentHelpBox;
        }
        private set
        {
            // Ensure all previous help pages are cleaned up
            CurrentGUIHelpPage = null;
            foreach (var page in _loadedGUIPages)
            {
                Destroy(page.gameObject);
            }
            _loadedGUIPages.Clear();

            if (value != null)
            {
                _titleText.text = value.HelpBoxName;
                // Load All pages
                foreach (var page in value.Pages)
                {
                    var helpPage = Instantiate(_helpPagePrefab) as GUIHelpPage;
                    helpPage.HelpInfoPage = page;
                    helpPage.transform.parent = _helpBoxContainer.transform;
                    helpPage.transform.localPosition = Vector3.zero;
                    helpPage.gameObject.SetActive(false);
                    _loadedGUIPages.Add(helpPage);
                }
                _maxPageText.text = _loadedGUIPages.Count.ToString();
                CurrentPageIndex = 0;
                _container.gameObject.SetActive(true);
            }
            else
            {
                _container.gameObject.SetActive(false);
            }
            _currentHelpBox = value;
        }
    }
    /// <summary>
    /// This is the current page on screen associated with the HelpBoxInfo.
    /// </summary>
    public GUIHelpPage CurrentGUIHelpPage
    {
        get; set;
    }
    public static HelpInfoWindow Instance { get; set; }

    public int CurrentPageIndex
    {
        get { return _currentPageIndex; }
        set
        {
            if (value >= 0 && value < _loadedGUIPages.Count)
            {
                // If we are on the last page disable the prev button
                if(value == 0)
                {
                    _prevButton.interactable = false;
                }
                else
                {
                    _prevButton.interactable = true;
                }

                // If we are on the last page changed next text to finished
                if(value == _loadedGUIPages.Count - 1)
                {
                    _nextbuttonText.text = "Finished";
                }
                else
                {
                    _nextbuttonText.text = "Next";
                }

                // Set the previous page to inactive
                if (CurrentGUIHelpPage != null)
                {
                    CurrentGUIHelpPage.gameObject.SetActive(false);
                }
                _loadedGUIPages[value].gameObject.SetActive(true);
                CurrentGUIHelpPage = _loadedGUIPages[value];
                _currentPageIndex = value;
                _currentPageText.text = (value + 1).ToString();
            }
            else if (value >= _loadedGUIPages.Count)
            {
                // If we are on the last page then the next button will have become the finish button
                // and if we are trying to go onto the next page from there, just close the window.
                CloseHelpBox();
            }
        }
    }
    protected override void Awake()
    {
        base.Awake();
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.Log("Instance already exists!");
            Destroy(gameObject);
            return;
        }
        _helpInfoCollection = Resources.LoadAll<HelpBoxInfo>("Prefabs/GUI/HelpInfo").ToDictionary(t => t.HelpBoxID, t => t);
        _loadedGUIPages = new List<GUIHelpPage>();
        _nextbuttonText = _nextButton.GetComponentsInChildren<Text>(true)[0];
    }

    protected override void Start()
    {
        base.Start();
        // Enusre the current help box is closed.
        CurrentHelpBox = null;
        GameMainController.Instance.OnCinematicChange += OnCinematicChange;
    }

    private void OnCinematicChange(bool obj)
    {
        CloseHelpBox();
    }

    /// <summary>
    /// Loads the specified help box. This will close the current helpbox on the screen if one exists.
    /// </summary>
    /// <param name="id"></param>
    public void DisplayHelpBox(string id)
    {
        if (!_helpInfoCollection.ContainsKey(id))
        {
            Debug.LogError("Help box info could not be found: " + id);
            return;
        }
        CurrentHelpBox = _helpInfoCollection[id];
    }

    public void CloseHelpBox()
    {
        if (CurrentHelpBox != null)
        {
            CurrentHelpBox = null;
        }
    }

    public void NextHelpPage()
    {
        CurrentPageIndex++;
    }

    public void PreviousHelpPage()
    {
        CurrentPageIndex--;
    }

}