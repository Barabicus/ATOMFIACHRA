using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

[System.Serializable]
public struct ElementalStats
{
    public float fire;
    public float water;
    public float air;
    public float earth;
    public float physical;
    public float death;
    public float arcane;

    public float this[Element e]
    {
        get
        {
            return GetElementalStat(e);
        }
        set
        {
            SetElementalStat(e, value);
        }
    }

    public float Magnitude
    {
        get
        {
            float a = 0f;
            foreach (Element element in Enum.GetValues(typeof(Element)))
            {
                a += this[element];
            }
            return a;
        }
    }

    public ElementalStats(float fire, float water, float air, float earth, float kinetic, float death, float arcane)
    {
        this.fire = fire;
        this.water = water;
        this.physical = kinetic;
        this.air = air;
        this.earth = earth;
        this.death = death;
        this.arcane = arcane;
    }

    private float GetElementalStat(Element element)
    {
        switch (element)
        {
            case Element.Fire:
                return fire;
            case Element.Water:
                return water;
            case Element.Air:
                return air;
            case Element.Earth:
                return earth;
            case Element.Kinetic:
                return physical;
            case Element.Arcane:
                return arcane;
            case Element.Death:
                return death;
            default:
                return -404;
        }
    }

    private void SetElementalStat(Element element, float value)
    {
        switch (element)
        {
            case Element.Fire:
                this.fire = value;
                break;
            case Element.Water:
                this.water = value;
                break;
            case Element.Air:
                this.air = value;
                break;
            case Element.Earth:
                this.earth = value;
                break;
            case Element.Kinetic:
                this.physical = value;
                break;
            case Element.Death:
                this.death = value;
                break;
            case Element.Arcane:
                this.arcane = value;
                break;
        }
    }

    public static ElementalStats Zero
    {
        get
        {
            return new ElementalStats(0, 0, 0, 0, 0, 0, 0);
        }
    }

    public static ElementalStats One
    {
        get
        {
            return new ElementalStats(1, 1, 1, 1, 1, 1, 1);
        }
    }

    public static ElementalStats operator +(ElementalStats e1, ElementalStats e2)
    {
        return new ElementalStats(e1[Element.Fire] + e2[Element.Fire], e1[Element.Water] + e2[Element.Water], e1[Element.Air] + e2[Element.Air], e1[Element.Earth] + e2[Element.Earth], e1[Element.Kinetic] + e2[Element.Kinetic], e1[Element.Death] + e2[Element.Death], e1[Element.Arcane] + e2[Element.Arcane]);
    }

    public static ElementalStats operator -(ElementalStats e1, ElementalStats e2)
    {
        return new ElementalStats(e1[Element.Fire] - e2[Element.Fire], e1[Element.Water] - e2[Element.Water], e1[Element.Air] - e2[Element.Air], e1[Element.Earth] - e2[Element.Earth], e1[Element.Kinetic] - e2[Element.Kinetic], e1[Element.Death] - e2[Element.Death], e1[Element.Arcane] - e2[Element.Arcane]);
    }
    public static ElementalStats operator *(ElementalStats e1, ElementalStats e2)
    {
        return new ElementalStats(e1[Element.Fire] * e2[Element.Fire], e1[Element.Water] * e2[Element.Water], e1[Element.Air] * e2[Element.Air], e1[Element.Earth] * e2[Element.Earth], e1[Element.Kinetic] * e2[Element.Kinetic], e1[Element.Death] * e2[Element.Death], e1[Element.Arcane] * e2[Element.Arcane]);
    }

    public static ElementalStats operator /(ElementalStats e1, ElementalStats e2)
    {
        return new ElementalStats(e1[Element.Fire] / e2[Element.Fire], e1[Element.Water] / e2[Element.Water], e1[Element.Air] / e2[Element.Air], e1[Element.Earth] / e2[Element.Earth], e1[Element.Kinetic] / e2[Element.Kinetic], e1[Element.Death] / e2[Element.Death], e1[Element.Arcane] / e2[Element.Arcane]);
    }

    public static ElementalStats operator *(ElementalStats e1, float f)
    {
        return new ElementalStats(e1[Element.Fire] * f, e1[Element.Water] * f, e1[Element.Air] * f, e1[Element.Earth] * f, e1[Element.Kinetic] * f, e1[Element.Death] * f, e1[Element.Arcane] * f);
    }

    public ElementalStats Difference(ElementalStats other)
    {
        ElementalStats stats = ElementalStats.Zero;
        foreach (Element e in Enum.GetValues(typeof(Element)))
        {
            stats[e] = this[e] + (other[e] * -1);
        }
        return stats;
    }

    public override string ToString()
    {
        return "(" + this[Element.Fire] + " : " + this[Element.Water] + " : " + this[Element.Air] + " : " + this[Element.Earth] + " : " + this[Element.Kinetic] + " : " + this[Element.Death] + " : " + this[Element.Arcane] + ")";
    }
}
[Flags]
public enum Element
{
    Fire = 2,
    Water = 4,
    Kinetic = 8,
    Air = 16,
    Earth = 32,
    Death = 64,
    Arcane = 128
}