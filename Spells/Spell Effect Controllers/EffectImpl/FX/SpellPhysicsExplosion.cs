using UnityEngine;
using System.Collections;
[SpellCategory("Physics Explosion", SpellEffectCategory.FX)]
[SpellEffectStandard(false, false, "Creates a physiscs explosion. This will effect objects around the spell and ragdolls. Nice for a bit of juice.")]
public class SpellPhysicsExplosion : SpellEffectStandard
{
    [SerializeField]
    private float _radius = 5f;
    [SerializeField]
    private float _power = 5f;
    [SerializeField]
    private float _upwardsModifer = 3f;
    [SerializeField]
    private ForceMode _forceMode = ForceMode.Impulse;

    protected override void DoEventTriggered(SpellEffectStandardEventArgs args)
    {
        base.DoEventTriggered(args);

        Collider[] colliders = Physics.OverlapSphere(effectSetting.transform.position, _radius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
                rb.AddExplosionForce(_power, effectSetting.transform.position, _radius, _upwardsModifer, _forceMode);
        }
    }

}
