using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using BehaviorDesigner.Runtime;

[RequireComponent(typeof(AudioSource))]
[DisallowMultipleComponent]
public class Entity : MonoBehaviour, IPoolableID
{
    #region Fields

    [SerializeField]
    private EntityLevelHandler _levelHandler;
    // [SerializeField]
    private float _currentHp = 0;
    [SerializeField]
    private string _entityName = "NOTSET";
    [SerializeField]
    private string _entityID;
    [SerializeField]
    private bool _isInvincible = false;
    [SerializeField]
    private bool loadHealthBar = true;
    [SerializeField]
    private bool loadMinimapIcon = true;
    [SerializeField]
    private bool _loadCastingBar = false;
    [SerializeField]
    private FactionFlags _factionFlags = FactionFlags.Two;
    [SerializeField]
    private bool _spellsIgnoreElementalCost = true;
    [SerializeField]
    private AudioClip _deathAudio = null;
    [SerializeField]
    [Tooltip("If true the entity will ignore the disable distance set out by the entity manager. The disable distance is determained by the distance from the player. Essentially if this is false players outside a certain distance of the player will be disabled to avoid runtime processing. If this is true the Entity will always be allowed to update regardless of distance from the player.")]
    [UnityEngine.Serialization.FormerlySerializedAs("_canAlwaysUpdate")]
    private bool _ignoreDisableDistance;
    [SerializeField]
    [Tooltip("This is the point the health bar will position itself to")]
    private Transform _guiHealthPoint;
    [SerializeField]
    private Transform _guiSpeechPoint;
    [SerializeField]
    private EntityCastPoint _entityCastPoint;
    [SerializeField]
    [Tooltip("The minimum amount of health an this entity can go down to. Use for events. An event will fire when this number is hit. If this is above 0 the entity will not be able to be killed by natural means.")]
    private float _minHealthClamp = 0f;
    [SerializeField]
    [Tooltip("If a behaviour tree is present on this entity should it be disabled when the game enters cinematic mode.")]
    private bool _disableBehaviorOnCinematic = true;

    private ElementalStats _currentElementalCharge = ElementalStats.Zero;

    private Vector3 _lastPosition = Vector3.zero;
    private float _currentSpeed;
    private EntityLivingState _livingState = EntityLivingState.Alive;
    private Dictionary<string, Spell> _singleInstanceSpells = new Dictionary<string, Spell>();
    private AudioSource _audio;
    private Timer _audioPlayTimer;
    private Timer _updateSpeedTimer;

    protected Transform SelectedTarget;
    private bool _canUpdate = true;
    private ElementalStats _rechargeLock;
    private List<EntityComponent> _entityComponents;

    private const string _baseStatsID = "entity_base_stats";

    #endregion

    #region Events
    /// <summary>
    /// This event fires when the Entity has bee killed.
    /// </summary>
    public event Action<Entity> OnKilled;
    /// <summary>
    /// This event fires when the entities health has been changed. Either increased or decreased.
    /// </summary>
    public event Action<float> OnHealthChanged;
    public event Action<Spell> OnSpellCast;
    /// <summary>
    /// This event fires when elemental cost has been subtracted from the Entity. The passed in parameter represents the amount subtracted.
    /// </summary>
    public event Action<ElementalStats> OnElementalSubtract;
    /// <summary>
    /// This event fires when an elemental spell has been cast on the entity
    /// </summary>
    public event Action<Entity, Spell> OnElementalSpellCastOnEntity;
    /// <summary>
    /// Much like ElementalSpellCastOnEntity this event fires when an elemental spell has been cast on an entity but results in the death of the entity
    /// </summary>
    public event Action<Entity, Spell> OnElementalDeathSpellCastOnEntity;
    /// <summary>
    /// Fires when the entities minimum health clamp is hit. This is not fired if the health clamp is 0.
    /// </summary>
    public event Action OnMinimumHealthClampHit;
    /// <summary>
    /// This fires when the Entity has been reset. This fires essentially anytime the entity is created from the pool. or
    /// reset by other means.
    /// </summary>
    public event Action<Entity> OnEntityReset;
    #endregion

    #region Properties

    private Dictionary<string, Timer> SpellMarkers { get; set; }

    public bool SpellsIgnoreElementalCost
    {
        get { return _spellsIgnoreElementalCost; }
        set { _spellsIgnoreElementalCost = value; }
    }

    public FactionFlags EntityFactionFlags
    {
        get { return _factionFlags; }
        set { _factionFlags = value; }
    }

    public float CurrentSpeed
    {
        get { return _currentSpeed; }
        set { _currentSpeed = value; }
    }

    public string EntityName
    {
        get { return _entityName; }
    }

    public string EntityID
    {
        get { return _entityID; }
        set { _entityID = value; }
    }

