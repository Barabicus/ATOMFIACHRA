using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class FlatListPool<T> : GameController where T : MonoBehaviour, IPoolable
{

    public int preloadAmount = 10;

    public static FlatListPool<T> Instance { get; set; }

    private Stack<T> _objectPool;
    [SerializeField]
    private T _objectPrefab;

    public override void OnAwake()
    {
        Instance = this;
        _objectPool = new Stack<T>();
        PreloadPool();

    }

    private void PreloadPool()
    {
        for (int i = 0; i < preloadAmount; i++)
        {
            PoolObject(CreateNewObject());
        }
    }

    public T GetObjectFromPool()
    {
        return GetObjectFromPool(null);
    }

    public T GetObjectFromPool(Action<T> preStartCallBack)
    {
        T obj = null;
        if (_objectPool.Count > 0)
        {
            obj = _objectPool.Pop();
        }
        else
        {
            PoolObject(CreateNewObject());
            obj = GetObjectFromPool();
        }
        if (preStartCallBack != null)
        {
            preStartCallBack(obj);
        }
        OnPoolStart(obj);
        obj.PoolStart();
        return obj;
    }

    private T CreateNewObject()
    {
        var obj = Instantiate(_objectPrefab);
        obj.Initialise();
        obj.gameObject.SetActive(false);
        return obj;
    }

    public void PoolObject(T obj)
    {
        obj.Recycle();
        obj.gameObject.SetActive(false);
        _objectPool.Push(obj);
    }

    /// <summary>
    /// Call to handle custom events before PoolStart is called on the pooled object.
    /// </summary>
    protected virtual void OnPoolStart(T obj)
    {
        
    }
}
