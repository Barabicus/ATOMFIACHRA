using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

[RequireComponent(typeof(EffectSetting))]
[RequireComponent(typeof(SpellMetaInfo))]
public class Spell : MonoBehaviour, IPoolableID
{

    #region Fields
    [FormerlySerializedAs("spellID")]
    [SerializeField]
    private string _spellID;
    [FormerlySerializedAs("spellName")]
    [SerializeField]
    private string _spellName = "NOTSET";
    [SerializeField]
    private SpellCastMethod _spellCastMethod = SpellCastMethod.FreeCast;
    [SerializeField]
    private SpellKillMethod _spellKillMethod = SpellKillMethod.Timed;
    [FormerlySerializedAs("spellLiveTime")]
    [SerializeField]
    private float _spellLiveTime;
    [FormerlySerializedAs("spellCastDelay")]
    [SerializeField]
    private float _spellCastDelay;
    [FormerlySerializedAs("spellType")]
    [SerializeField]
    private SpellType _spellType;
    [SerializeField]
    private SpellDeathMarker _spellDeathMarker = SpellDeathMarker.None;
    [FormerlySerializedAs("castAudio")]
    [SerializeField]
    private AudioClip _castAudio;
    [FormerlySerializedAs("spellIcon")]
    [SerializeField]
    private Sprite _spellIcon;
    [FormerlySerializedAs("spellDescription")]
    [SerializeField]
    private string _spellDescription = "NOT IMPLEMENTED";
    [FormerlySerializedAs("elementalCost")]
    [SerializeField]
    private ElementalStats _elementalCost;
    [SerializeField]
    [Tooltip("This is how many updates must be run before the spell can be destroyed via a time tick. This is useful when dealing with high delta time that could trigger a unexpected end to the spell.")]
    private int _forceUpdateAmount = 1;

    private AudioSource _audioSource;
    /// <summary>
    /// A check to see if the amount of updates ran is equal to the force update amount.
    /// </summary>
    private int _updatesRan = 0;

    #endregion

    #region Properties
    /// <summary>
    /// This is the Entity that triggered the creation of the spell
    /// </summary>
    public Entity CastingEntity { get; set; }
    /// <summary>
    /// This is a list of Entities that the Spell will ignore collisions with
    /// </summary>
    public List<Entity> IgnoreEntities { get; set; }
    /// <summary>
    /// This is the effect setting associated with this spell
    /// </summary>
    public EffectSetting SpellEffectSetting { get; set; }
    /// <summary>
    /// This is the death marker that the spell will leave when it is the spell that has killed an Entity. Eg burned, frozen etc
    /// </summary>
    public SpellDeathMarker SpellDeathMarker
    {
        get { return _spellDeathMarker; }
    }
    /// <summary>
    /// This is a brief description of the spell that will appear in the spell book
    /// </summary>
    public string SpellDescription
    {
        get { return _spellDescription; }
    }
    /// <summary>
    /// This is the target transform for the spell. The target transform is used by motors that will trigger a collision between the specific transform.
    /// Motors like the missile motor will move through world space and target what they encounter the spell target is used to target specific targets.
    /// This value is typically used for spells such as attached spells or physical spells where a specific transform should be specificed as the target.
    /// </summary>
    public Transform SpellTargetTransform
    {
        get;
        set;
    }
    /// <summary>
    /// This is the target position for the spell. This is used by motors to target a specific point in world space. Depending on the spell this point could be 
    /// used to position the spell where it starts or to get a direction to move in. A area spell could use a spell target to place itself there and a missile spell
    /// could use this to aim towards it.
    /// </summary>
    public Vector3? SpellTargetPosition
    {
        get;
        set;
    }
    /// <summary>
    /// The start position of the entity. This is the position that the spell was cast from. Typically this will be the casting point of the Entity.
    /// </summary>
    public Vector3 SpellStartPosition
    {
        get;
        private set;
    }
    /// <summary>
    /// The position where the spell was started. Note this is not usually the entity
    /// more so the casting hand for example.
    /// </summary>
    public Transform SpellStartTransform
    {
        get;
        private set;
    }
    /// <summary>
    /// This is how much Elemental power it will cost to cast the spell
    /// </summary>
    public ElementalStats ElementalCost
    {
        get { return _elementalCost; }
    }
    /// <summary>
    /// This is how long the spell can live for before it is destroyed
    /// </summary>
    public virtual float SpellLiveTime
    {
        get { return _spellLiveTime; }
    }
    /// <summary>
    /// This is the type of the spell i.e. Area, Missile, Attached etc
    /// </summary>
    public SpellType SpellType
    {
        get { return _spellType; }
    }
    /// <summary>
    /// How much delay this spell leaves before you can cast another spell
    /// </summary>
    public virtual float SpellCastDelay
    {
        get { return _spellCastDelay; }
    }
    /// <summary>
    ///  This is the ID associated with the spell. This is used for hashmap look ups.
    /// </summary>
    public string SpellID
    {
        get { return _spellID; }
    }
    /// <summary>
    /// The timer that willl control when the Spell should be destroyed. This ticks once the amount of time specified in spell
    /// live time has passed. This may be reset to prevent spell destruction.
    /// </summary>
    public Timer SpellDestroyTimer { get; set; }
    public string PoolID
    {
        get { return SpellID; }
    }
    public KeepAlive KeepAliveDelegate { get; set; }
    public Sprite SpellIcon
    {
        get { return _spellIcon; }
    }
    public string SpellName
    {
        get { return _spellName; }
    }
    public SpellKillMethod SpellKillMethod { get { return _spellKillMethod; } }
    public SpellCastMethod SpellCastMethod { get { return _spellCastMethod; } }
    public bool HasInitialised { get; private set; }
    #endregion

