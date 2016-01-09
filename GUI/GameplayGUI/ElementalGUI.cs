using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class ElementalGUI : MonoBehaviour
{
    [SerializeField]
    private ElementContentWrapper[] _elementContents;
    [SerializeField]
    private ElementVisualWrapper[] _elementalVisuals;

    //   private RectTransform[] _unlockedElementsTransforms;
    //   private Image[] _unlockedElementsImage;

    private Dictionary<Element, ElementVisualWrapper> _elementalVisualsDict;

    private PlayerController _playerControllerCache;

    private void Awake()
    {
        // Create visuals dictionairy for easy lookup
        _elementalVisualsDict = _elementalVisuals.ToDictionary(t => t.Element);
    }

    private void Start()
    {
        _playerControllerCache = GameMainReferences.Instance.PlayerController;
        _playerControllerCache.OnUnlockElementsChanged += OnUnlockElementsChanged;
        // Ensure the elemental gui is updated
        UpdateElementalGUI(_playerControllerCache.PlayerUnlockedElements);
    }

    private void Update()
    {
        UpdateElementCharge();
    }

    private void OnUnlockElementsChanged(Element[] elements)
    {
        UpdateElementalGUI(elements);
    }
    /// <summary>
    /// Sets the panel to display the proper elemental visuals.
    /// </summary>
    /// <param name="elements"></param>
    private void UpdateElementalGUI(Element[] elements)
    {
        // Disable all elements
        foreach (var t in _elementContents)
        {
            t.ElementTransform.gameObject.SetActive(false);
        }
        for (int i = 0; i < elements.Length; i++)
        {
            if (elements[i] == 0)
            {
                continue;
            }
            _elementContents[i].ElementTransform.gameObject.SetActive(true);
            _elementContents[i].ElementChargeImage.color = _elementalVisualsDict[elements[i]].ElementColor;
            _elementContents[i].ElementOutlineImage.color = _elementalVisualsDict[elements[i]].ElementOutlineColor;
        }
    }


    private void UpdateElementCharge()
    {
        for (int i = 0; i < _elementContents.Length; i++)
        {
            if (_playerControllerCache.PlayerUnlockedElements[i] == 0)
            {
                continue;
            }
            _elementContents[i].ElementChargeImage.fillAmount = MathUtility.GetPercent(_playerControllerCache.Entity.CurrentElementalCharge[_playerControllerCache.PlayerUnlockedElements[i]], _playerControllerCache.Entity.StatHandler.MaxElementalCharge[_playerControllerCache.PlayerUnlockedElements[i]]);

            _elementContents[i].ElementalText.text = Mathf.Floor(_playerControllerCache.Entity.CurrentElementalCharge[_playerControllerCache.PlayerUnlockedElements[i]]).ToString();
        }
    }
}
[System.Serializable]
public class ElementContentWrapper
{
    [SerializeField]
    private RectTransform _elementTransform;
    [SerializeField]
    private Image _elementChargeImage;
    [SerializeField]
    private Image _elementOutlineImage;
    [SerializeField]
    private Text _elementalText;

    public RectTransform ElementTransform { get { return _elementTransform; } }
    public Image ElementChargeImage { get { return _elementChargeImage; } }
    public Image ElementOutlineImage { get { return _elementOutlineImage; } }
    public Text ElementalText { get { return _elementalText; } }
}
[System.Serializable]
public class ElementVisualWrapper
{
    [SerializeField]
    private Element _element;
    [SerializeField]
    private Color _elementColor;
    [SerializeField]
    private Color _elementOutlineColor;

    public Element Element { get { return _element; } }
    public Color ElementColor { get { return _elementColor; } }
    public Color ElementOutlineColor { get { return _elementOutlineColor; } }
}