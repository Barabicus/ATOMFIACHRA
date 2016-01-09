using UnityEngine;
using System.Collections;
using ParticlePlayground;

[SpellEffectStandard(false, false, "This spell will set the mask of the particle playground system to match that of a Ray Motor. It should be noted this spell requires a Raycast Motor and will not function properly without one.")]
[SpellCategory("PP Beam Chaser", SpellEffectCategory.FX)]
public class PPBeamChaser : SpellEffectStandard
{
    [SerializeField]
    private bool _killAllParticlesOnDestroy = true;

    private PlaygroundParticlesC _particles;
    private int previousParticleCount;
    private RayCastMotor _rayMotor;

    public float ParticleMask
    {
        get
        {
            return Mathf.Clamp01(_rayMotor.CurrentRayDistance / _rayMotor.MaxDistance);
        }
    }

    public override void Initialize(EffectSetting effectSetting)
    {
        base.Initialize(effectSetting);
        _rayMotor = effectSetting.GetComponentsInChildren<RayCastMotor>(true)[0];
        _particles = GetComponent<PlaygroundParticlesC>();
        _particles.applyParticleMask = true;
        _particles.overflowOffset.z = _rayMotor.MaxDistance / (1 + _particles.particleCount);
        if (_rayMotor == null)
        {
            Debug.LogError("Beam Chaser cannot find raycast motor! spell: " + effectSetting.spell);
        }
    }

    protected override void DoEventTriggered(SpellEffectStandardEventArgs args)
    {
        base.DoEventTriggered(args);
        _particles.particleMask = _particles.particleCount - Mathf.RoundToInt(_particles.particleCount * ParticleMask);
    }

    protected override void effectSetting_OnSpellDestroy()
    {
        base.effectSetting_OnSpellDestroy();
        if (_killAllParticlesOnDestroy)
        {
            for(int i = 0; i < _particles.particleCount; i++)
            {
                _particles.Kill(i);
            }
        }
    }

    private void Reset()
    {
        TriggerEvent = SpellEffectTriggerEvent.SpellUpdate;
    }

}
