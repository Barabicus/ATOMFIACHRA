using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class AdjustFogByPlayerDistance : MonoBehaviour {


    private Transform _player;
    private GlobalFog _fog;

    private void Start()
    {
        _fog = GetComponent<GlobalFog>();
        _player = GameMainReferences.Instance.PlayerCharacter.transform;
    }

    private void LateUpdate()
    {
        _fog.height = _player.transform.position.y - LevelMetaInfo.Instance.FogKeepDistance;
        _fog.heightDensity = LevelMetaInfo.Instance.FogHeightDensity;

    }
}
