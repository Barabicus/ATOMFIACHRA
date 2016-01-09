using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class GeneralEventCore : MonoBehaviour
{
    [SerializeField]
    private bool _awakeEvents;
    [SerializeField]
    private bool _startEvents;
    [SerializeField]
    private bool _playerBoundsEvents;
    [SerializeField]
    private bool _updateEvents;

    [SerializeField]
    private BoundsCheckMethod _boundsCheckMethod = BoundsCheckMethod.Box;
    [SerializeField]
    private float _boundsRadius = 10f;
    [SerializeField]
    private Vector3 _boundsSize = new Vector3(10, 10, 10);

    private bool _isPlayerWithinBounds = false;
    private Transform _playerCache;
    private Bounds _bounds;

    private GeneralEventComponent[] _generalEventComponents;

    public enum BoundsCheckMethod
    {
        Radius,
        Box
    }

    private void Awake()
    {
        _generalEventComponents = gameObject.GetComponentsInChildren<GeneralEventComponent>(true);
        if (_playerBoundsEvents && _boundsCheckMethod == BoundsCheckMethod.Box)
        {
            _bounds = new Bounds(transform.position, _boundsSize);
        }
        if (_awakeEvents)
        {
            foreach (var c in _generalEventComponents)
            {
                c.TriggerAwake();
            }
        }
    }

    private void Start()
    {
        _playerCache = GameMainReferences.Instance.PlayerCharacter.transform;
        if (_startEvents)
        {
            foreach (var c in _generalEventComponents)
            {
                c.TriggerStart();
            }
        }
    }

    private void Update()
    {
        if (_playerBoundsEvents)
        {
            DoBoundsCheck();
        }
        if (_updateEvents)
        {
            foreach (var c in _generalEventComponents)
            {
                c.TriggerUpdate();
            }
        }
    }
    /// <summary>
    /// Check if the player is within bounds.
    /// </summary>
    private void DoBoundsCheck()
    {
        switch (_boundsCheckMethod)
        {
            case BoundsCheckMethod.Radius:
                PlayerWithinBoundsCheck(Vector3.Distance(_playerCache.position, transform.position) <= _boundsRadius);
                break;
            case BoundsCheckMethod.Box:
                PlayerWithinBoundsCheck(_bounds.Contains(_playerCache.position));
                break;
        }
    }
    /// <summary>
    /// Checks if the player is within bounds in comparison to if it was or wasnt in the previous update.
    /// Depending on the new value it will trigger the appropriate event.
    /// </summary>
    /// <param name="value"></param>
    private void PlayerWithinBoundsCheck(bool value)
    {
        if (value != _isPlayerWithinBounds)
        {
            foreach (var c in _generalEventComponents)
            {
                if (value)
                    c.TriggerPlayerEnter();
                else
                    c.TriggerPlayerExit();
            }
            _isPlayerWithinBounds = value;
        }
    }

    public void OnDrawGizmosSelected()
    {
        if (_playerBoundsEvents)
        {
            Gizmos.color = Color.green;
            switch (_boundsCheckMethod)
            {
                case BoundsCheckMethod.Box:
                    Gizmos.DrawWireCube(transform.position, _boundsSize);
                    break;
                case BoundsCheckMethod.Radius:
                    Gizmos.DrawWireSphere(transform.position, _boundsRadius);
                    break;
            }
        }
    }
}
