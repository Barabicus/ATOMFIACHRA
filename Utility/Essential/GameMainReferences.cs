using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class GameMainReferences : GameController
{
    private RTSCamera _rtsCamera;
    [SerializeField]
    private PlayerSpawnPoint _playerSpawnPoint;

    private PlayerController _player;
    private DepthOfField _depthOfField;

    public static GameMainReferences Instance
    {
        get;
        private set;
    }

    public PlayerController PlayerController
    {
        get { return _player; }
        set
        {
            _player = value;
            PlayerCharacter = _player.GetComponent<PlayerCharacter>();
            PlayerCostume = _player.GetComponent<PlayerCostume>();
        }
    }

    public PlayerCharacter PlayerCharacter { get; private set; }
    public PlayerCostume PlayerCostume { get; private set; }
    public PlayerSpawnPoint PlayerSpawnPoint { get { return _playerSpawnPoint; } set { _playerSpawnPoint = value; } }
    public AmplifyColorBase AmplifyColor { get; private set; }
    public DepthOfField DepthOfField { get; private set; }
    public bl_MiniMap MiniMap { get; set; }

    public RTSCamera RTSCamera
    {
        get
        {
            if(_rtsCamera == null)
            {
                _rtsCamera = GameObject.FindObjectOfType<RTSCamera>();
            }
            return _rtsCamera;
        }
    }

    public GameConfigInfo GameConfigInfo { get; set; }

    public override void OnAwake()
    {
        Instance = this;
        GameConfigInfo = Resources.LoadAll<GameConfigInfo>(ConfigPathLocations.ESSENTIAL_OBJECTS)[0];
        AmplifyColor = RTSCamera.GetComponent<AmplifyColorBase>();
        DepthOfField = RTSCamera.GetComponent<DepthOfField>();
        MiniMap = GetAndCheckReference<bl_MiniMap>();
    }

    private T GetAndCheckReference<T>() where T : Object
    {
        T reference = GameObject.FindObjectOfType<T>();
        if (reference == null)
        {
            Debug.LogErrorFormat("Game Main References could not find object for type {0}", typeof(T));
        }
        return reference;
    }
}
