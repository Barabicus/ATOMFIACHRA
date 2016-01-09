using UnityEngine;
using System.Collections;
using System;

public class HumanoidAnimatorController : AnimationWrapper, IAnimatorController
{
    private float _speedLerpAmount = 7f;

    private AnimatorStateInfo _layer1StateInfo;

    #region Animation Hashes
    // base
    private static int animSpeed = Animator.StringToHash("speed");
    private static int animDead = Animator.StringToHash("dead");
    
    // triggers
    private static int cast01 = Animator.StringToHash("cast01");
    private static int cast02 = Animator.StringToHash("cast02");
    private static int cast03 = Animator.StringToHash("cast03");

    private static int melee01 = Animator.StringToHash("melee01");
    #endregion

    private float _speedValue = 0f;

    protected override void UpdateAnimationValues()
    {
        switch (Entity.LivingState)
        {
            case EntityLivingState.Alive:
                Animator.SetBool(animDead, false);
                break;
            case EntityLivingState.Dead:
                Animator.SetBool(animDead, true);
                break;
        }

        // Update speed value
        //    _speedValue = Mathf.Lerp(_speedValue, Entity.CurrentSpeed, Time.deltaTime * _speedLerpAmount);
        _speedValue = Entity.CurrentSpeed;
        Animator.SetFloat(animSpeed, _speedValue);
    }

    public override void PlayAnimation(EntityAnimation animation)
    {
        switch (animation)
        {
            case EntityAnimation.Cast01:
                Animator.SetTrigger(cast01);
                break;
            case EntityAnimation.Cast02:
                Animator.SetTrigger(cast02);
                break;
            case EntityAnimation.Melee01:
                Animator.SetTrigger(melee01);
                break;
        }
    }

}
