using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class GameplayGUI : MonoBehaviour
{

    public static GameplayGUI instance;

    public Transform selectedTransform;
    public RectTransform healthAmount;
    public RectTransform experienceAmount;

    public Text playerHealthText;
    public Animator LevelUpAnimator;
    public Animator GameSaveAnimator;
    public RectTransform tipContainer;
    public Animator CinematicAnimator;
    public CanvasGroup[] fadeGroupsCinematic;

    public Text selectedEntityText;

    public event Action<Spell, int> SpellChanged;

    private bool _isMouseOver;
    private bool _mouseDragLock;

    private Entity _selectedEntity;
    private PlayerController _player;
    private PlayerCharacter _playerCharacter;

    public Entity SelectedEntity
    {
        get { return _selectedEntity; }
        set { _selectedEntity = value; }
    }

    public HitTextProperties HitTextProperties
    {
        get;
        private set;
    }

    public bool LockPlayerControls
    {
        get
        {
            return IsMouseOver || MouseDragLock;
        }
    }

    public bool IsMouseOver
    {
        get
        {
            return _isMouseOver;
        }
        set
        {
            _isMouseOver = value;
        }
    }

    public bool MouseDragLock
    {
        get
        {
            return _mouseDragLock;
        }
        set
        {
            _mouseDragLock = value;
        }
    }
    private void Awake()
    {
        instance = this;
        HitTextProperties = Resources.LoadAll<HitTextProperties>("Utility/GUI")[0];

    }

    private void Start()
    {
        _player = GameMainReferences.Instance.PlayerController;
        _playerCharacter = GameMainReferences.Instance.PlayerCharacter;
        _playerCharacter.OnLevelUp += _playerCharacter_OnLevelUp;
        GameMainController.Instance.OnCinematicChange += OnCinematicChange;
    }

    private void OnCinematicChange(bool value)
    {
        CinematicAnimator.SetBool("isCinematic", value);
    }

    private void _playerCharacter_OnLevelUp(int level)
    {
        LevelUpAnimator.SetTrigger("levelup");
    }

    void Update()
    {
        UpdatePlayer();
        UpdateTargeted();
        FadeCinematicGroups();
    }

    private void FadeCinematicGroups()
    {
        foreach (var cg in fadeGroupsCinematic)
        {
            cg.alpha = Mathf.Lerp(cg.alpha, GameMainController.Instance.IsCinematic ? 0f : 1f, Time.deltaTime * 10f);
        }
    }

    private void UpdatePlayer()
    {
    //    playerHealth.anchorMax = new Vector2(GetPercent(_player.Entity.CurrentHp, _player.Entity.StatHandler.MaxHp), playerHealth.anchorMax.y);
        experienceAmount.anchorMax = new Vector2(GetPercent(_playerCharacter.CurrentExperience, _playerCharacter.NextLevelExperience), experienceAmount.anchorMax.y);
        playerHealthText.text = Mathf.Ceil(_player.Entity.CurrentHp) + " : " + Mathf.Ceil(_player.Entity.StatHandler.MaxHp);

        //fireText.text = Mathf.Floor(_player.Entity.CurrentElementalCharge.fire).ToString();
        //waterText.text = Mathf.Floor(_player.Entity.CurrentElementalCharge.water).ToString();
        //earthText.text = Mathf.Floor(_player.Entity.CurrentElementalCharge.earth).ToString();
        //airText.text = Mathf.Floor(_player.Entity.CurrentElementalCharge.air).ToString();
    }

    private void UpdateTargeted()
    {
        if (SelectedEntity == null)
        {
            selectedTransform.gameObject.SetActive(false);
            return;
        }
        else
        {
            selectedTransform.gameObject.SetActive(true);
        }

        if (SelectedEntity != null)
            selectedEntityText.text = SelectedEntity.EntityName + " lvl: " + SelectedEntity.LevelHandler.CurrentLevel;
        else
            selectedEntityText.text = "";

        // Update Health Amount
        healthAmount.anchorMax = new Vector2(GetPercent(SelectedEntity.CurrentHp, SelectedEntity.StatHandler.MaxHp), healthAmount.anchorMax.y);

    }

    private float GetPercent(float value, float max)
    {
        return (value / max);
    }

    public void SetMouseOver(bool value)
    {
        IsMouseOver = value;
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    public void LoadMainLevel()
    {
        Application.LoadLevel(0);
    }

    public void TriggerSave()
    {
        GameDataManager.Instance.UpdateGameData();
        SaveGameUtility.SaveToDisk(GameDataManager.GameData);
        GameSaveAnimator.SetTrigger("saved");
    }

}
