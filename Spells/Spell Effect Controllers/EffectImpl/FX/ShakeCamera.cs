using UnityEngine;
using System.Collections;

[SpellCategory("Shake Camera", SpellEffectCategory.FX)]
public class ShakeCamera : SpellEffectStandard
{
    public float shakeAmount = 1f;
    public float shakeTime = 0.25f;
    [Tooltip("The further the way the distance from the player the less intense the shake will be. This takes position and max shake amount into account and outputs a value appropriately")]
    public float maxDistanceFromPlayer = 50f;

    protected override void DoEventTriggered(SpellEffectStandardEventArgs args)
    {
        var mult = 1f -
            MathUtility.GetPercent(
                Vector3.Distance(effectSetting.transform.position,
                    GameMainReferences.Instance.PlayerController.transform.position), maxDistanceFromPlayer);
        GameMainReferences.Instance.RTSCamera.TriggerShake(shakeTime, shakeAmount * mult);
    }


}