    /// <summary>
    /// The timer that controls whether or not enough time has passed to be able to recast a spellID again. This takes in spellcastdelay and will
    /// allow a spellrecast once that time has passed.
    /// </summary>
    public Timer SpellCastTimer
    {
        get;
        set;
    }

    /// <summary>
    /// A normalised value of the Entity's health. This only returns a value
    /// and cannot be set. If you need to set the health do so through the CurrentHP property.
    /// </summary>
    public float CurrentHealthNormalised
    {
        get { return CurrentHp / StatHandler.MaxHp; }
    }

    /// <summary>
    /// Specified by the entity if it wants to keep the beam open
    /// </summary>
    public bool KeepBeamOpen
    {
        get;
        set;
    }
    /// <summary>
    /// Returns true if this beam should be kept open
    /// </summary>
    public bool CanOpenBeam
    {
        get
        {
            if (LivingState != EntityLivingState.Alive || !KeepBeamOpen)
                return false;
            else
                return true;
        }
    }
    /// <summary>
    /// This is the current living state of the Entity. Setting this will also properly
    /// handle setting up the state of the Entity to match the living state.
    /// </summary>
    public EntityLivingState LivingState
    {
        get { return _livingState; }
        set
        {
            switch (value)
            {
                case EntityLivingState.Dead:
                    if (OnKilled != null)
                        OnKilled(this);
                    // Disable Colliders
                    foreach (var coll in GetComponents<Collider>())
                    {
                        coll.enabled = false;
                    }
                    // Remove all spell markers
                    SpellMarkers.Clear();
                    break;
                case EntityLivingState.Alive:
                    //    EntityManager.Instance.AddEntity(this);
                    // Ensure colliders are enabled
                    foreach (var coll in GetComponents<Collider>())
                    {
                        coll.enabled = true;
                    }
                    break;
            }
            _livingState = value;
        }
    }
    /// <summary>
    /// This returns the current HP of the Enity. It will also properly handle setting the HP based on the
    /// Entity's state.
    /// </summary>
    public float CurrentHp
    {
        get { return _currentHp; }
        set
        {
            if (_isInvincible)
                return;
            float oldHealth = _currentHp;
            _currentHp = Mathf.Clamp(value, _minHealthClamp, StatHandler.MaxHp);
            // If the Entity has not been initialised then don't perform game specific logic. 
            // Consider setting the health value more as a initial setup.
            // This means objects such as hit text won't register the event which we don't want them
            // to initially do.
            if (!HasStarted)
            {
                return;
            }
            // Check if the health clamp was hit by comparing the old health
            // and seeing if it was different to the min health clamp.
            if (_currentHp == _minHealthClamp && oldHealth != _minHealthClamp)
            {
                if (OnMinimumHealthClampHit != null)
                {
                    OnMinimumHealthClampHit();
                }
            }
            if (oldHealth != _currentHp && (HitText == null || HitText.ShouldDropReference))
            {
                HitText = HitTextPool.Instance.GetObjectFromPool((obj) =>
                {
                    obj.Owner = this;
                });
            }
            if (HitText != null)
            {
                HitText.HitAmount += _currentHp - oldHealth;
            }
            if (OnHealthChanged != null)
                OnHealthChanged(_currentHp - oldHealth);
            if (_currentHp == 0f && LivingState == EntityLivingState.Alive)
                Die();
        }
    }

    public ParticleSystem HitParticles
    {
        get;
        set;
    }

    public ParticleSystem DeathParticles
    {
        get;
        set;
    }

    public ElementalStats CurrentElementalCharge
    {
        get
        {
            ElementalStats em = ElementalStats.Zero;
            foreach (Element e in Enum.GetValues(typeof(Element)))
            {
                em[e] = (UnlockedElements.Has(e) ? _currentElementalCharge[e] : 0f);
            }
            return em;
        }
        set
        {
            ElementalStats es = ElementalStats.Zero;
            foreach (Element e in Enum.GetValues(typeof(Element)))
            {
                es[e] = Mathf.Clamp(value[e], 0, StatHandler.MaxElementalCharge[e]);
            }
            _currentElementalCharge = es;
        }
    }
    public bool IsInvincible
    {
        get { return _isInvincible; }
        set { _isInvincible = value; }
    }

