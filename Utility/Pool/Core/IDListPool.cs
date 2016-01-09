using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class IDListPool<T> : GameController where T : MonoBehaviour, IPoolableID
{

    public int preloadAmount = 10;

    public static IDListPool<T> Instance { get; set; }

    private Dictionary<string, Queue<T>> _objectPool;
    private Dictionary<string, T> _objectPrefabs;

    protected abstract Dictionary<string, T> ConstructedPrefabs { get; }

    public override void OnAwake()
    {
        Instance = this;
        _objectPool = new Dictionary<string, Queue<T>>();
        _objectPrefabs = ConstructedPrefabs;
        PreloadPool();
    }

    private void PreloadPool()
    {
        foreach (var obj in _objectPrefabs.Values)
        {
            _objectPool.Add(obj.PoolID, new Queue<T>());
            for (int i = 0; i < preloadAmount; i++)
            {
                PoolObject(CreateNewObject(obj.PoolID));
            }
        }
    }

    public T GetObjectFromPool(string poolID)
    {
        return GetObjectFromPool(poolID, null);
    }

    public T GetObjectFromPool(string poolID, System.Action<T> preStartCallBack)
    {
        var queue = _objectPool[poolID];
        T obj = null;
        if (queue.Count > 0)
        {
            obj = queue.Dequeue();
        }
        else
        {
            PoolObject(CreateNewObject(poolID));
            return GetObjectFromPool(poolID, preStartCallBack);
        }
        if (preStartCallBack != null)
        {
            preStartCallBack(obj);
        }
        obj.PoolStart();
        return obj;
    }

    private T CreateNewObject(string spellID)
    {
        var obj = Instantiate(_objectPrefabs[spellID]);
        obj.Initialise();
        obj.gameObject.SetActive(false);
        return obj;
    }

    public void PoolObject(T obj)
    {
        obj.Recycle();
        obj.gameObject.SetActive(false);
        obj.gameObject.transform.SetParent(null);
        _objectPool[obj.PoolID].Enqueue(obj);
    }

}
