using System;
using UnityEngine;
using System.Collections;

public class ObjectTransitionFx : MonoBehaviour, IPoolableID
{
    [SerializeField]
    private string _transitionID = "NOTSET";
    [SerializeField]
    [Tooltip("The time delay that will occur before the targeted transition begins")]
    private float _timeDelay;
    [SerializeField]
    private bool _randomDelay;
    [SerializeField]
    private float _minDelay;
    [SerializeField]
    private float _maxDelay;
    [Tooltip("How long must pass before the transition is automatically repooled")]
    [SerializeField]
    private float _repoolDelay = 1f;

    private Timer _triggerTimer;
    private Timer _repoolTimer;
    private ObjectTransitionComponent[] _transitionComponents;

    //Use Events to not iterate over components when they don't listen for the specific event
    public event Action OnCompleted;
    public event Action OnInitialise;
    public event Action OnStart;
    public event Action OnPrePool;

    public enum TransitionMethod
    {
        Activate,
        Deactivate,
        PoolEntity
    }

    public GameObject TargetObject { get; set; }
    public bool IsTransitionComplete { get; set; }
    public TransitionMethod FXTransitionMethod { get; set; }

    public void Initialise()
    {
        if (!_randomDelay)
        {
            _triggerTimer = new Timer(_timeDelay);
        }
        else
        {
            _triggerTimer = UnityEngine.Random.Range(_minDelay, _maxDelay);
        }
        _repoolTimer = new Timer(_repoolDelay);
        // Get all the transition components
        _transitionComponents = GetComponentsInChildren<ObjectTransitionComponent>(true);

        foreach (var comp in _transitionComponents)
        {
            comp.TriggerInitialise(this);
        }
        // Fire init event
        if (OnInitialise != null)
        {
            OnInitialise();
        }
    }

    public void PoolStart()
    {
        _triggerTimer.Reset();
        _repoolTimer.Reset();
        IsTransitionComplete = false;
        transform.position = TargetObject.transform.position;
        foreach (var comp in _transitionComponents)
        {
            comp.TriggerStart();
        }
        if (OnStart != null)
        {
            OnStart();
        }
    }

    public void Recycle()
    {
        TargetObject = null;
        // Unsubscribe all events
        OnInitialise = null;
        OnCompleted = null;
        OnPrePool = null;
        OnStart = null;
    }

    public string PoolID
    {
        get { return _transitionID; }
    }

    private void Update()
    {
        if (!IsTransitionComplete && _triggerTimer.CanTick)
        {
            DoTransition();
        }
        if (IsTransitionComplete && _repoolTimer.CanTick)
        {
            DoRepool();
        }
    }

    private void DoTransition()
    {
        Entity ent = TargetObject.GetComponent<Entity>();

        switch (FXTransitionMethod)
        {
            case TransitionMethod.Activate:
                if (ent != null)
                {
                    ent.ShouldTryEnableEntity = true;
                    EntityManager.Instance.AddEntity(ent);
                }
                else
                {
                    TargetObject.SetActive(true);
                }
                break;
            case TransitionMethod.Deactivate:
                if (ent != null)
                {
                    ent.ShouldTryEnableEntity = false;
                    EntityManager.Instance.RemoveEntity(ent);
                }
                TargetObject.SetActive(false);
                break;
            case TransitionMethod.PoolEntity:
                if (ent != null)
                {
                    // Ensure the Entity is removed
                    // Check if the Entity is in the pool
                    if (ent.HasBeenAddedToEntityManager)
                    {
                        EntityManager.Instance.RemoveEntity(ent);
                    }
                    EntityPool.Instance.PoolObject(ent);
                }
                break;
        }
        _repoolTimer.Reset();
        TriggerTransitionComplete();
    }

    // Will force the transition and all the components to trigger a completion
    public void TriggerTransitionComplete()
    {
        IsTransitionComplete = true;
        foreach (var comp in _transitionComponents)
        {
            comp.TriggerComplete();
        }
        if (OnCompleted != null)
        {
            OnCompleted();
        }
    }

    private void DoRepool()
    {
        foreach (var comp in _transitionComponents)
        {
            comp.TriggerPrePool();
        }
        if (OnPrePool != null)
        {
            OnPrePool();
        }
        ObjectTransitionPool.Instance.PoolObject(this);
    }
}
