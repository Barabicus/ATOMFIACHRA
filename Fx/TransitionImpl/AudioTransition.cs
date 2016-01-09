using UnityEngine;
using System.Collections;
[RequireComponent(typeof(AudioSource))]
[TransitionFxCategory("Audio", TransitionFxCategory.Fx)]
public class AudioTransition : ObjectTransitionStandard
{
    [SerializeField]
    private AudioClip _playClip;

    private AudioSource _source;

    protected override void OnInitialise()
    {
        base.OnInitialise();
        _source = gameObject.GetComponent<AudioSource>();
    }

    protected override void OnEventTriggered(ObjectTransitionEventArgs args)
    {
        base.OnEventTriggered(args);
        _source.PlayOneShot(_playClip);
    }


}
