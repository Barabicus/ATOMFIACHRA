using UnityEngine;
using System.Collections;
using System;

[AttributeUsage(AttributeTargets.Class)]
public class SpellEffectStandardAttribute : Attribute
{
    public bool HasTargetEntity { get; set; }
    public bool HasTargetPosition { get; set; }
    public string SpellInfo { get; set; }

    public SpellEffectStandardAttribute(bool hasTargetEntity, bool hasTargetPosition, string spellInfo)
    {
        HasTargetEntity = hasTargetEntity;
        SpellInfo = spellInfo;
        HasTargetPosition = hasTargetPosition;
    }
}
