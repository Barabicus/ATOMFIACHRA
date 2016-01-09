using System;
using UnityEngine;
using System.Collections;

public enum QuestCategory
{
    Entity,
    Location,
    Player,
    Misc,
    UI,
    Special
}

[AttributeUsage(AttributeTargets.Class)]
public class QuestCategoryAttribute : Attribute
{
    public string ListingName { get; set; }
    public QuestCategory Category { get; set; }

    public QuestCategoryAttribute(string listingName, QuestCategory category)
    {
        Category = category;
        ListingName = listingName;
    }

}