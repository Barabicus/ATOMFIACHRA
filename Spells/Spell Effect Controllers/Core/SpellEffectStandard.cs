using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

/// <summary>
/// The standard spell effect allows us a standard of setting up and handling specific spell related events.
/// Inheriting from this class means we can easily listen for when a Event is triggered and deal with it appropriately. 
/// Using this we can generalise many different spell effects to behave the same but respond to different triggers.
/// </summary>
public abstract class SpellEffectStandard : SpellEffect
{
    [HideInInspector]
    [SerializeField]
    [FormerlySerializedAs("triggerEvent")]
    private SpellEffectTriggerEvent _triggerEvent;
    [HideInInspector]
    [SerializeField]
    [FormerlySerializedAs("timeTrigger")]
    [Tooltip("If using a timed trigger event this is how long it will take to trigger the event")]
    private float _timeTrigger = 1f;
    [HideInInspector]
    [SerializeField]
    [FormerlySerializedAs("isSingleShot")]
    private bool _isSingleShot = false;
    [SerializeField]
    [HideInInspector]
    private EntityTarget _entityTargetMethod = EntityTarget.None;
    [SerializeField]
    [HideInInspector]
    private SpellPositionTarget _positionTarget = SpellPositionTarget.None;
    [SerializeField]
    [HideInInspector]
    private bool _tickOnStart = false;
    [SerializeField]
    [HideInInspector]
    private string[] _listenForEventIDs;

    private Entity _lastApplyEntity;

    public enum EntityTarget
    {
        None,
        ApplyEntity,
        CastingEntity,
        LastApplyEntity
    }

    public enum SpellPositionTarget
    {
        None,
        SpellTargetTransform, // The returned position will be that of the target transform. Useful is the spell should keep an updated lock on a target.
        SpellTargetPosition, // The returned position will be the assigned spell position. Since this is assigned just once it is useful for area location spells but not for keeping a lock on.
        CollisionPoint, // The returned position will be the hit point.
        SpellStartTransform, // The returned position will be associated with the transform of where it started. i.e. the hand it was cast from. This will keep a constant update on the location.
        SpellStartPosition // The world point that the spell started in initially.
    }

    private Timer timedEvent;
    private bool r_enabled;

    #region Properties
    public SpellEffectTriggerEvent TriggerEvent
    {
        get { return _triggerEvent; }
        set { _triggerEvent = value; }
    }
    public EntityTarget EntityTargetMethod
    {
        get { return _entityTargetMethod; }
        set { _entityTargetMethod = value; }
    }
    public SpellPositionTarget PositionTarget
    {
        get { return _positionTarget; }
        set { _positionTarget = value; }
    }
    public float TimeTrigger
    {
        get { return _timeTrigger; }
        set { _timeTrigger = value; }
    }
    public bool IsSingleShot
    {
        get { return _isSingleShot; }
        set { _isSingleShot = value; }
    }
    #endregion

    public override void Initialize(EffectSetting effectSetting)
    {
        base.Initialize(effectSetting);
        r_enabled = enabled;
    }

