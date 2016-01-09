using UnityEngine;
using System.Collections;
using System;

public enum TransitionFxCategory
{
    Entity,
    Fx
}

[AttributeUsage(AttributeTargets.Class)]
public class TransitionFxCategoryAttribute : Attribute
{
    public TransitionFxCategory Category { get; set; }
    public string ListingName { get; set; }

    public TransitionFxCategoryAttribute(string ListingName, TransitionFxCategory category)
    {
        this.ListingName = ListingName;
    }
}
