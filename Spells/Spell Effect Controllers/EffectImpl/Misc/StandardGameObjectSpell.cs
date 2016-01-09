using System;
using UnityEngine;
using System.Collections;
/// <summary>
/// Activates or deactivates specified gameObjects
/// </summary>
[SpellCategory("Standard Game Object", SpellEffectCategory.Misc)]
public class StandardGameObjectSpell : SpellEffectStandard
{
    public GameObjectEvent gameObjectEvent;

    private bool r_activated;

    public enum GameObjectEvent
    {
        Activate,
        Deactivate
    }

    public override void Initialize(EffectSetting effectSetting)
    {
        base.Initialize(effectSetting);
        r_activated = gameObject.activeSelf;
    }

    protected override void OnSpellStart()
    {
        base.OnSpellStart();
        gameObject.SetActive(r_activated);
    }

    protected override void DoEventTriggered(SpellEffectStandardEventArgs args)
    {
        switch (gameObjectEvent)
        {
            case GameObjectEvent.Activate:
                gameObject.SetActive(true);
                break;
            case GameObjectEvent.Deactivate:
                gameObject.SetActive(false);
                break;
        }
    }
}
