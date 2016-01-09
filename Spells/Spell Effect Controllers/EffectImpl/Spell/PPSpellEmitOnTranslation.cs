using UnityEngine;
using System.Collections;
using ParticlePlayground;

[SpellCategory("Emit On Translation", SpellEffectCategory.FX)]
public class PPSpellEmitOnTranslation : SpellEffect
{

    private PlaygroundParticlesC particles;				
    private Transform movingTransform;					
    public float minSpace = .1f;						// The minimum space required for emission
    public int emitMultiplier = 1;

    bool isReady;
    Vector3 previousPosition;

    public override void Initialize(EffectSetting effectSetting)
    {
        base.Initialize(effectSetting);
        if (particles == null)
            particles = GetComponent<PlaygroundParticlesC>();
        if (movingTransform == null)
            movingTransform = transform;
        if (particles != null && movingTransform != null)
            isReady = true;
    }

    protected override void OnSpellStart()
    {
        base.OnSpellStart();
        previousPosition = movingTransform.position;
    }

    protected override void UpdateSpell()
    {
        base.UpdateSpell();
        if (isReady)
            EmissionMoveCheck();
    }

    /// <summary>
    /// Emit when particle system is moving.
    /// </summary>
    private void EmissionMoveCheck()
    {
        if (movingTransform.position != previousPosition)
        {
            float emissionStepper = 0;
            float deltaDistance = Vector3.Distance(previousPosition, movingTransform.position);
            if (deltaDistance < minSpace) return;
            Vector3 delta = previousPosition;
            Vector3 velocity = Vector3.zero;
            Color color = Color.white;
            while (emissionStepper < deltaDistance)
            {
                delta = Vector3.Lerp(previousPosition, movingTransform.position, emissionStepper / deltaDistance);
                for (int i = 0; i < emitMultiplier; i++)
                    particles.Emit(delta);
                emissionStepper += minSpace;
            }
        }
        previousPosition = movingTransform.position;
    }
}
