using UnityEngine;
using System.Collections;

public class BarrierComponent : MonoBehaviour
{
    /// <summary>
    /// The barrier that owns this component
    /// </summary>
    public Barrier Owner { get; set; }


    #region Events
    /// <summary>
    /// Called when a Intialise event has been triggered
    /// </summary>
    protected virtual void OnInitialise() { }
    /// <summary>
    /// Called when the Barrier has been triggered on or off.
    /// </summary>
    /// <param name="value"></param>
    protected virtual void OnActiveStateChange (bool value) { }
    #endregion

    #region Triggers
    public void TriggerInitialise()
    {
        // Register State change event
        Owner.OnActiveStateChange += OnActiveStateChange;
        OnInitialise();
    }
    #endregion
}
