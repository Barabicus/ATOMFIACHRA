using UnityEngine;
using System.Collections;

public interface IAnimatorController
{
    void PlayAnimation(EntityAnimation animation);
    bool IsAnimationFrozen { get; set; }

}

public enum EntityAnimation
{
    Cast01, 
    Cast02,
    Melee01
}