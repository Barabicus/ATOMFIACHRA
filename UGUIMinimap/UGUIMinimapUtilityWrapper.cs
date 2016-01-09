using UnityEngine;
using System.Collections;
/// <summary>
/// Utility tool for handling minimap based icons a bit better.
/// </summary>
public class UGUIMinimapUtilityWrapper
{
    private bl_MiniMapItem _minimapItem;

    public Transform Target { get { return _minimapItem.Target; } set { _minimapItem.Target = value; } }
    public CanvasGroup IconCanvasGroup { get; private set; }
    public bl_IconItem IconItem { get; private set; }
    public Color IconColor
    {
        get
        {
            return _minimapItem.GraphicImage.color;
        }
        set
        {
            _minimapItem.GraphicImage.color = value;
        }
    }
    public Sprite IconSprite
    {
        get { return IconItem.TargetGrapihc.sprite; }
        set { IconItem.TargetGrapihc.sprite = value; }
    }

    public UGUIMinimapUtilityWrapper(bl_MiniMapItem minimapItem)
    {
        this._minimapItem = minimapItem;
        // Ensure the icon has been created
        _minimapItem.TriggerCreateIcon();
        IconItem = _minimapItem.GraphicImage.GetComponent<bl_IconItem>();
        IconCanvasGroup = IconItem.GetComponent<CanvasGroup>();

    }
}
