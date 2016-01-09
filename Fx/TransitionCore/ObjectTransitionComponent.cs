using UnityEngine;
using System.Collections;

public abstract class ObjectTransitionComponent : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Should the transition component continue to update after the Transition has completed. In this time the transition is waiting to repool. This is handy for delaying effects and allowing them to complete.")]
    private bool _updateOnCompleted = true;

    public ObjectTransitionFx Owner { get; set; }

    public void TriggerInitialise(ObjectTransitionFx owner)
    {
        Owner = owner;
        OnInitialise();
    }

    public void TriggerStart()
    {
        OnStart();
    }

    public void TriggerComplete()
    {
        OnComplete();
    }

    public void TriggerPrePool() { }

    private void Update()
    {
        if (_updateOnCompleted || (!Owner.IsTransitionComplete) )
            ComponentUpdate();
    }

    protected virtual void OnInitialise() { }
    protected virtual void OnStart() { }
    protected virtual void OnComplete() { }
    protected virtual void ComponentUpdate() { }
    protected virtual void OnPrePool() { }

}
