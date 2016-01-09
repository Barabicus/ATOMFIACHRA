using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public abstract class MoveableGameWindow : MonoBehaviour, IDragHandler
{

    private RectTransform _rectTransform;

    public RectTransform RectTransform { get { return _rectTransform; } }

    protected virtual void Awake() {
        _rectTransform = GetComponent<RectTransform>();
    }
    protected virtual void Start() { }
    protected virtual void Update() { }

    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.position += new Vector3(eventData.delta.x, eventData.delta.y);
    }

}