    public EntityInfoGUI HealthBar { get; set; }
    /// <summary>
    /// If true the Entity should be enabled whenever possible. Enabling an entity depends on factors controlled by the EntityManager.
    /// Factors such as distance play a part in determaining whether or not an entity is eligible to be activated. If they it is 
    /// eligible this value will determain whether or not the Entity should be enabled.
    /// </summary>
    public bool ShouldTryEnableEntity { get; set; }
    /// <summary>
    /// Reference to the Behaviour Tree associated with this Entity if one exists.
    /// </summary>
    public BehaviorTree EntityBehaviorTree { get; private set; }
    /// <summary>
    /// If true this will ignore the disable distance of an Entity controlled in the Entity Manager
    /// </summary>
    public bool IgnoreDisableDistance { get { return _ignoreDisableDistance; } }
    /// <summary>
    /// A reference to the Entity Path Finding interface if one exists
    /// </summary>
    public IPathFinder EntityPathFinder { get; private set; }
    /// <summary>
    /// Reference to the Enity Stat Handler
    /// </summary>
    public EntityStatHandler StatHandler { get; private set; }
    /// <summary>
    /// Reference to the Entity Level Handler.
    /// </summary>
    public EntityLevelHandler LevelHandler { get { return _levelHandler; } }
    /// <summary>
    /// This is a reference to the entity that has killed this entity. Assuming this Entity is dead of course.
    /// </summary>
    public Entity KilledBy { get; set; }

    /// <summary>
    /// Recharge lock on each element based on time i.e fire = 1 is a lock of 1 second on fire recharge
    /// </summary>
    public ElementalStats RechargeLock
    {
        get { return _rechargeLock; }
        set { _rechargeLock = value; }
    }
    /// <summary>
    /// A transform local point indicating where the health point should be positioned to.
    /// </summary>
    public Transform GUIHealthPoint { get { return _guiHealthPoint; } set { _guiHealthPoint = value; } }
    /// <summary>
    /// A transform local point indicating where the speech bubble should be positioned to.
    /// </summary>
    public Transform GUISpeechPoint { get { return _guiSpeechPoint; } set { _guiSpeechPoint = value; } }
    /// <summary>
    /// A reference to the hit text associated with the Entity. The hit text display numbers depending on the health change of the entity.
    /// </summary>
    public HitText HitText { get; set; }
    public DeathController DeathController { get; set; }
    /// <summary>
    /// This class acts as a proxy to the speech bubble. Since speech bubbles are pooled and registering to a specific speech 
    /// bubble may lead to undesired behaviours upon a repool this class acts as a tangible means to register to specific
    /// entity and control the speech bubble associated with them.
    /// </summary>
    public EntitySpeechBubbleProxy SpeechBubbleProxy { get; private set; }
    /// <summary>
    /// Returns true if the Entity has started properly.
    /// </summary>
    public bool HasStarted { get; private set; }
    /// <summary>
    /// Returns true if the Entity has initialised properly
    /// </summary>
    public bool HasInitialised { get; private set; }
    public EntityCastPoint EntityCastPoint
    {
        get { return _entityCastPoint; }
        set { _entityCastPoint = value; }
    }
    public string PoolID
    {
        get { return EntityID; }
    }
    public EntityMetaInfo EntityMetaInfo { get; set; }
    public bool HasBeenAddedToEntityManager { get { return EntityMetaInfo.Has(EntityMetaInfo.IsAddedToEntityManager); } }
    public IAnimatorController EntityAnimator { get; set; }
    public SpellDeathMarker SpellDeathMarker { get; set; }
    /// <summary>
    /// These are the elements that the Entity has unlocked and is able to use.
    /// Assuming ignoring spell cost is not enabled this will determain whether or not
    /// an Entity is able to cast specific spells. An Element that is not unlocked will
    /// always return a resource value of 0.
    /// </summary>
    public Element UnlockedElements { get; set; }

    #endregion

    #region Initialization

    public void Initialise()
    {
        // Add All Elements
        foreach (Element e in Enum.GetValues(typeof(Element)))
        {
            UnlockedElements = UnlockedElements.Add(e);
        }
        SpeechBubbleProxy = new EntitySpeechBubbleProxy(this);
        StatHandler = new EntityStatHandler(this);
        LevelHandler.Owner = this;
        SpellMarkers = new Dictionary<string, Timer>();
        _audio = GetComponent<AudioSource>();
        _updateSpeedTimer = new Timer(UnityEngine.Random.Range(0.15f, 0.35f));
        EntityBehaviorTree = GetComponent<BehaviorTree>();
        SpellCastTimer = new Timer(0f);
        foreach (var component in GetComponents<Component>())
        {
            if (component is IPathFinder)
            {
                EntityPathFinder = component as IPathFinder;
            }
        }
        DeathController = GetComponent<DeathController>();

        // Register health changed event for particles and such
        OnHealthChanged += HealthChanged;
        _audioPlayTimer = new Timer(0.25f);

        if (loadHealthBar)
        {
            HealthBar = Instantiate(GameMainReferences.Instance.GameConfigInfo.EntityHealthBarPrefab);
            HealthBar.Entity = this;
            HealthBar.gameObject.SetActive(true);
            //  HealthBar.transform.SetParent(transform);
        }

        if (loadMinimapIcon)
        {
            var miniMapIcon = Instantiate(GameMainReferences.Instance.GameConfigInfo.EntityMinimapIcon);
            miniMapIcon.Entity = this;
            // Ensure it is active
            miniMapIcon.gameObject.SetActive(true);
        }
        _entityComponents = gameObject.GetComponents<EntityComponent>().ToList();
        foreach (var comp in _entityComponents)
        {
            comp.Initialise();
        }
        // TODO Figure out the animation system a bit better
        // For now all entities should have a HumanoidAnimatorController
        // on the base object
        EntityAnimator = GetComponent<HumanoidAnimatorController>();
        ShouldTryEnableEntity = true;
        HasInitialised = true;
    }

