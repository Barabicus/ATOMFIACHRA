using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EntitySpellBook : MonoBehaviour
{
    [SerializeField]
    private List<SpellProperty> _spellBook;

    public Spell GetSpell(float distance, float health, out float castTime)
    {
        var spells = from s in _spellBook
            where
            ((s.minDistance == -1 || s.maxDistance == -1) || (s.minDistance <= distance && s.maxDistance >= distance)) &&
            ((s.minHealth == -1 || s.maxHealth == -1) || (s.minHealth <= health && s.maxHealth >= health))
            select s;
        var ss = spells.ToArray();
        if (ss.Length == 0)
        {
            castTime = 0f;
            return null;
        }
        var prop = ss[UnityEngine.Random.Range(0, ss.Length)];
        castTime = prop.castTime;
        return prop.spell;
    }
}

[Serializable]
public struct SpellProperty
{
    public Spell spell;
    public float minDistance;
    public float maxDistance;
    public float minHealth;
    public float maxHealth;
    public float castTime;
    public int weight;
}
