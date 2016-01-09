using UnityEngine;
using System.Collections;

public abstract class GameController : MonoBehaviour
{
    [SerializeField] private bool _levelOnly = false;

    public bool IsLevelOnly
    {
        get { return _levelOnly; }
    }
    public virtual void OnAwake() { }
    public virtual void OnStart() { }

    public virtual void OnLevelLoaded(int levelIndex)
    {
    }
}
