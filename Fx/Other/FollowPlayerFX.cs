using UnityEngine;
using System.Collections;

public class FollowPlayerFX : MonoBehaviour {

    private Transform _player;

    private void Start()
    {
        _player = GameMainReferences.Instance.PlayerCharacter.transform;
    }

    private void Update()
    {
        transform.position = _player.position;
    }
}
