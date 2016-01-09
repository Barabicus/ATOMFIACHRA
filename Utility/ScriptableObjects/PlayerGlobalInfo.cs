using UnityEngine;
using System.Collections;

public class PlayerGlobalInfo : ScriptableObject
{
    [SerializeField]
    [Tooltip("How much experience is required at level 0")]
    private float _minExperience;
    [SerializeField]
    [Tooltip("How much experience is required at level 100")]
    private float _maxExperience;
    [SerializeField]
    private AnimationCurve _experienceCurve;
    [SerializeField]
    private PlayerStatusTextElementColorAssociation[] _elementTextColors;

    public PlayerStatusTextElementColorAssociation[] ElementTextColor { get { return _elementTextColors; } }
    
    public float GetNextLevelExperience(Entity entity)
    {
        return MathUtility.GetRatioValue(_minExperience, _maxExperience, _experienceCurve.Evaluate(entity.LevelHandler.LevelNormal));
    }
}
