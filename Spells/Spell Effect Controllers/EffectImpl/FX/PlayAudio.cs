using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
[SpellCategory("Play Audio", SpellEffectCategory.FX)]
public class PlayAudio : SpellEffectStandard
{
    public AudioClip audioClip;

    private AudioSource _audioSource;

    protected override void OnSpellStart()
    {
        base.OnSpellStart();
        _audioSource = GetComponent<AudioSource>();
    }

    protected override void DoEventTriggered(SpellEffectStandardEventArgs args)
    {
        base.DoEventTriggered(args);
        _audioSource.PlayOneShot(audioClip);
    }
}
