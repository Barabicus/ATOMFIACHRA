using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
[SpellCategory("Missile Motor", SpellEffectCategory.Motor)]
public class MissileMotor : SpellMotor
{
    public float speed = 2f;
    [Tooltip("This will cause the spell to aim towards a certain direction. Aiming towards the forward direction will simply make the spell move in the forward direction initially where aiming towards the target will make the spell move towards the target initially.")]
    public MissileDirection targetDirection = MissileDirection.AimTowardsTarget;
    [SerializeField]
    private GroundDistanceMethod _groundDistanceMethod = GroundDistanceMethod.None;
    [Tooltip("If true the missile direction will be within the same y axis as the caster. This does not apply if modified by a direction offset or random offset")]
    public bool negateYDirection = true;
    public float minGroundDistance = 1f;
    [SerializeField]
    private float _keepGroundDistance = 1f;
    [SerializeField]
    private bool _canCollideWithGround = false;

    [Tooltip("The speed curve for this spell effect. Y axis is modifer X axis is living time percent from 0 - 1")]
    public AnimationCurve speedCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);
    [Tooltip("The Direction offset in spell living time. Note this is spell living real time and not a percent")]
    public AnimationCurve directionXCurve = AnimationCurve.Linear(0f, 0f, 1f, 0f);
    [Tooltip("The Direction offset in spell living time. Note this is spell living real time and not a percent")]
    public AnimationCurve directionYCurve = AnimationCurve.Linear(0f, 0f, 1f, 0f);
    [Tooltip("The Direction offset in spell living time. Note this is spell living real time and not a percent")]
    public AnimationCurve directionZCurve = AnimationCurve.Linear(0f, 0f, 1f, 0f);

    public bool randomXMove = false;
    public bool randomYMove = false;
    public bool randomZMove = false;
    public float randomXRadius = 1f;
    public float randomYRadius = 1f;
    public float randomZRadius = 1f;

    [Tooltip("The Random x radius multiplier curve based over spell life time")]
    public AnimationCurve randomXRadiusCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);
    [Tooltip("The Random y radius multiplier curve based over spell life time")]
    public AnimationCurve randomYRadiusCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);
    [Tooltip("The Random z radius multiplier curve based over spell life time")]
    public AnimationCurve randomZRadiusCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

    [Tooltip("The random amount of sin cycles that should occur. A Higher number will lead to rapid cycles leading to a lower radius area")]
    public float minRange = 1f;
    public float randomRange = 1f;
    [Tooltip("This will trigger a collision on the OnTriggerStay event if true. This means as long as an Entity remains within the spell collider it will contiously trigger")]
    public bool triggerCollisionOnSpellStay = false;
    [Tooltip("This is how frequent the spell can re-trigger collision events. Typically a missile will be destroyed when it collides with something but if it doesn't use this to set how frequent it will trigger collisions")]
    public float retriggerTime = 0.1f;
    [SerializeField]
    private MissileCheckMethod checkMethod = MissileCheckMethod.SphereCast;
    [SerializeField]
    private float sphereCastRadius = 0.5f;
    [SerializeField]
    private float _directionMinRange;
    [SerializeField]
    private float _directionMaxRange;

    private Vector3 direction;
    private bool shouldMove = true;

    private float _lastTime;
    private float _timeStartOffset;

    private float xRandDir = 0f, yRandDir = 0f, zRandDir = 0f;
    private float xRandSpeed, yRandSpeed, zRandSpeed;

    private Timer _triggerTimer;
    private Vector3 _lastPosition;

    public enum MissileCheckMethod
    {
        RayCast,
        SphereCast
    }

    public enum GroundDistanceMethod
    {
        None,
        MinDistance,
        KeepDistance
    }

    public virtual Vector3 Direction
    {
        get
        {
            return (direction).normalized;
        }
        set
        {
            direction = value;
        }
    }

    public Vector3 RandomRadius
    {
        get
        {
            return transform.TransformDirection(new Vector3(randomXRadius * randomXRadiusCurve.Evaluate(CurrentLivingTimePercent), randomYRadius * randomYRadiusCurve.Evaluate(CurrentLivingTimePercent), randomZRadius * randomZRadiusCurve.Evaluate(CurrentLivingTimePercent)));
        }
    }

    protected virtual float Speed
    {
        get
        {
            return (speed * speedCurve.Evaluate(CurrentLivingTimePercent)) * Time.deltaTime;
        }
    }

    public Vector3 DirectionRandomOffset
    {
        get
        {
            return transform.TransformDirection(new Vector3(xRandDir, yRandDir, zRandDir)) * Time.deltaTime;
        }
    }
    /// <summary>
    /// This is the direction curve used to modify the direction over a certain period of time. 
    /// </summary>
    public Vector3 DirectionCurve
    {
        get
        {
            return transform.TransformDirection(new Vector3(directionXCurve.Evaluate(CurrentTime), directionYCurve.Evaluate(CurrentTime), directionZCurve.Evaluate(CurrentTime))) * Time.deltaTime;
        }
    }

    protected float CurrentTimeAndOffset
    {
        get { return (CurrentTime) + _timeStartOffset; }
    }

    protected float CurrentTime
    {
        get { return Time.time - _lastTime; }
    }

    public enum MissileDirection
    {
        AimTowardsForward,
        AimTowardsTarget
    }

    public override void Initialize(EffectSetting effectSetting)
    {
        base.Initialize(effectSetting);
        GetComponent<Rigidbody>().isKinematic = true;
        _triggerTimer = new Timer(retriggerTime);
        Collider[] colliders = GetComponents<Collider>();

        // Ensure all colliders are set to trigger
        foreach (Collider c in colliders)
        {
            c.isTrigger = true;
        }
    }

    protected override void OnSpellStart()
    {
        base.OnSpellStart();
        shouldMove = true;
        _lastPosition = transform.position;
        _lastTime = Time.time;
        switch (targetDirection)
        {
            case MissileDirection.AimTowardsForward:
                Direction = effectSetting.spell.SpellStartTransform.forward;
                break;
            case MissileDirection.AimTowardsTarget:
                Direction = ((effectSetting.spell.SpellTargetPosition.Value) - effectSetting.spell.SpellStartPosition);
                break;
        }
        var randomDirection = new Vector3(Random.Range(_directionMinRange, _directionMaxRange), 0f, 0f);
        randomDirection = transform.TransformDirection(randomDirection);

        Direction = Direction + randomDirection;

        if (negateYDirection)
            direction.y = 0;
      //  Direction.Normalize();

        Debug.DrawRay(effectSetting.spell.CastingEntity.transform.position, Direction * 5f, Color.red, 10f);

        InitRandomVariables();
        transform.parent.forward = Direction;
    }

    private void TriggerSpell(Vector3 hitPoint, Vector3 hitNormal, Collider hitCollider)
    {
        if (_triggerTimer.CanTickAndReset())
        {
            if (hitCollider.gameObject != effectSetting.spell.CastingEntity.gameObject && hitCollider.gameObject.layer != LayerMask.NameToLayer("Spell") && hitCollider.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))
            {
                TryTriggerCollision(new ColliderEventArgs() { HitPoint = hitPoint, HitNormal = hitNormal, HitCollider = hitCollider }, hitCollider);
            }
        }
    }

    protected override void effectSetting_OnSpellCollision(ColliderEventArgs args, Collider obj)
    {
        base.effectSetting_OnSpellCollision(args, obj);
        // Collided with an enemy. Spell apply on the entity.
        if (obj.gameObject.layer == LayerMask.NameToLayer("Entity"))
        {
            Entity e = obj.gameObject.GetComponent<Entity>();
            if (e != null)
                effectSetting.TriggerApplySpell(e);
        }
    }

    protected override void DoEventTriggered(SpellEffectStandardEventArgs args)
    {
        base.DoEventTriggered(args);
        UpdateMissile();
    }

    private void UpdateMissile()
    {
        UpdateRandomValues();
        effectSetting.transform.forward = Direction;
        if (shouldMove)
        {
            transform.parent.forward = Direction;
            effectSetting.transform.position += DirectionCurve;
            effectSetting.transform.position += (Direction * Speed) + Vector3.Scale(DirectionRandomOffset, RandomRadius);

            RaycastHit hit;
            switch (_groundDistanceMethod)
            {
                case GroundDistanceMethod.MinDistance:
                    if (Physics.Raycast(effectSetting.transform.position, -Vector3.up, out hit, minGroundDistance, 1 << LayerMask.NameToLayer("Ground")))
                    {
                        var pos = effectSetting.transform.position;
                        pos.y = hit.point.y + minGroundDistance;
                        effectSetting.transform.position = pos;
                    }
                    break;
                case GroundDistanceMethod.KeepDistance:
                    if (Physics.Raycast(effectSetting.transform.position, Vector3.down, out hit, 500f, 1 << LayerMask.NameToLayer("Ground")))
                    {
                        var pos = effectSetting.transform.position;
                        pos.y = hit.point.y + _keepGroundDistance;
                        effectSetting.transform.position = pos;
                    }
                    break;
            }
        }
        TryDetectTarget();
        _lastPosition = transform.position;
    }

    private void TryDetectTarget()
    {
        RaycastHit hit;
        Ray r = new Ray(_lastPosition, transform.position - _lastPosition);

        var mask = 1 << LayerMask.NameToLayer("Entity") | 1 << LayerMask.NameToLayer("Environment") | 1 << LayerMask.NameToLayer("Obstacle");
        if (_canCollideWithGround)
        {
            mask = mask | 1 << LayerMask.NameToLayer("Ground");
        }
        switch (checkMethod)
        {
            case MissileCheckMethod.RayCast:
                if (Physics.Raycast(r, out hit, Vector3.Distance(_lastPosition, transform.position),
                    mask))
                {
                    TriggerSpell(hit.point, hit.normal, hit.collider);
                }
                break;
            case MissileCheckMethod.SphereCast:
                if (Physics.SphereCast(r, sphereCastRadius, out hit, Vector3.Distance(_lastPosition, transform.position),
                    mask))
                {
                    TriggerSpell(hit.point, hit.normal, hit.collider);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void InitRandomVariables()
    {
        _timeStartOffset = Random.Range(0, 1000);

        UpdateRandomSpeed();
        UpdateRandomValues();
    }

    private void UpdateRandomSpeed()
    {
        if (randomXMove)
            xRandSpeed = (Random.Range(minRange * 1000, 1000 * randomRange) + 1) / 1000f;
        if (randomYMove)
            yRandSpeed = (Random.Range(minRange * 1000, 1000 * randomRange) + 1) / 1000f;
        if (randomZMove)
            zRandSpeed = (Random.Range(minRange * 1000, 1000 * randomRange) + 1) / 1000f;
    }

    private void UpdateRandomValues()
    {
        if (randomXMove)
        {
            xRandDir = (CurrentTimeAndOffset * xRandSpeed) * Mathf.Deg2Rad;
            xRandDir = Mathf.Sin(xRandDir);
        }

        if (randomYMove)
        {
            yRandDir = (CurrentTimeAndOffset * yRandSpeed) * Mathf.Deg2Rad;
            yRandDir = Mathf.Sin(yRandDir);
        }

        if (randomZMove)
        {
            zRandDir = (CurrentTimeAndOffset * zRandSpeed) * Mathf.Deg2Rad;
            zRandDir = Mathf.Sin(zRandDir);
        }
    }

    protected override void effectSetting_OnSpellDestroy()
    {
        base.effectSetting_OnSpellDestroy();
        shouldMove = false;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (checkMethod == MissileCheckMethod.SphereCast)
        {
            Gizmos.DrawWireSphere(transform.position, sphereCastRadius);
            Gizmos.DrawRay(transform.position, transform.forward * 5f);
            Gizmos.DrawWireSphere(transform.position + transform.forward * 5f, sphereCastRadius);
        }
    }



}