    protected override void OnSpellStart()
    {
        base.OnSpellStart();
        enabled = r_enabled;
        timedEvent = new Timer(_timeTrigger);
        if (_tickOnStart)
        {
            timedEvent.ForceTickTime();
        }
    }
    /// <summary>
    /// Triggers an event when a spell has been refreshed and if the trigger even is set to refresh
    /// </summary>
    protected override void OnSpellRefresh()
    {
        base.OnSpellRefresh();
        if (_triggerEvent == SpellEffectTriggerEvent.SpellRefresh)
        {
            EventTriggered(new SpellEffectStandardEventArgs(SpellEffectTriggerEvent.SpellRefresh));
        }
    }
    /// <summary>
    /// Triggers an event when a spell has been cast and if the trigger event is set to cast
    /// </summary>
    protected override void effectSetting_OnSpellCast()
    {
        base.effectSetting_OnSpellCast();
        if (_triggerEvent == SpellEffectTriggerEvent.Cast)
            EventTriggered(new SpellEffectStandardEventArgs(SpellEffectTriggerEvent.Cast));
    }
    /// <summary>
    /// Triggers an event when on a collision has been cast and if the trigger event is set to OnCollision
    /// </summary>
    protected override void effectSetting_OnSpellCollision(ColliderEventArgs args, Collider obj)
    {
        base.effectSetting_OnSpellCollision(args, obj);
            if (_triggerEvent == SpellEffectTriggerEvent.Collision)
                EventTriggered(new SpellEffectStandardEventArgs(SpellEffectTriggerEvent.Collision) { Collider = obj, TargetPosition = args.HitPoint });
    }
    /// <summary>
    /// Triggers an event when a spell has been destroyed and if the trigger event is set to OnSpellDestroy
    /// </summary>
    protected override void effectSetting_OnSpellDestroy()
    {
        base.effectSetting_OnSpellDestroy();
        if (_triggerEvent == SpellEffectTriggerEvent.SpellDestroy)
            EventTriggered(new SpellEffectStandardEventArgs(SpellEffectTriggerEvent.SpellDestroy));
    }
    /// <summary>
    /// Triggers an event when a spell effect has been destroyed and if the trigger event is set to OnEffectDestroy
    /// </summary>
    protected override void effectSetting_OnEffectDestroy()
    {
        base.effectSetting_OnEffectDestroy();
        if (_triggerEvent == SpellEffectTriggerEvent.EffectDestroy)
            EventTriggered(new SpellEffectStandardEventArgs(SpellEffectTriggerEvent.EffectDestroy));
    }
    /// <summary>
    /// Triggers an event when a spell has been applied and if the trigger event is set to OnSpellApply
    /// </summary>
    protected override void effectSetting_OnSpellApply(Entity entity)
    {
        base.effectSetting_OnSpellApply(entity);
        _lastApplyEntity = entity;
        if (_triggerEvent == SpellEffectTriggerEvent.SpellApply)
        {
            // Set the args and check if the target entity should be passed in as spell apply
            var args = new SpellEffectStandardEventArgs(SpellEffectTriggerEvent.SpellApply);
            if (EntityTargetMethod == EntityTarget.ApplyEntity)
            {
                args.TargetEntity = entity;
            }
            EventTriggered(args);
        }
    }
    /// <summary>
    /// Trigegrs an event when a special event has been triggered and if the trigger event is set to OnSpecialEvent
    /// </summary>
    /// <param name="eventID"></param>
    protected override void OnSpecialEvent(string eventID)
    {
        base.OnSpecialEvent(eventID);
        // Check if any of the listening IDs are equal to the event ID and trigger the event
        foreach (var id in _listenForEventIDs)
        {
            if (id.Equals(eventID))
            {
                if (_triggerEvent == SpellEffectTriggerEvent.SpecialEvent)
                {
                    var args = new SpellEffectStandardEventArgs(SpellEffectTriggerEvent.SpecialEvent);
                    args.EventID = eventID;
                    EventTriggered(args);
                }
                return;
            }
        }

    }
    /// <summary>
    /// Triggers an event when a certain amount of time has passed and if the trigger event is set to Timed
    /// Or Trigger an event if the trigger event is set to Spell Update. 
    /// </summary>
    protected override void UpdateSpell()
    {
        base.UpdateSpell();
        if (_triggerEvent == SpellEffectTriggerEvent.SpellUpdate)
        {
            EventTriggered(new SpellEffectStandardEventArgs(SpellEffectTriggerEvent.SpellUpdate));
        }
        else if (_triggerEvent == SpellEffectTriggerEvent.Timed && timedEvent.CanTickAndReset())
        {
            if (_isSingleShot)
                enabled = false;
            EventTriggered(new SpellEffectStandardEventArgs(SpellEffectTriggerEvent.Timed));
        }
    }
    /// <summary>
    /// Called when an event should be triggered. This is a private method for handling
    /// the event before calling DoEventTriggered which will register the actual event.
    /// </summary>
    /// <param name="args"></param>
    private void EventTriggered(SpellEffectStandardEventArgs args)
    {
        // If casting Entity set the casting Entity
        // If it is the last Apply Entity use that instead
        // If it uses any other method it should have already been assigned.
        switch (EntityTargetMethod)
        {
            case EntityTarget.CastingEntity:
                args.TargetEntity = effectSetting.spell.CastingEntity;
                break;
            case EntityTarget.LastApplyEntity:
                args.TargetEntity = _lastApplyEntity;
                break;
        }
        switch (PositionTarget)
        {
            case SpellPositionTarget.SpellStartPosition:
                args.TargetPosition = effectSetting.spell.SpellStartPosition;
                break;
            case SpellPositionTarget.SpellStartTransform:
                if (effectSetting.spell.SpellTargetTransform == null)
                    Debug.LogErrorFormat("Spell {0} with spell effect {1} using postion target of SpellTargetTransform had no spell Target", effectSetting.name, name);
                args.TargetPosition = effectSetting.spell.SpellStartTransform.position;
                break;
            case SpellPositionTarget.SpellTargetPosition:
                args.TargetPosition = effectSetting.spell.SpellTargetPosition.Value;
                break;
            case SpellPositionTarget.SpellTargetTransform:
                if (effectSetting.spell.SpellTargetTransform == null)
                    Debug.LogErrorFormat("Spell {0} with spell effect {1} using postion target of SpellTargetTransform had no spell Target", effectSetting.name, name);
                args.TargetPosition = effectSetting.spell.SpellTargetTransform.position;
                break;
        }
        DoEventTriggered(args);
    }

    protected virtual void DoEventTriggered(SpellEffectStandardEventArgs args)
    {

    }

}

public struct SpellEffectStandardEventArgs
{
    private SpellEffectTriggerEvent _triggerEvent;
    private Entity _entity;
    private Collider _collider;
    private string _eventID;
    private Vector3 _targetPosition;


    public SpellEffectTriggerEvent TriggerEvent
    {
        get { return _triggerEvent; }
        set { _triggerEvent = value; }
    }

    public Entity TargetEntity
    {
        get { return _entity; }
        set { _entity = value; }
    }

    public Collider Collider
    {
        get { return _collider; }
        set { _collider = value; }
    }
    /// <summary>
    /// A motor may wish to assign a specific ID to an event. Spells can listen for this
    /// ID and determine if they should trigger an event or not.
    /// </summary>
    public string EventID
    {
        get { return _eventID; }
        set { _eventID = value; }
    }

    public Vector3 TargetPosition
    {
        get
        {
            return _targetPosition;
        }

        set
        {
            _targetPosition = value;
        }
    }

    public SpellEffectStandardEventArgs(SpellEffectTriggerEvent triggerEvent)
    {
        _triggerEvent = triggerEvent;
        _collider = null;
        _entity = null;
        _eventID = "";
        _targetPosition = Vector3.zero;
    }

}

public enum SpellEffectTriggerEvent
{
    Timed, // Event On Timed
    Collision, // Event On Collision
    SpellApply, // Event on Spell Apply ( called by motors)
    SpellDestroy, // Event on Spell Destroy
    EffectDestroy, // Event on Effect Destroy
    Cast, // Event on Cast
    SpecialEvent, // Special Event ( called by motor for a general type specific event )
    SpellUpdate, // Continiously called
    SpellRefresh // Triggered when a spell has been refreshed
}
