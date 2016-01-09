using UnityEngine;
using System.Collections;

[SpellCategory("Homing Missile Motor", SpellEffectCategory.Motor)]
[SpellEffectStandard(false, false, "The homing missile motor is much like the standard missile motor except it has the ability to seek out targets when they are within range. Several options exist when determaining how it should seek targets. Such as ignoring targets with specific markers. Homing detection is done via the Update Spell method and is not in control of the event trigger")]
public class HomingMissileMotor : MissileMotor
{
    [SerializeField]
    private MissileTargetingMethod _missileTargetingMethod;
    public bool ignoreEntitiesWithSpellMarker = false;
    [SerializeField]
    private string[] _ignoreMarkerIDs;
    public float searchTime = 1f;
    public float homingRadius = 10f;
    public float homingSpeed = 30f;

    [Tooltip("The target to move towards. If the target is null the homing missle will attempt to find one throughout its life based on homingradius and homingpspeed")]
    private Transform _homingTarget;
    private Timer _searchTimer;
    private Entity _targetEntity;
    private float r_speed;

    public enum MissileTargetingMethod
    {
        Enemies,
        Friendlies,
        Both
    }

    public override Vector3 Direction
    {
        get
        {
            if (_homingTarget == null)
                return base.Direction;
            else
                return (_homingTarget.position - effectSetting.transform.position).normalized + DirectionRandomOffset;
        }
    }

    public override void Initialize(EffectSetting effectSetting)
    {
        base.Initialize(effectSetting);
        _searchTimer = searchTime;
        r_speed = speed;
    }

    protected override void OnSpellStart()
    {
        _searchTimer.Reset();
        _targetEntity = null;
        _homingTarget = null;
        speed = r_speed;
        base.OnSpellStart();
        //  CheckForTargets();
    }

    protected override void UpdateSpell()
    {
        base.UpdateSpell();
        if (_homingTarget == null && _searchTimer.CanTickAndReset())
        {
            CheckForTargets();
        }

        if (_homingTarget != null)
        {
            // ensure collision occurs
            if (Vector3.Distance(effectSetting.transform.position, _homingTarget.position) <= 0.25f)
                TryTriggerCollision(new ColliderEventArgs() { HitCollider = _homingTarget.GetComponent<Collider>(), HitPoint = effectSetting.transform.position }, _homingTarget.GetComponent<Collider>());

            // If we have a target but it is no longer alive, remove the target and reset the variables.
            if (_targetEntity != null && _targetEntity.LivingState != EntityLivingState.Alive)
            {
                _targetEntity = null;
                _homingTarget = null;
                speed = r_speed;
            }
        }
    }

    private void CheckForTargets()
    {
        Transform nearestTransform = null;
        float nearestDistance = 9999999f;
        Entity ent = null;
        foreach (Collider c in Physics.OverlapSphere(effectSetting.transform.position, homingRadius, 1 << LayerMask.NameToLayer("Entity")))
        {
            if (c.gameObject != effectSetting.spell.CastingEntity.gameObject)
            {
                Entity e = c.gameObject.GetComponent<Entity>();
                if (e == null || e.LivingState != EntityLivingState.Alive || (ignoreEntitiesWithSpellMarker && HasMarkerID(e)) || effectSetting.spell.IgnoreEntities.Contains(e))
                    continue;

                switch (_missileTargetingMethod)
                {
                    case MissileTargetingMethod.Enemies:
                        if (!e.IsEnemy(effectSetting.spell.CastingEntity))
                        {
                            continue;
                        }
                        break;
                    case MissileTargetingMethod.Friendlies:
                        if (e.IsEnemy(effectSetting.spell.CastingEntity))
                        {
                            continue;
                        }
                        break;
                }

                float distance = Vector3.Distance(e.transform.position, effectSetting.transform.position);
                if (distance < nearestDistance)
                {
                    ent = e;
                    nearestDistance = distance;
                    nearestTransform = e.transform;
                }
            }
        }

        if (nearestTransform != null)
        {
            _targetEntity = ent;
            _homingTarget = nearestTransform;
            speed = homingSpeed;
        }
    }

    private bool HasMarkerID(Entity e)
    {
        foreach (var id in _ignoreMarkerIDs)
        {
            if (e.HasSpellMarker(id)) return true;
        }
        return false;
    }
}
