using UnityEngine;
using System.Collections;

public class PlayerSpawnPoint : MonoBehaviour
{
    [SerializeField]
    private string _spawnPointName;

    private Entity _player;
    /// <summary>
    /// Gets the spawn position associated with this spawn point.
    /// </summary>
    public Vector3 SpawnPosition { get { return transform.position; } }

    private void Start()
    {
        _player = GameMainReferences.Instance.PlayerCharacter.Entity;
    }

    public void SetAsActiveSpawnPoint(bool quietSet = false)
    {
        GameMainReferences.Instance.PlayerSpawnPoint = this;
        if (!quietSet)
        {
            UIToast.Instance.DoSpawnPointToast(_spawnPointName);
        }
    }

    //private void Update()
    //{
    //    if (GameMainReferences.Instance.PlayerSpawnPoint != transform)
    //    {
    //        if (Vector3.Distance(_player.transform.position, transform.position) <= _setDistance)
    //        {
    //            GameMainReferences.Instance.PlayerSpawnPoint = transform;
    //            Debug.Log("Spawn point " + name + " set");
    //        }
    //    }
    //}

}
