using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Entity))]
[RequireComponent(typeof(PlayerCharacter))]
public class PlayerController : EntityComponent
{

    #region Fields
    public float reselectDelay = 0.5f;
    public Transform respawnPoint;

    private float _lastSelectTime;
    private Entity _entity;
    private PlayerCharacter _playerCharacter;
    private bool _castSlowness;
    private Timer _slowTimer;
    private const string _slowMod = "player_cast_slowness";
    private EntityStats _slowStat;
    private bool _canControl = true;
    private Spell _costumeBaseSpellReference;
    private Timer _displayStatusTextTimer;
    private Dictionary<Element, Color> _elementTextColorsDict;

    public event Action<Element[]> OnUnlockElementsChanged;

    public bool CastSlowness
    {
        get { return _castSlowness; }
        set
        {
            // If the value has changed apply or remove the slow stat mod
            // this is to prevent multiple stat mods being added.
            if (_castSlowness != value)
            {
                if (value)
                {
                    Entity.StatHandler.ApplyStatModifier(_slowMod, _slowStat);
                }
                else
                {
                    Entity.StatHandler.RemoveAllStatModifiers(_slowMod);

                }
            }
            _castSlowness = value;
            _slowTimer.Reset();
        }
    }

    public bool CanControl
    {
        get { return _canControl; }
        set
        {
            _canControl = value;
            if (!value)
            {
                // stop in position
                Entity.EntityPathFinder.SetDestination(transform.position);
            }
        }
    }
    // An Array of elements the player has unlocked and is able to use.
    // There should only ever be 4 elements. Use SetSelectedElement to change
    // and unlocked element.
    public Element[] PlayerUnlockedElements { get; private set; }
    #endregion
    public override void Initialise()
    {
        base.Initialise();
        PlayerUnlockedElements = new Element[4];
        _displayStatusTextTimer = 2f;
        _elementTextColorsDict = Resources.Load<PlayerGlobalInfo>(ConfigPathLocations.PLAYER_GLOBAL_INFO).ElementTextColor.ToDictionary(e => e.Element, e => e.Color);
    }

    public override void OnStart()
    {
        base.OnStart();
        _slowStat = new EntityStats(-2f, 0f, ElementalStats.Zero, ElementalStats.Zero, ElementalStats.Zero, ElementalStats.Zero);
        _slowTimer = new Timer(0.25f);
        _lastSelectTime = Time.time;
        _playerCharacter = gameObject.GetComponent<PlayerCharacter>();
        _costumeBaseSpellReference = GetComponent<PlayerCostume>().CostumeBaseSpell;
        GameMainController.Instance.OnCinematicChange += OnCinematicChange;
        // Unlock all saved elements
        int eIndex = 0;
        if (GameDataManager.GameData != null)
        {
            foreach (Element e in GameDataManager.GameData.PlayerData.UnlockedElements)
            {
                SetSelectedElement(eIndex, e);
                eIndex++;
            }
        }
        else
        {
            // Load debug elements
            SetSelectedElement(0, LevelMetaInfo.Instance.FirstDebugElement);
            SetSelectedElement(1, LevelMetaInfo.Instance.SecondDebugElement);
            SetSelectedElement(2, LevelMetaInfo.Instance.ThirdDebugElement);
            SetSelectedElement(3, LevelMetaInfo.Instance.FourthDebugElement);
        }
        var p = StatusTextPool.Instance.GetObjectFromPool((e) =>
        {
            e.TextColor = Color.green;
            e.StatusText = "This is test";
            e.Target = Entity.GUIHealthPoint;
        });
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (Entity.LivingState == EntityLivingState.Alive && !GameMainController.Instance.IsPaused)
            DoUpdate();
    }

    private void DoUpdate()
    {

        if (CastSlowness && _slowTimer.CanTick)
        {
            CastSlowness = false;
        }

        if (!CanControl)
        {
            return;
        }

        if (!GameplayGUI.instance.LockPlayerControls)
            MouseControl();

        SelectSpellInput();
        Entity.LookAt(GetMouseWorldPoint());
    }

    private void OnCinematicChange(bool value)
    {
        CanControl = !value;
    }

