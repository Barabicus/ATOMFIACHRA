using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SpellItemGUI : MonoBehaviour {

    public Image iconImage;
    public Text spellTitle;
    public Text spellDescription;
    [SerializeField]
    private RectTransform _spellSubInfoContainer;
    [SerializeField]
    private Text _fireCost;
    [SerializeField]
    private Text _waterCost;
    [SerializeField]
    private Text _airCost;
    [SerializeField]
    private Text _earthCost;
    [SerializeField]
    private Text _deathCost;
    [SerializeField]
    private Text _arcaneCost;

    public Spell AssociatedSpell { get; set; }

    private void Start()
    {
        iconImage.sprite = AssociatedSpell.SpellIcon;
        spellTitle.text = AssociatedSpell.SpellName;
        spellDescription.text = AssociatedSpell.SpellDescription;
    }

    public void ToggleSubInfo()
    {
        _spellSubInfoContainer.gameObject.SetActive(!_spellSubInfoContainer.gameObject.activeSelf);
    }

}
