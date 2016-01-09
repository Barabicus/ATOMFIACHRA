using UnityEngine;
using System.Collections;
using ParticlePlayground;

[SpellCategory("PP Spell Emission", SpellEffectCategory.FX)]
public class PPSpellEmission : SpellEffectStandard
{

    public EmissionEvent emissionEvent;
    public int emitAmount = 0;

    private PlaygroundParticlesC particleSystem;

    #region Start State

    private bool r_emit;
    private bool r_playing;

    #endregion
    public enum EmissionEvent
    {
        EmitAmount,
        Play,
        Stop
    }

    public override void Initialize(EffectSetting effectSetting)
    {
        base.Initialize(effectSetting);
        particleSystem = GetComponent<PlaygroundParticlesC>();
        r_playing = particleSystem.emit;
    }

    protected override void OnSpellStart()
    {
        base.OnSpellStart();
        particleSystem.emit = r_playing;
    }

    protected override void DoEventTriggered(SpellEffectStandardEventArgs args)
    {
        switch (emissionEvent)
        {
            case EmissionEvent.EmitAmount:
                DoEmitAmount();
                break;
            case EmissionEvent.Play:
                DoPlay();
                break;
            case EmissionEvent.Stop:
                DoStop();
                break;
        }
    }

    private void DoStop()
    {
        particleSystem.emit = false;
    }

    private void DoPlay()
    {
        particleSystem.emit = true;
    }

    private void DoEmitAmount()
    {
        particleSystem.Emit(emitAmount, transform.position, Vector3.zero, Vector3.zero, Color.white);
    }

}
