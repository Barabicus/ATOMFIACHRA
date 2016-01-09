using System;
using UnityEngine;
using System.Collections;

public enum SpellEffectCategory
{
    Motor,
    Entity,
    Spell,
    FX,
    Misc
}

[AttributeUsage(AttributeTargets.Class)]
public class SpellCategoryAttribute : Attribute
{
    public string ListingName { get; set; }
    public SpellEffectCategory Category { get; set; }

    public SpellCategoryAttribute(string listingName, SpellEffectCategory category)
    {
        Category = category;
        ListingName = listingName;
    }

}