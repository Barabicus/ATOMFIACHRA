using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class OnScreenImageToPointOverlay : MonoBehaviour, IPoolable
{
    [SerializeField]
    private Image _targetImage;

    private RectTransform _transform;

    public Sprite TargetSprite
    {
        set
        {
            _targetImage.overrideSprite = value;
        }
    }
    public RectTransform RectTransform { get; set; }
    public RectTransform StartTransform { get; set; }
    public RectTransform EndTransform { get; set; }

    public void Initialise()
    {
        RectTransform = GetComponent<RectTransform>();
    }

    public void PoolStart()
    {
    }

    public void Recycle()
    {
    }

    private void Update()
    {

    }
}
