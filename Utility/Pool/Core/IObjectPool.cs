using UnityEngine;
using System.Collections;

public interface IObjectPool 
{
    void PoolObject(IPoolable obj);
}
