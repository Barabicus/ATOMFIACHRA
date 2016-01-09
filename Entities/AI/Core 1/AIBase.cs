using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public abstract class AIBase : EntityComponent
{
    //private AIProperty _currentAIProperty;

    //public AIProperty CurrentAIProperty
    //{
    //    get
    //    {
    //        return _currentAIProperty;
    //    }
    //    set
    //    {
    //        _currentAIProperty = value;
    //    }
    //}


    //public EntityAITarget AITarget { get; set; }

    ///// <summary>
    ///// Returns the distance to the target. returns null if no chase target exists
    ///// </summary>
    //public float? TargetDistance
    //{
    //    get
    //    {
    //        if (AITarget.Target != null)
    //            return Vector3.Distance(transform.position, AITarget.Target.position);
    //        else
    //            return null;
    //    }
    //}
    ///// <summary>
    ///// Returns the distance normal from 0 - 1 based on the distance between the Entity and the target compared to the min and the max target distances
    ///// of the current AI property.
    ///// </summary>
    //public float? DistanceNormal
    //{
    //    get
    //    {
    //        if (TargetDistance.HasValue)
    //        {
    //            if (MinTargetDistance <= 0 && MaxTargetDistance <= 0)
    //                return 0f;
    //            return 1f - (TargetDistance.Value / (MaxTargetDistance - MinTargetDistance));
    //        }
    //        return null;
    //    }
    //}

    //public abstract AIProperty[] AIProperties { get; }

    //public float MinTargetDistance
    //{
    //    get
    //    {
    //        if (CurrentAIProperty != null)
    //        {
    //            return CurrentAIProperty.Conditionals.minTargetDistance;
    //        }
    //        return 0f;
    //    }
    //}
    //public float MaxTargetDistance
    //{
    //    get
    //    {
    //        if (CurrentAIProperty != null)
    //        {
    //            return CurrentAIProperty.Conditionals.maxTargetDistance;
    //        }
    //        return 0f;
    //    }
    //}
    //public float MinHealth
    //{
    //    get
    //    {
    //        if (CurrentAIProperty != null)
    //        {
    //            return CurrentAIProperty.Conditionals.minHealth;
    //        }
    //        return 0f;
    //    }
    //}
    //public float MaxHealth
    //{
    //    get
    //    {
    //        if (CurrentAIProperty != null)
    //        {
    //            return CurrentAIProperty.Conditionals.maxHealth;
    //        }
    //        return 0f;
    //    }
    //}

    //#region Initialisation

    //protected override void Start()
    //{
    //    base.Start();
    //    AITarget = GetComponent<EntityAITarget>();
    //}

    //#endregion

    //#region AI

    //private void TrySelectAIProperty()
    //{
    //    if (AIProperties == null)
    //        return;

    //    var properties = (
    //        from n in AIProperties
    //        where n.IsSelectable(this)
    //        select n).ToList();

    //    if (properties.Count > 0)
    //        SelectAIProperty(properties);
    //}

    //protected abstract void SelectAIProperty(List<AIProperty> properties);

    //#endregion

}
//[System.Serializable]
//public class AIProperty
//{
//    [SerializeField]
//    private PropertyConditionals _propertyConditionals;
//    /// <summary>
//    /// This is the timer associated with the Entity Motion. Note this is hidden from the inspector view and will count how much time has passed since
//    /// this property has been activated. This can be used to control Motion based on time.
//    /// </summary>
//    private float _propertyTime;

//    public PropertyConditionals Conditionals { get { return _propertyConditionals; } }
//    public float PropertyTime { get { return _propertyTime; } }

//    public virtual bool IsSelectable(AIBase ai)
//    {
//        float healthNorm = ai.Entity.CurrentHealthNormalised;
//        return (Conditionals.maxTargetDistance < 0 || Conditionals.maxTargetDistance < 0 || (ai.TargetDistance.HasValue && Conditionals.minTargetDistance <= ai.TargetDistance.Value && Conditionals.maxTargetDistance >= ai.TargetDistance.Value)) && ((Conditionals.maxHealth < 0 || Conditionals.maxHealth < 0) || (Conditionals.minHealth <= healthNorm && Conditionals.maxHealth >= healthNorm));
//    }

//}
//[System.Serializable]
//public class PropertyConditionals
//{
//    [Tooltip("The minimum distance to the target the Ai must be before this property can be considered")]
//    public float minTargetDistance;
//    [Tooltip("The maximum distance to the target the Ai must be before this property can be considered")]
//    public float maxTargetDistance;
//    [Tooltip("The minimum health the Ai must have before this property can be considered")]
//    public float minHealth;
//    [Tooltip("The maximum health the Ai must have before this property can be considered")]
//    public float maxHealth;
//}