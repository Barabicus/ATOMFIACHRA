using System;
using UnityEngine;
using System.Collections;

public class EntityCastPoint : MonoBehaviour
{
    [SerializeField]
    private Transform[] _castPoints;
    [SerializeField]
    private CastPointSelectionMethod _castPointSelectionMethod = CastPointSelectionMethod.Linear;

    private int _selectedIndex;

    public enum CastPointSelectionMethod
    {
        Linear,
        Random
    }

    /// <summary>
    /// Gets the transform of the cast point the entity should be using
    /// </summary>
    public Transform CastPoint
    {
        get
        {
            Transform point = null;
            switch (_castPointSelectionMethod)
            {
                case CastPointSelectionMethod.Linear:
                    point = _castPoints[_selectedIndex];
                    _selectedIndex++;
                    if (_selectedIndex >= _castPoints.Length)
                    {
                        _selectedIndex = 0;
                    }
                    break;
                case CastPointSelectionMethod.Random:
                    point = _castPoints[UnityEngine.Random.Range(0, _castPoints.Length)];
                    break;
            }
            return point;
        }
    }

    private void Reset()
    {
        _castPoints = new Transform[1];
        var t1 = new GameObject("t1");
        t1.transform.SetParent(transform);
        t1.transform.localPosition = Vector3.zero;
        _castPoints[0] = t1.transform;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if(_castPoints == null)
        {
            return;
        }
        foreach(var p in _castPoints)
        {
            Gizmos.DrawSphere(p.position, 0.25f);
        }
    }
}
