using UnityEngine;
using System.Collections;
using System;

public abstract class AnimationWrapper : EntityComponent, IAnimatorController {

    [SerializeField]
    [Tooltip("Specify animator. If not null it will try to get an animator from the gameobject it is on")]
    private Animator _animator;

    private bool _isAnimationFrozen;

    public virtual bool IsAnimationFrozen
    {
        get
        {
            return _isAnimationFrozen;
        }

        set
        {
            _isAnimationFrozen = value;
            Animator.enabled = !value;
        }
    }

    protected Animator Animator { get { return _animator; } set { _animator = value; } }

    public override void Initialise()
    {
        base.Initialise();
        if (_animator == null)
        {
            Animator = GetComponent<Animator>();
        }
    }

    private void Update()
    {
        UpdateAnimationValues();
    }

    public abstract void PlayAnimation(EntityAnimation animation);
    protected abstract void UpdateAnimationValues();
}
