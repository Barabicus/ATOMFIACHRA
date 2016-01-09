using UnityEngine;
using System.Collections;

public interface IPoolable
{
    void Initialise();
    void PoolStart();
    void Recycle();
}
