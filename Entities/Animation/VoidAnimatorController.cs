using UnityEngine;
using System.Collections;

public class VoidAnimatorController : EntityAnimatorController<VoidAnimation>
{

    private static int animSpeed = Animator.StringToHash("speed");
    private static int animDead = Animator.StringToHash("dead");

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

        Animator.SetFloat(animSpeed, Entity.CurrentSpeed);
    }

}

public enum VoidAnimation
{

}