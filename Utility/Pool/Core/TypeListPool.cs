using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class TypeListPool<T> : GameController where T : MonoBehaviour, IPoolable
{

    public int preloadAmount = 10;

    public static TypeListPool<T> Instance { get; set; }

    private Dictionary<Type, Queue<T>> _objectPool;
    private Dictionary<Type, T> _objectPrefabs;

    protected abstract Dictionary<Type, T> ConstructedPrefabs { get; }

    public override void OnAwake()
    {
        Instance = this;
        _objectPool = new Dictionary<Type, Queue<T>>();
        _objectPrefabs = ConstructedPrefabs;
        PreloadPool();
    }

    private void PreloadPool()
    {
        foreach (var obj in _objectPrefabs.Values)
        {
            // Create the queue associated with the pool
            _objectPool.Add(obj.GetType(), new Queue<T>());
            for (int i = 0; i < preloadAmount; i++)
            {
                PoolObject(CreateNewObject(obj.GetType()));
            }
        }
    }

    public TA GetObjectFromPool<TA>() where TA : T
    {
        return GetObjectFromPool<TA>(null);
    }

    public TA GetObjectFromPool<TA>(System.Action<TA> preStartCallBack) where TA : T
    {
        var queue = _objectPool[typeof(TA)];
        T obj = null;
        if (queue.Count > 0)
        {
            obj = queue.Dequeue();
        }
        else
        {
            PoolObject(CreateNewObject(typeof(TA)));
            return GetObjectFromPool<TA>(preStartCallBack);
        }
        if(preStartCallBack != null)
        {
            preStartCallBack(obj as TA);
        }
        obj.PoolStart();
        return obj as TA;
    }

    private T CreateNewObject(Type poolType)
    {
        var obj = Instantiate(_objectPrefabs[poolType]);
        obj.Initialise();
        obj.gameObject.SetActive(false);
        return obj;
    }

    public void PoolObject(T obj)
    {
        obj.Recycle();
        obj.gameObject.SetActive(false);
        _objectPool[obj.GetType()].Enqueue(obj);
    }

}
