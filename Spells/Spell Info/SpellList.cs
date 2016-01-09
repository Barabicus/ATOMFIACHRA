using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SpellList : GameController
{

    public static SpellList Instance;

    private Dictionary<string, Spell> _spellDict;
    private IDListPool<Spell> _spellPool;

    public override void OnAwake()
    {
        Instance = this;
        //   _spellDict = Resources.LoadAll<SpellListInfo>("Utility")[0].SpellDictionary;
        var spells = Resources.LoadAll<Spell>("Prefabs/Spells");

        _spellDict = new Dictionary<string, Spell>();
        foreach (Spell spell in spells)
        {
            if (!_spellDict.ContainsKey(spell.SpellID))
                _spellDict.Add(spell.SpellID, spell);
            else
                Debug.LogWarning("Spell already exists: " + spell.SpellID);
            spell.gameObject.SetActive(false);
        }

    }

    public override void OnStart()
    {
        _spellPool = SpellPool.Instance;
    }

    public Spell[] Spells
    {
        get { return SpellDictionary.Values.ToArray(); }
    }

    public Dictionary<string, Spell> SpellDictionary
    {
        get { return _spellDict;}
    } 

    public Spell GetNewSpell(string spell)
    {
        Spell sp = _spellPool.GetObjectFromPool(spell);
        sp.enabled = true;
        return sp;
    }

    public Spell GetNewSpell(Spell spell)
    {
        return GetNewSpell(spell.SpellID);
    }

    public Spell GetSpell(string spell)
    {
        if (spell == null)
            return null;
        return _spellDict[spell];
    }

    public Spell GetSpell(Spell spell)
    {
        return _spellDict[spell.SpellID];
    }

}