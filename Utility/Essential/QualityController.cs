using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class QualityController : GameController
{
    public static QualityController Instance { get; set; }

    [SerializeField]
    private QualityRule[] _qualityRules;

    #region Quality References
    private Antialiasing _antialiasing;
    private ScreenSpaceAmbientObscurance _ssao;
    private GlobalFog _globalFog;
    private Bloom _bloom;
    private VignetteAndChromaticAberration _vignetteAndChromaticAberration;
    private DepthOfField _depthOfField;
    private AmplifyColorBase _amplifyColor;
    private ColorCorrectionCurves _deathDesaturationCurve;
    #endregion

    private int _qualityLevel;

    public override void OnAwake()
    {
        base.OnAwake();
        Instance = this;
        GetReferences();
        UpdateQualitySetting();
    }

    public int QualityLevel { get { return _qualityLevel; } }

    private void GetReferences()
    {
        var camera = GameMainReferences.Instance.RTSCamera.GetComponent<Camera>();
        _antialiasing = CheckAndGetComponent<Antialiasing>(camera);
        _ssao = CheckAndGetComponent<ScreenSpaceAmbientObscurance>(camera);
        _globalFog = CheckAndGetComponent<GlobalFog>(camera);
        _bloom = CheckAndGetComponent<Bloom>(camera);
        _vignetteAndChromaticAberration = CheckAndGetComponent<VignetteAndChromaticAberration>(camera);
        _depthOfField = CheckAndGetComponent<DepthOfField>(camera);
        _amplifyColor = CheckAndGetComponent<AmplifyColorBase>(camera);
        _deathDesaturationCurve = CheckAndGetComponent<ColorCorrectionCurves>(camera);

    }

    private T CheckAndGetComponent<T>(Camera camera)
    {
        var c = camera.gameObject.GetComponent<T>();
        if(c == null)
        {
            Debug.LogError("Component " + typeof(T) + " was not found on the camera for quality settings.");
        }
        return c;
    }
    /// <summary>
    /// Use this method to set the quality setting to ensure all post processing effects are handled properly.
    /// </summary>
    /// <param name="index"></param>
    public void SetPresetQualitySetting(int index)
    {
        QualitySettings.SetQualityLevel(index);
        UpdateQualitySetting();
        _qualityLevel = index;
    }

    private void UpdateQualitySetting()
    {
        int level = QualitySettings.GetQualityLevel();
        if(_qualityRules.Length == 0 || level >= _qualityRules.Length)
        {
            Debug.LogErrorFormat("Quality Setting {0} was out of bounds for the amount of rules specified!", level);
            return;
        }
        QualityRule rule = _qualityRules[(QualitySettings.GetQualityLevel())];

        // Set quality level
        _antialiasing.enabled = rule.Antialiasing;
        _ssao.enabled = rule.Ssao;
        _globalFog.enabled = rule.GlobalFog;
        _bloom.enabled = rule.Bloom;
        _vignetteAndChromaticAberration.enabled = rule.Bloom;
        _depthOfField.enabled = rule.DepthOfField;
        _amplifyColor.enabled = rule.AmplifyColor;
        _deathDesaturationCurve.enabled = rule.DeathDesaturationCurve;
        
    }
}
[System.Serializable]
public struct QualityRule
{
    [SerializeField]
    private bool _isAntialiasing;
    [SerializeField]
    private bool _ssao;
    [SerializeField]
    private bool _globalFog;
    [SerializeField]
    private bool _bloom;
    [SerializeField]
    private bool _vignetteAndChromaticAberration;
    [SerializeField]
    private bool _depthOfField;
    [SerializeField]
    private bool _amplifyColor;
    [SerializeField]
    private bool _deathDesaturationCurve;
    [SerializeField]
    private bool _shadowsOn;

    public bool Antialiasing
    {
        get
        {
            return _isAntialiasing;
        }

        set
        {
            _isAntialiasing = value;
        }
    }

    public bool Ssao
    {
        get
        {
            return _ssao;
        }

        set
        {
            _ssao = value;
        }
    }

    public bool GlobalFog
    {
        get
        {
            return _globalFog;
        }

        set
        {
            _globalFog = value;
        }
    }

    public bool Bloom
    {
        get
        {
            return _bloom;
        }

        set
        {
            _bloom = value;
        }
    }

    public bool VignetteAndChromaticAberration
    {
        get
        {
            return _vignetteAndChromaticAberration;
        }

        set
        {
            _vignetteAndChromaticAberration = value;
        }
    }

    public bool DepthOfField
    {
        get
        {
            return _depthOfField;
        }

        set
        {
            _depthOfField = value;
        }
    }

    public bool AmplifyColor
    {
        get
        {
            return _amplifyColor;
        }

        set
        {
            _amplifyColor = value;
        }
    }

    public bool DeathDesaturationCurve
    {
        get
        {
            return _deathDesaturationCurve;
        }

        set
        {
            _deathDesaturationCurve = value;
        }
    }

    public bool ShadowsOn
    {
        get
        {
            return _shadowsOn;
        }

        set
        {
            _shadowsOn = value;
        }
    }
}
