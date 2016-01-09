using UnityEngine;
using System.Collections;

[SpellCategory("Blend Lut", SpellEffectCategory.FX)]
public class BlendLUTSpellEffect : SpellEffectStandard
{
    [SerializeField]
    private Texture2D _LutTarget;
    [SerializeField]
    private float blendInTime = 0.2f;
    [SerializeField]
    private float blendOutTime = 0.2f;

    private Texture2D _previousLUT;

    protected override void DoEventTriggered(SpellEffectStandardEventArgs args)
    {
        base.DoEventTriggered(args);
        CameraController.Instance.BlendTo(_LutTarget, blendInTime, blendOutTime);
    }

}
