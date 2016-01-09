using UnityEngine;
using System.Collections;

public class GameObjectBarrierComponent : BarrierComponent
{
    [SerializeField]
    private bool _activationIsActive = true;
    [SerializeField]
    private bool _deactivationIsActive = false;

    protected override void OnInitialise()
    {
        base.OnInitialise();
    }

    protected override void OnActiveStateChange(bool value)
    {
        base.OnActiveStateChange(value);
        gameObject.SetActive(value ? _activationIsActive : _deactivationIsActive);
    }

}
