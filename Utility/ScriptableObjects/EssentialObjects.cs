using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

public class EssentialObjects : ScriptableObject
{
    [SerializeField]
    [FormerlySerializedAsAttribute("player")]
    private PlayerController _player;
    [SerializeField]
    [FormerlySerializedAsAttribute("camera")]
    private RTSCamera _camera;
    [SerializeField]
    [FormerlySerializedAsAttribute("gameplayGUI")]
    private GameplayGUI _gameplayGUI;
    [SerializeField]
    [FormerlySerializedAsAttribute("mainGameController")]
    private GameMainController _mainGameController;

    public PlayerController Player
    {
        get
        {
            return _player;
        }
    }

    public RTSCamera Camera
    {
        get
        {
            return _camera;
        }
    }

    public GameplayGUI GameplayGUI
    {
        get
        {
            return _gameplayGUI;
        }
    }

    public GameMainController MainGameController
    {
        get
        {
            return _mainGameController;
        }
    }
}
