using UnityEngine;
using System.Collections;
using System;

[Serializable]
public struct EntityStats
{
    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _maxHp;
    [SerializeField]
    private ElementalStats _spellElementalModifier;
    [SerializeField]
    private ElementalStats _rechargeRate;
    [SerializeField]
    private ElementalStats _elementalResistance;
    [SerializeField]
    private ElementalStats _maxElementalCharge;

    public float Speed { get { return _speed; } set { _speed = value; } }
    public float MaxHp { get { return _maxHp; } set { _maxHp = value; } }
    public ElementalStats SpellElementalModifier { get { return _spellElementalModifier; } set { _spellElementalModifier = value; } }
    public ElementalStats RechargeRate { get { return _rechargeRate; } set { _rechargeRate = value; } }
    public ElementalStats ElementalResistance { get { return _elementalResistance; } set { _elementalResistance = value; } }
    public ElementalStats MaxElementalCharge { get { return _maxElementalCharge; } set { _maxElementalCharge = value; } }


    public EntityStats(float speed, float maxHp, ElementalStats spellElementalMod, ElementalStats rechargeRate, ElementalStats elementalResistance, ElementalStats maxElementalCharge)
    {
        this._speed = speed;
        this._maxHp = maxHp;
        this._spellElementalModifier = spellElementalMod;
        this._rechargeRate = rechargeRate;
        this._elementalResistance = elementalResistance;
        this._maxElementalCharge = maxElementalCharge;
    }

    public static EntityStats operator +(EntityStats e1, EntityStats e2)
    {
        return new EntityStats(e1._speed + e2._speed, e1._maxHp + e2._maxHp, e1._spellElementalModifier + e2._spellElementalModifier, e1._rechargeRate + e2._rechargeRate, e1._elementalResistance + e2._elementalResistance, e1._maxElementalCharge + e2._maxElementalCharge);
    }

    public static EntityStats operator -(EntityStats e1, EntityStats e2)
    {
        return new EntityStats(e1._speed - e2._speed, e1._maxHp - e2._maxHp, e1._spellElementalModifier - e2._spellElementalModifier, e1._rechargeRate - e2._rechargeRate, e1._elementalResistance - e2._elementalResistance, e1._maxElementalCharge - e2._maxElementalCharge);
    }

    public EntityStats Difference(EntityStats other)
    {
        return new EntityStats(FloatDifference(_speed, other._speed), FloatDifference(_maxHp, other._maxHp), _spellElementalModifier.Difference(other._spellElementalModifier), _rechargeRate.Difference(other.RechargeRate), _elementalResistance.Difference(other.ElementalResistance), _maxElementalCharge.Difference(other._maxElementalCharge));
    }

    private float FloatDifference(float f, float other)
    {
        return f + (other*-1);
    }

    public override string ToString()
    {
        return "(" + Speed + "," + MaxHp + "," + SpellElementalModifier + "," + RechargeRate +"," + ElementalResistance +"," + MaxElementalCharge + ")";
    }

}