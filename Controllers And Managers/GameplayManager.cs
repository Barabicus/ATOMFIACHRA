using UnityEngine;
using System.Collections;
using System;

public class GameplayManager : GameController
{

    public static GameplayManager Instance { get; set; }

    public event Action OnPlayerSpawnIn;

    public override void OnAwake()
    {
        base.OnAwake();
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Multiple gameplay managers");
            Destroy(gameObject);
        }
    }

    public void RespawnPlayer()
    {
        if (GameMainReferences.Instance.PlayerController.gameObject.activeSelf)
        {
            DoSpawnOut();
        }
        else
        {
            DoSpawnIn();
        }
    }

    private void DoSpawnOut()
    {
        var trans = ObjectTransitionPool.Instance.GetObjectFromPool(LevelMetaInfo.Instance.SpawnInFX.PoolID, (o) =>
        {
            o.TargetObject = GameMainReferences.Instance.PlayerController.gameObject;
            o.FXTransitionMethod = ObjectTransitionFx.TransitionMethod.Deactivate;
        });
        trans.OnCompleted += () => { DoSpawnIn(); };
        trans.gameObject.SetActive(true);
    }

    private void DoSpawnIn()
    {
        Entity playerEnt = GameMainReferences.Instance.PlayerController.Entity;
        //Ensure the player is on full health and alive
        playerEnt.CurrentHp = playerEnt.StatHandler.MaxHp;
        playerEnt.transform.position = GameMainReferences.Instance.PlayerSpawnPoint.transform.position;
        playerEnt.LivingState = EntityLivingState.Alive;

        GameMainReferences.Instance.RTSCamera.SetCameraPositionToOffset();

        // GameMainReferences.Instance.Player.gameObject.SetActive(true);
        var trans = ObjectTransitionPool.Instance.GetObjectFromPool(LevelMetaInfo.Instance.SpawnInFX.PoolID, (o) =>
        {
            o.TargetObject = GameMainReferences.Instance.PlayerController.gameObject;
            o.FXTransitionMethod = ObjectTransitionFx.TransitionMethod.Activate;
        });
        trans.OnCompleted += () =>
            {
                if(OnPlayerSpawnIn != null)
                {
                    OnPlayerSpawnIn();
                }
            };
        trans.gameObject.SetActive(true);
    }

}
