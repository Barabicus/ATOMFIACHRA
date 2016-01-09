using System;
using UnityEngine;
using System.Collections;

[SpellCategory("RayCast Motor", SpellEffectCategory.Motor)]
[SpellEffectStandard(false, false, "The raycast motor shoots a ray from the target position in an attempt to find targets. How fast the ray moves, the max distance and the size can all be adjusted. This can make a ray behave like a beam or like a cone. ")]
public class RayCastMotor : SpellMotor
{
    [SerializeField]
    private RayCastMotorMethod _motorMethod;
    [SerializeField]
    private float _tryApplyFrequency = 1f;
    [SerializeField]
    private float _rayDistance = 5f;
    [SerializeField]
    [Tooltip("This is how fast the ray will go until it reaches the rayDistance")]
    private float _raySpeed = 1f;
    [SerializeField]
    [Tooltip("This is the radius of the sphere cast")]
    private float _sphereRadius = 1f;
    [SerializeField]
    [Tooltip("If this is true it will attempt to trigger a collision with every Entity in the way of the ray. If this is false it will only target the first Entity")]
    private bool triggerAll = false;
    [SerializeField]
    [Tooltip("If this is true the raycast motor will set the raycast distance to match that of the object it hits")]
    private bool _actsLikeBeam = false;

    private float _currentDistance;
    private Timer _tryApplyTimer;

    public RayCastMotorMethod MotorMethod { get { return _motorMethod; } set { _motorMethod = value; } }
    public float CurrentRayDistance { get { return _currentDistance; } private set { _currentDistance = value; } }
    public Vector3 CurrentRayPosition { get { return transform.position + (transform.forward * CurrentRayDistance); } }
    public float MaxDistance { get { return _rayDistance; } }

    public enum RayCastMotorMethod
    {
        RayCast,
        CapsuleCast,
        SphereCast
    }

    public override void Initialize(EffectSetting effectSetting)
    {
        base.Initialize(effectSetting);
        _tryApplyTimer = _tryApplyFrequency;
    }

    protected override void OnSpellStart()
    {
        base.OnSpellStart();
        _tryApplyTimer.ForceTickTime();
        CurrentRayDistance = 0f;
    }

    protected override void UpdateSpell()
    {
        base.UpdateSpell();
        UpdateDistance();
        UpdateRayPosition();
        CheckRay();
    }

    private void UpdateDistance()
    {
        CurrentRayDistance = Mathf.Min(CurrentRayDistance + (_raySpeed * Time.deltaTime), _rayDistance);
    }
    private void CheckRay()
    {
        if (_tryApplyTimer.CanTickAndReset())
        {
            switch (MotorMethod)
            {
                case RayCastMotorMethod.RayCast:
                    DoRayCast();
                    break;
                case RayCastMotorMethod.SphereCast:
                    DoSphereCast();
                    break;
            }
        }
    }

    private void DoRayCast()
    {
        Debug.DrawRay(transform.position, transform.forward * _currentDistance, Color.red, 0.2f);
        var hits = Physics.RaycastAll(new Ray(transform.position, transform.forward), _currentDistance);
        // Find the min distance between all the hits and set that as the distance.
        foreach (var hit in hits)
        {
            if (_actsLikeBeam)
            {
                AdjustBeamDistance(hit);
            }
            TryTriggerCollision(new ColliderEventArgs(), hit.collider);
        }

    }

    private void DoSphereCast()
    {
        Debug.DrawRay(transform.position, transform.forward * _currentDistance, Color.red);
        var hits = Physics.SphereCastAll(new Ray(transform.position, transform.forward), _sphereRadius, _currentDistance);
        foreach (var hit in hits)
        {
            if (_actsLikeBeam)
            {
                AdjustBeamDistance(hit);
            }
            TryTriggerCollision(new ColliderEventArgs(), hit.collider);
        }
    }

    private void AdjustBeamDistance(RaycastHit hit)
    {
        if (hit.distance < CurrentRayDistance)
        {
            CurrentRayDistance = hit.distance;
        }
    }

    /// <summary>
    /// Updates the Spell's position to match the spell origin
    /// </summary>
    private void UpdateRayPosition()
    {
        effectSetting.spell.transform.position = effectSetting.spell.SpellStartTransform.position;
        effectSetting.spell.transform.forward = effectSetting.spell.CastingEntity.transform.forward;
    }

    protected override void effectSetting_OnSpellCollision(ColliderEventArgs args, Collider obj)
    {
        base.effectSetting_OnSpellCollision(args, obj);
        if (obj.gameObject.layer == LayerMask.NameToLayer("Entity"))
        {
            Entity e = obj.gameObject.GetComponent<Entity>();
            if (e != null)
                effectSetting.TriggerApplySpell(e);
        }
    }

    public void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(CurrentRayPosition, 0.25f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        switch (MotorMethod)
        {
            case RayCastMotorMethod.RayCast:
                Gizmos.DrawLine(transform.position, transform.position + transform.forward * _rayDistance);
                break;
            case RayCastMotorMethod.CapsuleCast:
                break;
            case RayCastMotorMethod.SphereCast:
                Gizmos.DrawWireSphere(transform.position, _sphereRadius);
                Gizmos.DrawLine(transform.position, transform.position + transform.forward * _rayDistance);
                Gizmos.DrawWireSphere(transform.position + transform.forward * _rayDistance, _sphereRadius);
                break;
        }
    }

}
