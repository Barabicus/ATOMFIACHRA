using UnityEngine;
using System.Collections;
using System;

public class Barrier : MonoBehaviour
{
    [SerializeField]
    private bool _initialBarrierState;
    private BarrierComponent[] _barrierComponents;
    private bool _isBarrierActive;

    public event Action<bool> OnActiveStateChange;

    /// <summary>
    /// Enable or disable the barrier
    /// </summary>
    public bool BarrierActivityState
    {
        get { return _isBarrierActive; }
        set
        {
            // Ensure the barrier is initialised
            Initialise();

            _isBarrierActive = value;
            if (OnActiveStateChange != null)
            {
                OnActiveStateChange(value);
            }
        }
    }
    public bool HasInitialised { get; private set; }

    private void Awake()
    {
        Initialise();
    }
    /// <summary>
    /// Initialises the barrier. This shoud be setup before any state changes take place.
    /// </summary>
    public void Initialise()
    {
        if (HasInitialised)
        {
            return;
        }
        HasInitialised = true;

        // Get all barrier components
        _barrierComponents = GetComponentsInChildren<BarrierComponent>(true);
        foreach (var b in _barrierComponents)
        {
            b.Owner = this;
            b.TriggerInitialise();
        }
        BarrierActivityState = _initialBarrierState;
    }

}