    public void PoolStart()
    {
        ResetEntity();
    }
    /// <summary>
    /// Resets the Enity back to starting values.
    /// </summary>
    public void ResetEntity()
    {
        SpellMarkers.Clear();
        SpeechBubbleProxy.Reset();
        SpellCastTimer.Reset();
        LivingState = EntityLivingState.Alive;
        // Ensure the stat handler is reset first
        StatHandler.RemoveAllStatModifiers();
        // Ensure the entity and it's stats are updated for it's current level
        LevelHandler.UpdateEntityStats();
        // Get the initial last position so we can workout how fast the entity is moving
        _lastPosition = transform.position;
        // Set the current elemental charge to the max elemental charge.
        CurrentElementalCharge = StatHandler.MaxElementalCharge;
        // Ensure HP is properly clamped
        CurrentHp = StatHandler.MaxHp;
        // Trigger an update of the component stats once everything has been set
        StatHandler.UpdateStatComponents();
        _audioPlayTimer.Reset();

        foreach (var comp in _entityComponents)
        {
            comp.OnStart();
        }
        HasStarted = true;
        if (OnEntityReset != null)
        {
            OnEntityReset(this);
        }
    }

    public void Recycle()
    {
        HasStarted = false;
    }

    #endregion

    #region Event Listeners


    #endregion

    #region Updates
    protected void Update()
    {
        DoUpdate();
        UpdateSpellMarkers();
    }

    private void UpdateSpellMarkers()
    {
        var removals = (from r in SpellMarkers where r.Value.CanTick select r.Key).ToList<String>();
        foreach (var r in removals)
        {
            SpellMarkers.Remove(r);
        }
    }

    /// <summary>
    /// Performs all Entity specific updates
    /// </summary>
    private void DoUpdate()
    {
        switch (LivingState)
        {
            case EntityLivingState.Alive:
                LivingUpdate();
                break;
            case EntityLivingState.Dead:
                DeadUpdate();
                break;
        }
    }

    /// <summary>
    /// Called while the entity is Living
    /// </summary>
    protected virtual void LivingUpdate()
    {
        // Debug Living update to make sure it is always added to the Entity Manager when living
#if UNITY_EDITOR
        if (!EntityMetaInfo.Has<EntityMetaInfo>(global::EntityMetaInfo.IsAddedToEntityManager))
        {
            Debug.LogError(gameObject + " is living but has not been added to the EntityManager");
        }
#endif

        UpdateSpeed();

        if (!SpellsIgnoreElementalCost)
        {
            UpdateRechargeLock();
            CurrentElementalCharge += StatHandler.RechargeRate * Time.deltaTime;
        }
    }
    /// <summary>
    /// Called while the entity is Dead
    /// </summary>
    protected virtual void DeadUpdate() { }

    #endregion

    #region State And Value Changes
    /// <summary>
    /// This should be called to pool an Entity rather than directly pooling it via EntityPool.
    /// This ensures the Entity is in the correct state and avoids annoying bugs relating to entity pooling.
    /// </summary>
    public void PoolEntity()
    {
        EntityManager.Instance.RemoveEntity(this);
        EntityPool.Instance.PoolObject(this);
    }
    /// <summary>
    /// Adjusts the health of the entity ensuring that the amount passed in is a whole number
    /// </summary>
    /// <param name="amount"></param>
    public void AdjustHealthByAmount(float amount)
    {
        amount = Mathf.Floor(amount);
        CurrentHp += amount;
    }
    /// <summary>
    /// Updates the current speed the Entity is moving at. This is useful for animation.
    /// </summary>
    private void UpdateSpeed()
    {
        var moveAmount = transform.position - _lastPosition;
        _lastPosition = transform.position;
        CurrentSpeed = moveAmount.magnitude / Time.deltaTime;
        // Normalise speed
        //  CurrentSpeed /= 5f;
    }

    #endregion

    #region Entity Events

    protected virtual void HealthChanged(float amount)
    {
        if (amount < 0 && HitParticles != null && UnityEngine.Random.Range(0, 2) == 0)
        {
            HitParticles.Emit(10);
        }
    }

