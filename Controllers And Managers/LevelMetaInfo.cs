using UnityEngine;
using System.Collections;

public class LevelMetaInfo : MonoBehaviour
{
    [SerializeField]
    private bool _isGameLevel = true;
    [SerializeField]
    private string _levelName = DEFAULTNAME;
    [SerializeField]
    private AudioClip[] _musicClips;
    [SerializeField]
    private ObjectTransitionFx _spawnInFx;
    [SerializeField]
    private bool _isDebugging;
    [SerializeField]
    private bool _debugSpellCost;
    [SerializeField]
    private bool _debugGodMode;
    [SerializeField]
    private Spell[] _debugSpellUnlocks;
    [SerializeField]
    private Element firstDebugElement = Element.Fire;
    [SerializeField]
    private Element secondDebugElement = Element.Water;
    [SerializeField]
    private Element thirdDebugElement = Element.Air;
    [SerializeField]
    private Element fourthDebugElement = Element.Earth;
    [SerializeField]
    private int _setLevel = -1;
    [SerializeField]
    private float _fogKeepDistance = 1f;
    [SerializeField]
    private float _fogHeightDensity = 0f;
    [SerializeField]
    private Texture2D _startLutTexture;

    public const string DEFAULTNAME = "DEFAULT";
    private const string INSTANT_FX_PATH = "Prefabs/Fx/TransitionFX/Misc/instant_fx";

    private ObjectTransitionFx _instantFX;

    public static LevelMetaInfo Instance { get; set; }

    public bool IsGameLevel
    {
        get { return _isGameLevel; }
        set { _isGameLevel = value; }
    }

    public string LevelName { get { return _levelName; } }

    public ObjectTransitionFx SpawnInFX { get { return _spawnInFx; } }

    public bool IsDebugging { get { return _isDebugging; } set { _isDebugging = value; } }

    public float FogKeepDistance { get { return _fogKeepDistance; } }

    public float FogHeightDensity { get { return _fogHeightDensity; } }

    public Texture2D StartLutTexture { get { return _startLutTexture; } }

    public AudioClip[] MusicClips
    {
        get { return _musicClips; }
    }

    public Element FirstDebugElement
    {
        get
        {
            return firstDebugElement;
        }

        set
        {
            firstDebugElement = value;
        }
    }

    public Element SecondDebugElement
    {
        get
        {
            return secondDebugElement;
        }

        set
        {
            secondDebugElement = value;
        }
    }

    public Element ThirdDebugElement
    {
        get
        {
            return thirdDebugElement;
        }

        set
        {
            thirdDebugElement = value;
        }
    }

    public Element FourthDebugElement
    {
        get
        {
            return fourthDebugElement;
        }

        set
        {
            fourthDebugElement = value;
        }
    }

    private bool _lateStart = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _instantFX = Resources.Load<ObjectTransitionFx>(INSTANT_FX_PATH);
        if(_instantFX == null)
        {
            Debug.LogErrorFormat("Level meta info could not find instant fx check path {0}", INSTANT_FX_PATH);
        }
    }

    private void Update()
    {
        if (_isDebugging)
        {
            GameMainReferences.Instance.PlayerController.Entity.SpellsIgnoreElementalCost = _debugSpellCost;
            GameMainReferences.Instance.PlayerController.Entity.IsInvincible = _debugGodMode;
            if(_setLevel != -1)
            {
                GameMainReferences.Instance.PlayerController.Entity.LevelHandler.CurrentLevel = _setLevel;
            }
            if (!_lateStart)
            {
                LateStart();
                _lateStart = true;
            }
        }
    }

    private void LateStart()
    {
        foreach (var spell in _debugSpellUnlocks)
        {
            if(!GameMainReferences.Instance.PlayerCharacter.UnlockSpell(spell, true))
            {
                continue;
            }

            //var pc = GameMainReferences.Instance.PlayerCharacter;
            //for (int i = 0; i < PlayerCharacter.SpellToolBarCount; i++)
            //{
            //    if (spell != null && pc.GetSpellAtIndex(i) == null)
            //    {
            //        pc.SetSpellAtIndex(i, spell.SpellID);
            //        break;
            //    }
            //}

        }
    }

}
