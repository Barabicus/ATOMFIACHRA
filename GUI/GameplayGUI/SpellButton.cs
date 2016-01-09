using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class SpellButton : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private int _spellIndex;
    [SerializeField]
    private Image _iconImage;
    private Spell _spell;
    private PlayerCharacter player;

    public event Action<SpellButton> OnPointerEnterButton;
    public event Action<SpellButton> OnPointerExitButton;

    public Spell Spell { get { return _spell; } }

    public void Start()
    {
        player = GameMainReferences.Instance.PlayerCharacter;
        _spell = player.GetSpellAtIndex(_spellIndex);
        ChangeSpellIcon(_spell);
        player.OnSpellChanged += OnSpellChanged;
    }

    void OnSpellChanged(int changedIndex, Spell changedSpell)
    {
        if (changedIndex == _spellIndex)
            UpdateButton(changedSpell);
    }

    public void OnDrop(PointerEventData data)
    {
        //  SpellMetaInfo dragSpell = data.pointerDrag.GetComponent<SpellMetaInfo>();
        var spellItem = data.pointerDrag.GetComponent<SpellItemGUI>();
        if (spellItem != null)
        {
            UpdateButton(spellItem.AssociatedSpell);
            player.SetSpellAtIndex(_spellIndex, _spell);
        }
    }

    private void UpdateButton(Spell spell)
    {
        ChangeSpellIcon(spell);
        this._spell = spell;
    }

    private void ChangeSpellIcon(Spell spell)
    {
        if (spell != null)
        {
            _iconImage.overrideSprite = spell.SpellIcon;
            _iconImage.gameObject.SetActive(true);
        }
        else
        {
            _iconImage.overrideSprite = null;
            _iconImage.gameObject.SetActive(false);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(OnPointerExitButton != null)
        {
            OnPointerExitButton(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(OnPointerEnterButton != null)
        {
            OnPointerEnterButton(this);
        }
    }
}