    /// <summary>
    /// Called when the entity should die. 
    /// </summary>
    public void Die()
    {
        LivingState = EntityLivingState.Dead;
        if (DeathParticles != null)
            DeathParticles.Emit(UnityEngine.Random.Range(5, 25));
        PlaySound(_deathAudio);
        EntityManager.Instance.TriggerEntityKilled(this);
    }

    #endregion

    #region Spells

    public bool HasSpellMarker(string spellID)
    {
        return SpellMarkers.ContainsKey(spellID);
    }

    public void AddSpellMarker(string spellID, float removeTime)
    {
        if (!SpellMarkers.ContainsKey(spellID))
            SpellMarkers.Add(spellID, new Timer(removeTime));
    }

    public void RemoveSpellMarker(string spellID)
    {
        if (SpellMarkers.ContainsKey(spellID))
            SpellMarkers.Remove(spellID);
    }

    public void ApplyElementalSpell(ElementalApply elementalSpell)
    {
        bool wasLiving = LivingState == EntityLivingState.Alive;

        foreach (Element e in Enum.GetValues(typeof(Element)))
        {
            AdjustHealthByAmount((elementalSpell.ElementalPower[e] * elementalSpell.effectSetting.spell.CastingEntity.StatHandler.SpellElementalModifier[e]) * -StatHandler.ElementalResistance[e]);
        }

        // A elemental spell has been cast on the entiy, fire the appropriate event
        if (OnElementalSpellCastOnEntity != null)
            OnElementalSpellCastOnEntity(elementalSpell.effectSetting.spell.CastingEntity, elementalSpell.effectSetting.spell);

        // A elemental spell has been cast on the entity that  has lead to it's death. 
        // Fire an event that informs it subscribers that it was this spell that has killed the Entity.
        if (wasLiving && LivingState == EntityLivingState.Dead)
        {
            if (OnElementalDeathSpellCastOnEntity != null)
                OnElementalDeathSpellCastOnEntity(elementalSpell.effectSetting.spell.CastingEntity,
                    elementalSpell.effectSetting.spell);

            KilledBy = elementalSpell.effectSetting.spell.CastingEntity;
            SpellDeathMarker = elementalSpell.effectSetting.spell.SpellDeathMarker;
        }

    }

    public bool CastSpell(Spell spell, bool ignoreSpellCost = false)
    {
        Spell ta;
        return CastSpell(spell.SpellID, out ta, ignoreSpellCost);
    }

    public bool CastSpell(Spell spell, Vector3? spellTargetPosition, bool ignoreSpellCost)
    {
        Spell ta;
        return CastSpell(spell, out ta, null, spellTargetPosition, ignoreSpellCost);
    }

    public bool CastSpell(string spellId, bool ignoreSpellCost = false)
    {
        Spell ta;
        return CastSpell(spellId, out ta, ignoreSpellCost);
    }

    public bool CastSpell(string spell, out Spell castSpell, bool ignoreSpellCost = false)
    {
        return CastSpell(SpellList.Instance.GetSpell(spell), out castSpell, null, null, ignoreSpellCost);
    }
    /// <summary>
    /// This creates a new spell that the Entity cast itself. This mean the spell will recognise this Enity
    /// as the spell caster. It ensures that the Entity can only cast the spell if possible and returns if it cannot.
    /// It also handles the casting of different types of spells. For example if an attached spell is cast it will 
    /// attach it appropriately 
    /// </summary>
    /// <param name="spell"></param>
    /// <param name="castSpell"></param>
    /// <param name="spellTarget"></param>
    /// <param name="spellTargetPosition"></param>
    /// <returns></returns>
    public bool CastSpell(Spell spell, out Spell castSpell, Transform spellTarget = null, Vector3? spellTargetPosition = null, bool ignoreSpellCost = false)
    {
        if (!CanCastSpell(spell, ignoreSpellCost))
        {
            castSpell = null;
            return false;
        }

        // If we are attaching a spell ensure the spell target is set to this transform.
        if (spell.SpellType == SpellType.Attached)
            spellTarget = transform;

        Spell sp = null;
        // Freely create a new spell if the cast method is FreeCast. If it is not free cast and the Entity
        // has no single instance spells of this type then also create a new spell. If it is not freecast and therefore
        // a single instance spell it must be of type SingleInstanceRefresh otherwise it would not have been allowed to cast.
        // In that case refresh the spell.
        // Also note that if this is a single instance spell and it has been added already we would not have got to this point.
        // The Method CanCastSpell checks for this scenario and will return false before we get to this code block.
        if (spell.SpellCastMethod == SpellCastMethod.FreeCast || (!HasSingleInstanceSpell(spell)))
        {
            sp = CreateNewSpellFromEntity(spell, spellTarget, spellTargetPosition);
        }
        else
        {
            RefreshSingleInstanceSpell(spell);
        }

        // Take spell cost
        if (!ignoreSpellCost)
        {
            SubtractSpellCost(spell);
        }
        castSpell = sp;
        SpellCastTimer.TickTime = spell.SpellCastDelay;
        SpellCastTimer.Reset();
        //sPlaySound(spell.castAudio);
        if (OnSpellCast != null)
            OnSpellCast(sp);
        return true;
    }

