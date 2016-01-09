using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class SpellUnlockedToast : ToastObject<Spell>
{
    [SerializeField]
    private Image _spellImage;
    [SerializeField]
    private Text _spellName;

    public RectTransform OverlayEndRect { get; set; }

    protected override void DoSetupToast(Spell obj)
    {
        _spellImage.overrideSprite = obj.SpellIcon;
        _spellName.text = obj.SpellName;
        if (OverlayEndRect != null)
        {
            var spellOverlay = OnScreenImageToPointPool.Instance.GetObjectFromPool((o) =>
           {
               o.TargetSprite = obj.SpellIcon;
               o.StartTransform = _spellImage.rectTransform;
               o.EndTransform = OverlayEndRect;
               o.RectTransform.SetParent(transform.parent);
           });
        }
    }

}
