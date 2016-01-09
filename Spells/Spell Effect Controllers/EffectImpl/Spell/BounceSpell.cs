using System;
using UnityEngine;
using System.Collections;
using System.Security;

[SpellCategory("Bounce Spell", SpellEffectCategory.Spell)]
[SpellEffectStandard(true, false, "This will target entities around the target entity and bounce the spell specified. The spell requires a target and will bounce spells from that target to nearby entities. The entity that the spell was bounced from will be ignored from interacting with the bounced spells. The bounced spells also behave as if they were cast from the initial spell caster.")]
public class BounceSpell : SpellEffectStandard
{
    [SerializeField]
    private float radius = 5f;
    [SerializeField]
    private bool bounceLimit = true;
    [SerializeField]
    private int maxBounceAmount = 1;
    [SerializeField]
    private string[] _ignoreMarkers;
    [SerializeField]
    private BounceTargetingMethod _bounceTargetingMethod = BounceTargetingMethod.Enemies;
    [SerializeField]
    private BounceState bounceState = BounceState.UseCastSpell;
    [SerializeField]
    private Spell customSpell;

    private int _currentBounces;

    public enum BounceState
    {
        UseCastSpell,
        UseCustomSpell
    }

    public enum BounceTargetingMethod
    {
        Enemies,
        Friendlies,
        Both
    }

    protected override void OnSpellStart()
    {
        base.OnSpellStart();
        _currentBounces = 0;
    }

    protected override void DoEventTriggered(SpellEffectStandardEventArgs args)
    {
        base.DoEventTriggered(args);
        if(args.TargetEntity != null)
        {
            TryBounce(args.TargetEntity);
        }
        else
        {
            Debug.LogError("Tried to bounce spell: " + effectSetting.spell + " but target entity was null");
        }
    }

    private void TryBounce(Entity hitEnt)
    {

        //if (!ignoreHitMarker && hitEnt.HasSpellMarker(SpellMarker) || _currentBounces == maxBounceAmount)
        //    return;

        Collider[] colls = Physics.OverlapSphere(effectSetting.transform.position, radius, 1 << LayerMask.NameToLayer("Entity"));

        foreach (var c in colls)
        {
            if (c.gameObject != hitEnt.gameObject && c.gameObject != effectSetting.spell.CastingEntity.gameObject)
            {
                Entity targetEnt = c.gameObject.GetComponent<Entity>();

                if (targetEnt == null || targetEnt.LivingState != EntityLivingState.Alive)
                    continue;

                if (ShouldIgnore(targetEnt))
                    continue;

                switch (_bounceTargetingMethod)
                {
                    case BounceTargetingMethod.Both:
                        break;
                    case BounceTargetingMethod.Enemies:
                        if (!targetEnt.IsEnemy(effectSetting.spell.CastingEntity))
                            continue;
                        break;
                    case BounceTargetingMethod.Friendlies:
                        if (targetEnt.IsEnemy(effectSetting.spell.CastingEntity))
                            continue;
                        break;
                }

                Vector3 startVector = new Vector3(hitEnt.transform.position.x, effectSetting.spell.transform.position.y, hitEnt.transform.position.z);
                Spell sp = null;

                switch (bounceState)
                {
                    case BounceState.UseCastSpell:
                        sp = SpellList.Instance.GetNewSpell(effectSetting.spell);
                        break;
                    case BounceState.UseCustomSpell:
                        sp = SpellList.Instance.GetNewSpell(customSpell);
                        break;
                }
                sp.CastSpell(effectSetting.spell.CastingEntity, effectSetting.spell.transform, startVector, targetEnt.transform, targetEnt.transform.position);

                //Ignore the entity the spell was bounced from
                sp.IgnoreEntities.Add(hitEnt);

                if (bounceLimit)
                {
                    _currentBounces++;
                    if (_currentBounces == maxBounceAmount)
                        return;
                }

            }
        }
    }

    private bool ShouldIgnore(Entity e)
    {
        foreach (var marker in _ignoreMarkers)
        {
            if (e.HasSpellMarker(marker)) return true;
        }
        return false;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
