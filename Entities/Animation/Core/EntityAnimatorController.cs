using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Entity))]
public abstract class EntityAnimatorController<T> : MonoBehaviour
{

    public Entity Entity { get; set; }
    public Animator Animator { get; set; }

    protected virtual void Start()
    {
        Entity = GetComponent<Entity>();
        Animator = GetComponent<Animator>();
    }

    private void Update()
    {
        UpdateAnimationValues();
    }

    protected abstract void UpdateAnimationValues();

    public virtual void PlayAnimation(T animation)
    {
    }


}