    private Spell CreateNewSpellFromEntity(Spell spell, Transform spellTarget, Vector3? spellTargetPosition)
    {
        Spell sp = SpellList.Instance.GetNewSpell(spell);
        sp.CastSpell(this, EntityCastPoint.CastPoint, null, spellTarget, spellTargetPosition);

        switch (sp.SpellType)
        {
            case SpellType.Attached:
                AttachSpell(sp);
                break;
        }
        // If a single instance spell is created for the first time.
        AddSingleInstanceSpell(sp);
        return sp;
    }

    public void SubtractSpellCost(Spell spell)
    {
        SubtractElementCost(spell.ElementalCost);
    }

    public void SubtractElementCost(ElementalStats element)
    {
        if (SpellsIgnoreElementalCost)
            return;

        CurrentElementalCharge -= element;
        SetRechargeLock(element);
        if (OnElementalSubtract != null)
        {
            OnElementalSubtract(element);
        }
    }

    public bool CanCastSpell(string spell, bool ignoreSpellCost = false)
    {
        return CanCastSpell(SpellList.Instance.GetSpell(spell), ignoreSpellCost);
    }
    /// <summary>
    /// Check if the entity can cast the desired spell. Takes into account the various state of the entity such as 
    /// elemental charge, spell cast time, living state etc
    /// </summary>
    /// <param name="spell"></param>
    /// <returns></returns>
    public bool CanCastSpell(Spell spell, bool ignoreSpellCost = false)
    {
        // Check other spell cast conditions. If any evalulate at true return false.
        // Also check if the spell is an attached spell and if the spell can refresh.
        // Check if a single instance of the spell exists. If this spell is not a single instance spell,
        // then it will not appear in the spell single instance dictionary. If it does have an instance
        // and it is a SingleInstanceRefresh spell then we can cast this spell to allow for refreshing.
        if (!SpellCastTimer.CanTick || LivingState != EntityLivingState.Alive || (HasSingleInstanceSpell(spell) && spell.SpellCastMethod == SpellCastMethod.SingleInstance))
            return false;
        if (ignoreSpellCost)
        {
            return true;
        }
        return HasElementalChargeToCast(spell);
    }
    /// <summary>
    /// Check if the entity has enough elemental charge to cast the desired spell
    /// </summary>
    /// <param name="spell"></param>
    /// <returns></returns>
    public bool HasElementalChargeToCast(Spell spell)
    {
        if (SpellsIgnoreElementalCost)
            return true;

        foreach (Element e in Enum.GetValues(typeof(Element)))
        {
            if (CurrentElementalCharge[e] < spell.ElementalCost[e])
                return false;
        }
        return true;
    }

    public void AddSingleInstanceSpell(Spell spell)
    {
        if (spell.SpellCastMethod == SpellCastMethod.SingleInstance || spell.SpellCastMethod == SpellCastMethod.SingleInstanceRefresh)
        {
            if (!HasSingleInstanceSpell(spell))
            {
                _singleInstanceSpells.Add(spell.SpellID, spell);
                spell.OnSpellDestroy += RemoveSingleInstanceSpell;
            }
        }
    }

    public bool HasSingleInstanceSpell(Spell spell)
    {
        return HasSingleInstanceSpell(spell.SpellID);
    }

    public bool HasSingleInstanceSpell(string spellId)
    {
        return _singleInstanceSpells.ContainsKey(spellId);
    }
    /// <summary>
    /// This will attach a spell to this entity. 
    /// </summary>
    /// <param name="spell"></param>
    public void AttachSpell(Spell spell)
    {
        if (spell.SpellType != SpellType.Attached)
        {
            Debug.LogError("Spell: " + spell + " is not set as an attach type and is trying to be attached to: " + gameObject);
        }
        if (HasSingleInstanceSpell(spell))
            Debug.LogError("Spell already contained and should not be attached!");
        //    _singleInstanceSpells.Add(spell.SpellID, spell);
        spell.transform.parent = transform;
        spell.transform.position = transform.position;
        //   spell.OnSpellDestroy += RemoveSingleInstanceSpell;
    }

