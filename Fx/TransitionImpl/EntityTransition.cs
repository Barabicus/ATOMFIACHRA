using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[TransitionFxCategory("Entity Visual", TransitionFxCategory.Entity)]
public class EntityTransition : ObjectTransitionStandard
{
    [SerializeField]
    private bool _freezeInAnimationFrame = false;
    [SerializeField]
    private bool _freezeOutAnimationFrame = false;

    [SerializeField]
    private AnimationCurve _dissolveInCurve = AnimationCurve.Linear(0f, 0f, 1f, 0f);
    [SerializeField]
    private AnimationCurve _dissolveOutCurve = AnimationCurve.Linear(0f, 0f, 1f, 0f);

    [SerializeField]
    private AnimationCurve _overrideInCurve = AnimationCurve.Linear(0f, 0f, 1f, 0f);
    [SerializeField]
    private AnimationCurve _overrideOutCurve = AnimationCurve.Linear(0f, 0f, 1f, 0f);

    [SerializeField]
    private AnimationCurve _freezeInCurve = AnimationCurve.Linear(0f, 0f, 1f, 0f);
    [SerializeField]
    private AnimationCurve _freezeOutCurve = AnimationCurve.Linear(0f, 0f, 1f, 0f);

    private bool _shouldUpdate;
    private List<Material> _matCache;
    private float _currentTime;

    protected override void OnInitialise()
    {
        base.OnInitialise();
        _matCache = new List<Material>();
    }

    protected override void OnStart()
    {
        _shouldUpdate = false;
        _matCache.Clear();
        base.OnStart();
    }

    protected override void OnEventTriggered(ObjectTransitionEventArgs args)
    {
        base.OnEventTriggered(args);

        var entity = Owner.TargetObject.GetComponent<Entity>();
        if (entity != null)
        {
            bool v;
            if(Owner.FXTransitionMethod == ObjectTransitionFx.TransitionMethod.Activate)
            {
                v = _freezeInAnimationFrame;
            }
            else
            {
                v = _freezeOutAnimationFrame;
            }
            if(entity.EntityAnimator != null)
            entity.EntityAnimator.IsAnimationFrozen = v;
        }

        var renderers = Owner.TargetObject.GetComponentsInChildren<Renderer>(true);
        if (renderers == null)
        {
            Debug.LogError(Owner.name + " Could not find renderer on: " + Owner.TargetObject);
            return;
        }
        foreach (var rend in renderers)
        {
            foreach (var mat in rend.materials)
            {
                _matCache.Add(mat);
            }
        }
        _shouldUpdate = true;
        _currentTime = 0f;
    }

    protected override void ComponentUpdate()
    {
        base.ComponentUpdate();
        if (_shouldUpdate)
        {
            DoUpdate();
        }
    }

    private void DoUpdate()
    {
        foreach (var mm in _matCache)
        {
            if (Owner.FXTransitionMethod == ObjectTransitionFx.TransitionMethod.Activate)
            {
                mm.SetFloat("_OverrideAmount", _overrideInCurve.Evaluate(_currentTime));
                mm.SetFloat("_DissolveAmount", _dissolveInCurve.Evaluate(_currentTime));
                mm.SetFloat("_FreezeAmount", _freezeInCurve.Evaluate(_currentTime));
            }
            else if (Owner.FXTransitionMethod == ObjectTransitionFx.TransitionMethod.Deactivate || Owner.FXTransitionMethod == ObjectTransitionFx.TransitionMethod.PoolEntity)
            {
                mm.SetFloat("_OverrideAmount", _overrideOutCurve.Evaluate(_currentTime));
                mm.SetFloat("_DissolveAmount", _dissolveOutCurve.Evaluate(_currentTime));
                mm.SetFloat("_FreezeAmount", _freezeOutCurve.Evaluate(_currentTime));
            }
        }
        _currentTime += Time.deltaTime;
    }

}
