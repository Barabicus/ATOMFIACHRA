using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class SpellOverDetailsGUI : MonoBehaviour
{
    [SerializeField]
    private Text _infoText;
    [SerializeField]
    private RectTransform _container;
    [SerializeField]
    private RectTransform _spellButtonContainer;
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

    private void Awake()
    {
        var buttons = _spellButtonContainer.GetComponentsInChildren<SpellButton>(true);
        foreach(var b in buttons)
        {
            b.OnPointerEnterButton += OnPointerEnterButton;
            b.OnPointerExitButton += OnPointerExitButton;
        }
    }

    private void OnPointerExitButton(SpellButton obj)
    {
        _container.gameObject.SetActive(false);
    }

    private void OnPointerEnterButton(SpellButton obj)
    {
        // Dont set if spell is null and ensure it is closed
        if(obj.Spell == null)
        {
            _container.gameObject.SetActive(false);
            return;
        }
        SetDetails(obj.Spell);
        _container.gameObject.SetActive(true);
    }
    /// <summary>
    /// Set all the spell details
    /// </summary>
    /// <param name="spell"></param>
    private void SetDetails(Spell spell)
    {
        if(spell == null)
        {
            return;
        }
        _infoText.text = spell.SpellDescription;
        _fireCost.text = spell.ElementalCost.fire.ToString();
        _waterCost.text = spell.ElementalCost.water.ToString();
        _airCost.text = spell.ElementalCost.air.ToString();
        _earthCost.text = spell.ElementalCost.earth.ToString();
        _deathCost.text = spell.ElementalCost.death.ToString();
        _arcaneCost.text = spell.ElementalCost.arcane.ToString();
    }
}
