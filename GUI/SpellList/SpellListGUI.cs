using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

public class SpellListGUI : MonoBehaviour
{
    [SerializeField]
    private Toggle _fireFilter;
    [SerializeField]
    private Toggle _airFilter;
    [SerializeField]
    private Toggle _waterFilter;
    [SerializeField]
    private Toggle _earthFilter;
    [SerializeField]
    private Toggle _deathFilter;
    [SerializeField]
    private Toggle _arcaneFilter;

    public Transform spellListBody;
    public Transform spellItemPrefab;
    public int spellsPerPage = 5;
    public ScrollRect spellScroll;

    private List<Transform> _spellsInList = new List<Transform>();

    private SpellMetaInfoConstructed _filterInfo;

    private void Awake()
    {
        _filterInfo = new SpellMetaInfoConstructed();
    }

    private void Start()
    {
        GameMainReferences.Instance.PlayerCharacter.OnSpellUnlocked += OnSpellUnlocked;

        // Set Filters
        _fireFilter.onValueChanged.AddListener((v) =>
       {
           SetFilter(SpellFilters.Fire, v);
       });
        _airFilter.onValueChanged.AddListener((v) =>
        {
            SetFilter(SpellFilters.Air, v);
        });
        _waterFilter.onValueChanged.AddListener((v) =>
        {
            SetFilter(SpellFilters.Water, v);
        });
        _earthFilter.onValueChanged.AddListener((v) =>
        {
            SetFilter(SpellFilters.Earth, v);
        });
        _deathFilter.onValueChanged.AddListener((v) =>
        {
            SetFilter(SpellFilters.Death, v);
        });
        _arcaneFilter.onValueChanged.AddListener((v) =>
        {
            SetFilter(SpellFilters.Arcane, v);
        });
    }

    #region Spell Methods

    public void CreateSpellList()
    {
        // Remove all spells in the list
        for (int i = _spellsInList.Count - 1; i >= 0; i--)
            RemoveSpell(_spellsInList[i]);

        int spellCount = 0;
        foreach (Spell s in GameMainReferences.Instance.PlayerCharacter.UnlockedSpells)
        {
            var metaInfo = s.GetComponent<SpellMetaInfo>();
            if(metaInfo == null)
            {
                continue;
            }
            if (!_filterInfo.SpellApplicable(metaInfo))
            {
                continue;
            }
            AddSpell(s);
            spellCount++;
            if (spellCount == spellsPerPage)
                break;
        }

        Canvas.ForceUpdateCanvases();
        spellScroll.verticalScrollbar.value = 1f;
        Canvas.ForceUpdateCanvases();
    }

    private void AddSpell(Spell spell)
    {
        Transform t = Instantiate(spellItemPrefab);
        SpellItemGUI item = t.GetComponent<SpellItemGUI>();
        item.AssociatedSpell = spell;
        t.GetComponentInChildren<Text>().text = spell.SpellName;
        t.parent = spellListBody.transform;
        _spellsInList.Add(t);
    }

    private void RemoveSpell(Transform t)
    {
        _spellsInList.Remove(t);
        Destroy(t.gameObject);
    }
    /// <summary>
    /// Rebuild the list if a spell has been unlocked
    /// </summary>
    /// <param name="spell"></param>
    private void OnSpellUnlocked(Spell spell)
    {
        CreateSpellList();
    }

    public void SetFilter(string filter, bool value)
    {
        _filterInfo.SetFilter(filter, value);
        // Update Spell list once filter has been changed
        CreateSpellList();
    }

    public void SetFilter(SpellFilters filter, bool value)
    {
        _filterInfo.SetFilter(filter, value);
        // Update Spell list once filter has been changed
        CreateSpellList();
    }

    #endregion

    #region Event Methods

    public bool MouseOver
    {
        set { GameplayGUI.instance.IsMouseOver = value; }
    }

    public void CloseGUIWindow()
    {
        GameplayGUI.instance.IsMouseOver = false;
        gameObject.SetActive(false);
    }

    #endregion


}