    private Vector3 GetMouseWorldPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // create a plane at 0,0,0 whose normal points to +Y:
        Plane hPlane = new Plane(Vector3.up, transform.position);
        // Plane.Raycast stores the distance from ray.origin to the hit point in this variable:
        float distance = 0;
        // if the ray hits the plane...
        if (hPlane.Raycast(ray, out distance))
        {
            return ray.GetPoint(distance);
        }
        Debug.LogError("Did not hit");
        return Vector3.zero;
    }

    private void MouseControl()
    {

        if (Input.GetMouseButton(2))
        {
            SelectEntity();
        }

        // Pathfinding control
        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000f, 1 << LayerMask.NameToLayer("Ground")))
            {
                if (Vector3.Distance(transform.position, hit.point) > 1f)
                {
                    Entity.EntityPathFinder.SetDestination(hit.point);
                }
            }
        }
    }

    private void SelectEntity()
    {
        if (Time.time - _lastSelectTime >= reselectDelay)
        {
            // Select targeted entity
            RaycastHit hit;
            if (Physics.SphereCast(Camera.main.ScreenPointToRay(Input.mousePosition), 0.2f, out hit, 1000f, 1 << LayerMask.NameToLayer("Entity")))
            {
                GameplayGUI.instance.SelectedEntity = hit.collider.GetComponent<Entity>();
            }
            else
            {
                GameplayGUI.instance.SelectedEntity = null;
            }
            _lastSelectTime = Time.time;
        }
    }

    /// <summary>
    /// Handles casting the specified spell. It will ensure the player Entity casts the spell with the mouse coordinate passed in 
    /// as arguments. It will also ensure the player looks at the mouse and stops moving.
    /// </summary>
    /// <param name="spell"></param>
    public void CastSpellAtIndex(Spell spell, KeepAlive spellKeepAlive)
    {
        if (spell == null)
            return;

        CastSlowness = true;
        Spell castSpell;
        // Ignore spell cost is spell equals the costume's base spell.
        if (Entity.CastSpell(spell, out castSpell, null, GetMouseWorldPoint(), false))
        {
            if (castSpell != null)
                castSpell.KeepAliveDelegate = spellKeepAlive;
        }
        else if (!Entity.HasElementalChargeToCast(spell) && _displayStatusTextTimer.CanTickAndReset())
        {
            // Check what element is not sufficiant
            foreach (Element elem in Enum.GetValues(typeof(Element)))
            {
                if(Entity.CurrentElementalCharge[elem] < spell.ElementalCost[elem])
                {
                    var s = StatusTextPool.Instance.GetObjectFromPool((e) =>
                    {
                        e.TextColor = _elementTextColorsDict[elem];
                        e.Target = Entity.GUIHealthPoint.transform;
                        e.StatusText = string.Format("Not Enough {0}!", elem);
                    });
                    s.gameObject.SetActive(true);
                    // Only display one text status at the time.
                    // In this case may as well just get the first element that we need charge for.
                    return;
                }
            }

        }
    }

    public void CastSpellAtIndex(int spellIndex, KeepAlive spellKeepAlive)
    {
        var sp = _playerCharacter.GetSpellAtIndex(spellIndex);
        if (sp != null)
        {
            CastSpellAtIndex(sp, spellKeepAlive);
        }
    }

    private void SelectSpellInput()
    {
        if (Input.GetMouseButton(1))
        {
            CastSpellAtIndex(_costumeBaseSpellReference, () => { return Input.GetMouseButton(1); });
        }
        if (Input.GetKey(KeyCode.Q))
            CastSpellAtIndex(0, KeyKeepAliveFactory(KeyCode.Q));
        if (Input.GetKey(KeyCode.W))
            CastSpellAtIndex(1, KeyKeepAliveFactory(KeyCode.W));
        if (Input.GetKey(KeyCode.E))
            CastSpellAtIndex(2, KeyKeepAliveFactory(KeyCode.E));
        if (Input.GetKey(KeyCode.R))
            CastSpellAtIndex(3, KeyKeepAliveFactory(KeyCode.R));
        if (Input.GetKey(KeyCode.A))
            CastSpellAtIndex(4, KeyKeepAliveFactory(KeyCode.A));
        if (Input.GetKey(KeyCode.S))
            CastSpellAtIndex(5, KeyKeepAliveFactory(KeyCode.S));
        if (Input.GetKey(KeyCode.D))
            CastSpellAtIndex(6, KeyKeepAliveFactory(KeyCode.D));
        if (Input.GetKey(KeyCode.F))
            CastSpellAtIndex(7, KeyKeepAliveFactory(KeyCode.F));
        if (Input.GetKey(KeyCode.Alpha1))
            CastSpellAtIndex(8, KeyKeepAliveFactory(KeyCode.Alpha1));
        if (Input.GetKey(KeyCode.Alpha2))
            CastSpellAtIndex(9, KeyKeepAliveFactory(KeyCode.Alpha2));
        if (Input.GetKey(KeyCode.Alpha3))
            CastSpellAtIndex(10, KeyKeepAliveFactory(KeyCode.Alpha3));
        if (Input.GetKey(KeyCode.Alpha4))
            CastSpellAtIndex(11, KeyKeepAliveFactory(KeyCode.Alpha4));
    }

    private KeepAlive KeyKeepAliveFactory(KeyCode code)
    {
        return () => { return Input.GetKey(code); };
    }

    protected override void OnKilled(Entity e)
    {
        base.OnKilled(e);
        Invoke("Resurrect", 3f);
    }
    public void SetSelectedElement(int index, Element e)
    {
        if (index < 0 || index > 4)
        {
            Debug.LogErrorFormat("Tryng to set player element at invalid index ({0})", index);
            return;
        }
        PlayerUnlockedElements[index] = e;

        // Reset unlocked elements everytime this is set to ensure
        // only the proper elements are activated
        Entity.UnlockedElements = 0;
        foreach (Element ee in PlayerUnlockedElements)
        {
            // Re-add all unlocked elements.
            // make sure not to add an Element that is set to 0
            // since it will negate all other elements.
            if (ee == 0)
                continue;
            Entity.UnlockedElements = Entity.UnlockedElements.Add(ee);
        }
        if (OnUnlockElementsChanged != null)
        {
            OnUnlockElementsChanged(PlayerUnlockedElements);
        }
    }
    private void Resurrect()
    {
        GameplayManager.Instance.RespawnPlayer();
    }
}

[Serializable]
public struct PlayerStatusTextElementColorAssociation
{
    [SerializeField]
    private Element _element;
    [SerializeField]
    private Color _color;
    public Element Element { get { return _element; } }
    public Color Color { get { return _color; } }
}