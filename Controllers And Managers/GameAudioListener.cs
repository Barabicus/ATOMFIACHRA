using UnityEngine;
using System.Collections;
[RequireComponent(typeof(AudioListener))]
public class GameAudioListener : GameController
{
    private Transform _playerCache;

    public override void OnStart()
    {
        base.OnStart();
        _playerCache = GameMainReferences.Instance.PlayerCharacter.transform;
    }

}
