using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class EntityLevelHandler
{
    public const int MAXLEVEL = 100;

    [SerializeField]
    private int _currentLevel = 1;

    [SerializeField]
    private float _minSpeed = 5f;
    [SerializeField]
    private float _maxSpeed = 5f;
    [SerializeField]
    private AnimationCurve _speedCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    [SerializeField]
    private float _minHealth = 100f;
    [SerializeField]
    private float _maxHealth = 500f;
    [SerializeField]
    private AnimationCurve _healthCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    [SerializeField]
    private ElementalStats _minElementalModifier = ElementalStats.One;
    [SerializeField]
    private ElementalStats _maxElementalModifier = ElementalStats.One;
    [SerializeField]
    private AnimationCurve _elementalModifierCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    [SerializeField]
    private ElementalStats _minRechargeRate = ElementalStats.One;
    [SerializeField]
    private ElementalStats _maxRechargeRate = ElementalStats.One;
    [SerializeField]
    private AnimationCurve _rechargeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    [SerializeField]
    private ElementalStats _minElementalResistance = ElementalStats.One;
    [SerializeField]
    private ElementalStats _maxElementalResistance = ElementalStats.One;
    [SerializeField]
    private AnimationCurve _elementalResistanceCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    [SerializeField]
    private ElementalStats _minElementalCharge = ElementalStats.One;
    [SerializeField]
    private ElementalStats _maxElementalCharge = ElementalStats.One;
    [SerializeField]
    private AnimationCurve _elementalChargeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    [SerializeField]
    [Tooltip("How much experience this entity deals when defeated")]
    private float _minExperienceReward;
    [SerializeField]
    [Tooltip("How much experience this entity deals when defeated")]
    private float _maxExperienceReward;
    [SerializeField]
    private AnimationCurve _experienceCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    public Entity Owner { get; set; }

    public float MinSpeed { get { return _minSpeed; } set { _minSpeed = value; } }
    public float MaxSpeed { get { return _maxSpeed; } set { _maxSpeed = value; } }
    public float MinHealth { get { return _minHealth; } set { _minHealth = value; } }
    public float MaxHealth { get { return _maxHealth; } set { _maxHealth = value; } }
    public ElementalStats MinElementalModifier { get { return _minElementalModifier; } set { _minElementalModifier = value; } }
    public ElementalStats MaxElementalModifier { get { return _maxElementalModifier; } set { _maxElementalModifier = value; } }
    public ElementalStats MinRechargeRate { get { return _minRechargeRate; } set { _minRechargeRate = value; } }
    public ElementalStats MaxRechargeRate { get { return _maxRechargeRate; } set { _maxRechargeRate = value; } }
    public ElementalStats MinElementalResistance { get { return _minElementalResistance; } set { _minElementalResistance = value; } }
    public ElementalStats MaxElementalResistance { get { return _maxElementalResistance; } set { _maxElementalResistance = value; } }
    public ElementalStats MinElementalCharge { get { return _minElementalCharge; } set { _minElementalCharge = value; } }
    public ElementalStats MaxElementalCharge { get { return _maxElementalCharge; } set { _maxElementalCharge = value; } }
    public float MinExperienceReward { get { return _minExperienceReward; } set { _minExperienceReward = value; } }
    public float MaxExperienceReward { get { return _maxExperienceReward; } set { _maxExperienceReward = value; } }


  //  public float ExperienceReward { get { return GetRatioValue(MinExperienceReward, MaxExperienceReward, _experienceCurve.Evaluate(LevelNormal)); } }

    public float LevelNormal { get { return _currentLevel / (float)MAXLEVEL; } }

    private const string levelStatsID = "EntityLevelStats";
    /// <summary>
    /// Ensure the stats for the specific level of the entity are updated and reflect on the entity.
    /// </summary>
    public void UpdateEntityStats()
    {
        Owner.StatHandler.UpdateStatModifiers(levelStatsID, EntityStats, true);
    }

    /// <summary>
    /// Gets the Entity stats associated with the level and the attributue specifications
    /// </summary>
    public EntityStats EntityStats
    {
        get
        {
            return new EntityStats(GetRatioValue(_minSpeed, _maxSpeed, _speedCurve.Evaluate(LevelNormal)), GetRatioValue(_minHealth, _maxHealth, _healthCurve.Evaluate(LevelNormal)), GetElementalStatsRatioValue(_minElementalModifier, _maxElementalModifier, _elementalModifierCurve.Evaluate(LevelNormal)), GetElementalStatsRatioValue(_minRechargeRate, _maxRechargeRate, _rechargeCurve.Evaluate(LevelNormal)), GetElementalStatsRatioValue(_minElementalResistance, _maxElementalResistance, _elementalResistanceCurve.Evaluate(LevelNormal)), GetElementalStatsRatioValue(_minElementalCharge, _maxElementalCharge, _elementalChargeCurve.Evaluate(LevelNormal)));
        }
    }

    /// <summary>
    /// Returns how much experience should be rewarded based on the passed in level. 
    /// For balancing purposes the max amount of experience will be reward on two factors.
    /// The first one being the level of this entity. This entity will only return the max amount 
    /// of experience for it's level. This is to prevent high level players killing low level enemies of this type
    /// and returning the same experience.
    /// The second factor is the level of the player. Experience will be only returned on the max level of the player
    /// so if the player kills high level enemies of this type it will only allow an experience gain factor of 2 levels
    /// ahead of the player. This is to prevent high level enemies giving off absurb amount of experience if the player
    /// somehow managest to defeat them.
    /// </summary>
    /// <param name="compareLevel"></param>
    /// <returns></returns>
    public float GetExperienceRewardForLevel(int compareLevel)
    {
        // Don't grant experience three levels in advance
        if( Mathf.Abs(CurrentLevel - compareLevel) > 3)
        {
            return 0f;
        }
        float level = CurrentLevel;
        if(CurrentLevel > compareLevel)
        {
            // Allow for 2 level increments if the entity level is greater
            level = compareLevel + Mathf.Min(2f, CurrentLevel - compareLevel);
        }

        // Normalise level
        level /= (float)MAXLEVEL;
        return GetRatioValue(MinExperienceReward, MaxExperienceReward, _experienceCurve.Evaluate(level));
    }

    private float GetRatioValue(float min, float max, float ratio)
    {
        return (ratio * (max - min)) + min;
    }

    private ElementalStats GetElementalStatsRatioValue(ElementalStats min, ElementalStats max, float ratio)
    {
        ElementalStats es = ElementalStats.Zero;
        foreach (Element e in Enum.GetValues(typeof(Element)))
        {
            es[e] = GetRatioValue(min[e], max[e], ratio);
        }
        return es;
    }

    /// <summary>
    /// The current level of the Entity. While it is possible to directly modify the level it is suggested
    /// to use AdvanceLevel for typical leveling up needs.
    /// </summary>
    public int CurrentLevel
    {
        get { return _currentLevel; }
        set
        {
            _currentLevel = Mathf.Min(value, MAXLEVEL);
            UpdateEntityStats();
        //    Owner.CurrentHp = Owner.StatHandler.MaxHp;
        }
    }

    public void AdvanceLevel()
    {
        CurrentLevel++;
    }
}
