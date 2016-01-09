using UnityEngine;
using System.Collections;
using System;
using UnityStandardAssets.ImageEffects;

/// <summary>
/// Used as a means to handle camera control and various scripts associated with the camera.
/// </summary>
public class CameraController : GameController
{

    private AmplifyColorBase _amplifyCache;
    private ColorCorrectionCurves _correctionCurve;
    private Entity _playerEntity;

    public static CameraController Instance { get; private set; }

    public override void OnAwake()
    {
        base.OnAwake();
        Instance = this;
    }

    public override void OnStart()
    {
        base.OnStart();
        _amplifyCache = GameMainReferences.Instance.AmplifyColor;
        _playerEntity = GameMainReferences.Instance.PlayerController.Entity;
        _correctionCurve = GameMainReferences.Instance.RTSCamera.GetComponent<ColorCorrectionCurves>();
        if(LevelMetaInfo.Instance.StartLutTexture != null)
        {
            _amplifyCache.LutTexture = LevelMetaInfo.Instance.StartLutTexture;
            MainLutTexture = LevelMetaInfo.Instance.StartLutTexture;
        }
    }

    public Texture2D MainLutTexture { get; private set; }

    private void LowHealthEffect()
    {
        _correctionCurve.saturation = Mathf.Clamp(_playerEntity.CurrentHealthNormalised * 2f, 0f, 1f);
    }

    /// <summary>
    /// Blends to a lut texture for the specified amount of time and automatically blends back to the main lut texture
    /// </summary>
    /// <param name="LutTexture"></param>
    /// <param name="time"></param>
    public void BlendTo(Texture2D LutTexture, float timeIn, float timeOut)
    {
        if (_amplifyCache.IsBlending)
        {
            return;
        }
        _amplifyCache.BlendTo(LutTexture, timeIn, () =>
        {
            _amplifyCache.BlendTo(MainLutTexture, timeOut, null);
        });
    }

    private void Update()
    {
        LowHealthEffect();
    }

}