    public void RefreshSingleInstanceSpell(Spell spell)
    {
        if (!HasSingleInstanceSpell(spell))
        {
            Debug.LogError("Single Instance Spell does not exist!");
            return;
        }
        // No point in checking for this outside of debugging purposes
#if UNITY_EDITOR
        if (_singleInstanceSpells[spell.SpellID].SpellKillMethod == SpellKillMethod.KeepAlive || _singleInstanceSpells[spell.SpellID].SpellKillMethod == SpellKillMethod.Triggered)
        {
            Debug.LogError("Trying to refresh spell that is not of a refreshable type. Spell: " + spell.SpellID);
        }
#endif

        _singleInstanceSpells[spell.SpellID].TriggerSpellRefresh();
    }

    private void RemoveSingleInstanceSpell(Spell spell)
    {
        _singleInstanceSpells.Remove(spell.SpellID);
    }

    #endregion


    #region Helper Methods

    public void LookAt(Vector3 target)
    {
        target.y = transform.position.y;
        transform.rotation = Quaternion.LookRotation(target - transform.position, Vector3.up);
    }

    public void UpdateRechargeLock()
    {
        foreach (Element e in Enum.GetValues(typeof(Element)))
        {
            _rechargeLock[e] = Mathf.MoveTowards(RechargeLock[e], 1f, Time.deltaTime);
        }
    }

    /// <summary>
    /// Set the recharge lock to lock a spellID if a passed in stat is greater than 0
    /// </summary>
    /// <param name="stats"></param>
    public void SetRechargeLock(ElementalStats stats)
    {
        foreach (Element e in Enum.GetValues(typeof(Element)))
        {
            // Setting recharge lock to 0 will cause the element of that type not to be updated
            _rechargeLock[e] = stats[e] > 0 ? 0 : _rechargeLock[e];
        }
    }

    private void PlaySound(AudioClip audioClip)
    {
        if (audioClip != null && _audioPlayTimer.CanTickAndReset())
            _audio.PlayOneShot(audioClip);
    }

    #endregion

    #region Entity Relations

    public bool IsEnemy(Entity other)
    {
        // If the other entity should be ignored
        if ((EntityFactionFlags & FactionFlags.Ignore) == FactionFlags.Ignore || (other.EntityFactionFlags & FactionFlags.Ignore) == FactionFlags.Ignore)
        {
            return false;
        }
        if ((other.EntityFactionFlags & EntityFactionFlags) == 0)
            return true;
        else
            return false;
    }

    #endregion

    private void Reset()
    {
        gameObject.layer = LayerMask.NameToLayer("Entity");
    }

}

public class EntityStatHandler
{

    private Entity _owner;

    public EntityStatHandler(Entity owner)
    {
        this._owner = owner;
        EntityStatModifiers = new Dictionary<string, List<EntityStats>>();
    }

    public float Speed
    {
        get
        {
            return CachedStats.Speed;
        }
    }

    /// <summary>
    /// This is the maximum HP of the Enity.
    /// </summary>
    public float MaxHp
    {
        get { return CachedStats.MaxHp; }
    }

    public ElementalStats SpellElementalModifier
    {
        get { return CachedStats.SpellElementalModifier; }
    }

    public ElementalStats MaxElementalCharge { get { return CachedStats.MaxElementalCharge; } }

    public ElementalStats ElementalResistance { get { return CachedStats.ElementalResistance; } }

    /// <summary>
    /// The modifiers such as Debuffs or Buffs that are modifying the Entity's stats. It is associated with a spell and uses
    /// the spell ID to look up the spell. Each spell may apply many different stats depending if the spell is stackable or not. 
    /// When the spell is removed it iterates through all the Modifiers associated with the spell and removes them.
    /// </summary>
    public Dictionary<string, List<EntityStats>> EntityStatModifiers { get; private set; }

    /// <summary>
    /// This is the recharge rate of the Entity's elements. It will return the value
    /// that the Entity should increment it's current charge, taking into account if the 
    /// respective element is locked or not.
    /// </summary>
    public ElementalStats RechargeRate
    {
        get
        {
            ElementalStats et = ElementalStats.Zero;
            foreach (Element e in Enum.GetValues(typeof(Element)))
            {
                et[e] = _owner.RechargeLock[e] == 1 ? 1 : 0;
            }
            return CachedStats.RechargeRate * et;
        }
    }

    public EntityStats CachedStats { get; private set; }