    #region Events
    public event Action<Spell> OnSpellDestroy;
    public event Action<Spell> OnSpellRecycle;
    /// <summary>
    /// An Event fired when a spell has been refreshed. This typically only occurs with single instance spells
    /// such as attached spells eg: Burning tick. This can be useful order events not when the spell has been cast
    /// but when the spell has been refreshed which in a sense is like a cast.
    /// </summary>
    public event Action<Spell> OnSpellRefresh;
    #endregion

    /// <summary>
    /// This is called when a spell is first created. This is similiar to Awake but ensure it is called as the spell is created.
    /// </summary>
    public void Initialise()
    {
        IgnoreEntities = new List<Entity>();
        gameObject.layer = 10;
        SpellEffectSetting = GetComponent<EffectSetting>();
        SpellEffectSetting.Initialize();
        SpellDestroyTimer = new Timer(SpellLiveTime);
        // Stop the Timer until the spell is cast
        SpellDestroyTimer.IsStopped = true;
        _audioSource = gameObject.AddComponent<AudioSource>();
        HasInitialised = true;
    }

    private void Start()
    {
        if (!HasInitialised)
        {
            Debug.LogErrorFormat("Spell {0} was not intialised properly.", gameObject.name);
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// When a spell is created it must be cast before it can exist properly in the world. This will setup important information associated with the spell.
    /// </summary>
    /// <param name="castingEntity"></param>
    /// <param name="startPosition"></param>
    /// <param name="startVector"></param>
    /// <param name="spellTarget"></param>
    /// <param name="spellTargetPosition"></param>
    public void CastSpell(Entity castingEntity, Transform startPosition = null, Vector3? startVector = null, Transform spellTarget = null, Vector3? spellTargetPosition = null)
    {
        CastingEntity = castingEntity;

        SpellStartTransform = startPosition;
        SpellStartPosition = startVector.HasValue ? startVector.Value : startPosition.position;
        SpellTargetTransform = spellTarget;
        SpellTargetPosition = spellTargetPosition ?? (SpellTargetTransform != null ? (Vector3?)SpellTargetTransform.position : null);
        transform.position = SpellStartPosition;
        transform.rotation = CastingEntity.transform.rotation;

        // Reset and ensure the timer is started
        SpellDestroyTimer.Reset();
        SpellDestroyTimer.IsStopped = false;

        SpellEffectSetting.TriggerSpellStart();

        gameObject.SetActive(true);
        PlaySound();
    }

    private void PlaySound()
    {
        _audioSource.PlayOneShot(_castAudio);
    }

    #region Trigger Events

    /// <summary>
    /// Called when the spell should be destroyed. This calls the OnSpellDestoyEvent and will disable the spell script.
    /// </summary>
    public void TriggerDestroySpell()
    {
        SpellDestroyTimer.IsStopped = true;
        if (OnSpellDestroy != null)
            OnSpellDestroy(this);
        enabled = false;
    }
    public void TriggerSpellRefresh()
    {
        SpellDestroyTimer.Reset();
    }
    #endregion

    private void Update()
    {
        if (_updatesRan == _forceUpdateAmount)
        {
            switch (_spellKillMethod)
            {
                case SpellKillMethod.Timed:
                    if (SpellDestroyTimer.CanTick)
                    {
                        TriggerDestroySpell();
                    }
                    break;
                case SpellKillMethod.KeepAlive:
                    if (KeepAliveDelegate == null || !KeepAliveDelegate())
                    {
                        TriggerDestroySpell();
                    }
                    break;
                case SpellKillMethod.TimedAndKeepAlive:
                    if (KeepAliveDelegate == null || SpellDestroyTimer.CanTick || !KeepAliveDelegate())
                    {
                        TriggerDestroySpell();
                    }
                    break;
            }
        }
        _updatesRan = Mathf.Min(_updatesRan + 1, _forceUpdateAmount);
    }
    /// <summary>
    /// This is triggered when the spell should be reset and returned to the pool.
    /// </summary>
    public void Recycle()
    {
        OnSpellDestroy = null;
        IgnoreEntities.Clear();
        KeepAliveDelegate = null;
        if(OnSpellRecycle != null)
        {
            OnSpellRecycle(this);
        }
    }

    public void PoolStart() { }
}

public enum SpellType
{
    Missile,
    Physical,
    Attached,
    Area,
    Raycast
}

public enum SpellDeathMarker
{
    None,
    Explode,
    Freeze,
    Burn,
    Gore
}

public class SpellEventargs : EventArgs
{
    public Spell spell;

    public SpellEventargs(Spell spell)
    {
        this.spell = spell;
    }

}

public delegate bool KeepAlive();

public enum SpellKillMethod
{
    Timed, // The spell will be killed after the specified time finished
    KeepAlive, // A delegate call will be used to keep the spell alive. Returning false at anytime will kill the spell.
    TimedAndKeepAlive, // A mix of the above.
    Triggered // Spell destruction must be explicitly triggered.
}
/// <summary>
/// Spell cast methods used to indicate how the spell wil behave when cast.
/// FreeCast means the spell can be continiously cast without consequence. Single Instance
/// means a entity can only create one spell of this type and must wait until it is dead to recast.
/// SingleInstanceRefresh means that like single instance only a single spell of this Type can be cast
/// but if another spell of the same type is cast it will refresh the duration of that spell. Assuming of course
/// it is using a Timed Spell Kill Method.
/// Single Instance Refresh should be the typical method used in conjuction with attached spells.
/// </summary>
public enum SpellCastMethod
{
    FreeCast, // Cast without consequence
    SingleInstance, // An Entity may only cast a single instance of the spell
    SingleInstanceRefresh // Similar to the above except any subsequent casts will refresh the spell timer, should it be time specific.
}