using UnityEngine;
using System.Collections;
[DisallowMultipleComponent]
public class HideObjectsInFrontOfPlayer : MonoBehaviour {

    private Transform _player;
    
    private void Start()
    {
        _player = GameMainReferences.Instance.PlayerCharacter.transform;
    }
}
