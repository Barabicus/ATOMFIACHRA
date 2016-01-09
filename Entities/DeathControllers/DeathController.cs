using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class DeathController : EntityComponent
{
    [SerializeField]
    private ObjectTransitionFx _noMarkerFx;
    [SerializeField]
    private ObjectTransitionFx _frozenFx;
    [SerializeField]
    private ObjectTransitionFx _goreFx;

    private bool _hasTriggered;

    public override void Initialise()
    {
        base.Initialise();
    }

    public override void OnStart()
    {
        base.OnStart();
        _hasTriggered = false;
    }

    protected override void Update()
    {
        base.Update();
        if (!_hasTriggered && Entity.LivingState == EntityLivingState.Dead)
        {
            DoTrigger();
        }
    }

    private void DoTrigger()
    {
        ObjectTransitionFx transitionFx = null;

        switch (Entity.SpellDeathMarker)
        {
            case SpellDeathMarker.None:
                transitionFx = _noMarkerFx;
                break;
            case SpellDeathMarker.Freeze:
                transitionFx = _frozenFx;
                break;
            case SpellDeathMarker.Gore:
                transitionFx = _goreFx;
                break;
        }

        _hasTriggered = true;
        if (transitionFx == null)
        {
            Debug.LogErrorFormat("No transition fx found on Entity {0} with death Marker {1} setting default", Entity.name, Entity.SpellDeathMarker);
            transitionFx = _noMarkerFx;
        }
        else
        {
            var obj = ObjectTransitionPool.Instance.GetObjectFromPool(transitionFx.PoolID, (o) =>
            {
                o.FXTransitionMethod = ObjectTransitionFx.TransitionMethod.PoolEntity;
                o.TargetObject = Entity.gameObject;
            });
            obj.gameObject.SetActive(true);
        }
    }
}