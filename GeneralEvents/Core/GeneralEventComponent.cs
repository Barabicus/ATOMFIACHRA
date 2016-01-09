using UnityEngine;
using System.Collections;

public class GeneralEventComponent : MonoBehaviour
{
    #region Events
    protected virtual void OnPlayerEnter() { }
    protected virtual void OnPlayerExit() { }
    protected virtual void OnAwake() { }
    protected virtual void OnStart() { }
    protected virtual void OnUpdate() { }
    #endregion

    #region Triggers
    public void TriggerPlayerEnter() { OnPlayerEnter(); }
    public void TriggerPlayerExit() { OnPlayerExit(); }
    public void TriggerAwake() { OnAwake(); }
    public void TriggerStart() { OnStart(); }
    public void TriggerUpdate() { OnUpdate(); }
    #endregion

}