    /// <summary>
    /// Modifies the EntityStats to add the amount passed in. It will automatically update the state of the Entity to reflect
    /// the new stats.
    /// </summary>
    /// <param name="stat"></param>
    public void ApplyStatModifier(string id, EntityStats stat)
    {
        DoAddApplyStatModifier(id, stat);
        // Update the stats of the Entity to reflect the new stats
        UpdateStatComponents();
    }
    /// <summary>
    /// Removes all EnityStats applied using the specified ID. It will automatically update the state of the Entity to reflect
    /// the new stats.
    /// </summary>
    /// <param name="stat"></param>
    public void RemoveAllStatModifiers(string id)
    {
        DoRemoveAllStatModifiers(id);
        // Update the stats of the Entity to reflect the new stats
        UpdateStatComponents();
    }
    /// <summary>
    /// This will remove all stats and insert the new stat before updating the stat components
    /// </summary>
    /// <param name="id"></param>
    /// <param name="stats"></param>
    public void UpdateStatModifiers(string id, EntityStats stats)
    {
        UpdateStatModifiers(id, stats, true);
    }
    /// <summary>
    /// This will remove all stats and insert the new stat. Specifying true or false to the updateComponents argument will trigger an automatic update
    /// of updating the stats.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="stats"></param>
    /// <param name="updateComponents"></param>
    public void UpdateStatModifiers(string id, EntityStats stats, bool updateComponents)
    {
        DoRemoveAllStatModifiers(id);
        DoAddApplyStatModifier(id, stats);
        if (updateComponents)
            UpdateStatComponents();
    }

    private void DoAddApplyStatModifier(string id, EntityStats stat)
    {
        if (!EntityStatModifiers.ContainsKey(id))
        {
            EntityStatModifiers.Add(id, new List<EntityStats>());
        }
        EntityStatModifiers[id].Add(stat);
        // Apply the stat to the cached stats
        CachedStats += stat;
    }

    private void DoRemoveAllStatModifiers(string id)
    {
        if (!EntityStatModifiers.ContainsKey(id))
        {
            return;
        }
        else
        {
            // Iterate through all EntityStats and remove them from the cached stats
            foreach (EntityStats s in EntityStatModifiers[id])
            {
                CachedStats = CachedStats.Difference(s);
            }
            // Clear the list
            EntityStatModifiers[id].Clear();
        }
    }

    /// <summary>
    /// Updates the Entity to match the cached stats
    /// </summary>
    public void UpdateStatComponents()
    {
        if (_owner.EntityPathFinder != null)
        {
            _owner.EntityPathFinder.Speed = Mathf.Max(CachedStats.Speed, 1f);
        }
        // Ensure the Current Hp is properly clamped
        _owner.CurrentHp = _owner.CurrentHp;
    }
    /// <summary>
    /// Use this to reset all stats in the stat handler
    /// </summary>
    public void RemoveAllStatModifiers()
    {
        foreach (string key in EntityStatModifiers.Keys)
        {
            DoRemoveAllStatModifiers(key);
        }
    }

}

public class EntityEventArgs : EventArgs
{
    public Entity Entity;

    public EntityEventArgs(Entity entity)
    {
        this.Entity = entity;
    }
}

[Flags]
public enum FactionFlags
{
    One = 1 << 0,
    Two = 1 << 1,
    Three = 1 << 2,
    Four = 1 << 3,
    Ignore = 1 << 4
}
[Flags]
public enum EntityMetaInfo
{
    IsAddedToEntityManager = 1
}
#region Enums

public enum EntityLivingState
{
    Alive,
    Dead
}

#endregion

public class EntitySpeechBubbleProxy
{
    private Entity _owner;
    private EntitySpeechBubble _speechBubble;
    public event Action OnFinishedPlaying;
    public event Action OnFinishedTalking;

    public EntitySpeechBubbleProxy(Entity owner)
    {
        this._owner = owner;
    }

    public EntitySpeechBubble SpeechBubble
    {
        get { return _speechBubble; }
        set
        {
            _speechBubble = value;
            if (value == null)
            {
                return;
            }
            value.Entity = _owner;
            // When the speech bubble has finished playing remove it from the Entity
            value.OnFinishedPlaying += PlayingAction;
            value.OnFinishedTalking += TalkingAction;
        }
    }

    private void PlayingAction()
    {
        SpeechBubble.OnFinishedPlaying -= PlayingAction;
        SpeechBubble.OnFinishedTalking -= TalkingAction;
        if (OnFinishedPlaying != null)
        {
            OnFinishedPlaying();
        }
        SpeechBubble = null;
    }

    private void TalkingAction()
    {
        // value.OnFinishedTalking -= talkingAction;
        if (OnFinishedTalking != null)
        {
            OnFinishedTalking();
        }
    }

    public void QueueText(string text)
    {
        if (SpeechBubble == null)
        {
            SpeechBubble = SpeechBubblePool.Instance.GetObjectFromPool();
        }
        SpeechBubble.QueueText(text);
    }

    public void Reset()
    {
        if (SpeechBubble != null)
        {
            SpeechBubble.OnFinishedPlaying -= PlayingAction;
            SpeechBubble.OnFinishedTalking -= TalkingAction;
        }
        SpeechBubble = null;
    }
}
