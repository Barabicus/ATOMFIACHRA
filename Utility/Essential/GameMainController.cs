using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameMainController : MonoBehaviour
{

    public static GameMainController Instance { get; set; }

    private GameController[] _controllers;
    private LevelMetaInfo _levelMetaInfo;
    private bool _isCinematic = false;
    private bool _isPaused = false;
    public event Action<bool> OnCinematicChange;
    public event Action<bool> OnPauseChange;

    public bool IsDebuging { get; set; }
    public bool IsPaused
    {
        get
        {
            return _isPaused;
        }
        set
        {
            if (value)
            {
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = 1f;
            }
            _isPaused = value;
            if(OnPauseChange != null)
            {
                OnPauseChange(value);
            }
        }
    }
    public bool IsCinematic
    {
        get
        {
            return _isCinematic;
        }
        set
        {
            _isCinematic = value;
            if(OnCinematicChange != null)
            {
                OnCinematicChange(value);
            }
        }
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadControllers();
            if(_levelMetaInfo == null)
            {
                Debug.LogError("Level meta info does not exist");
                return;
            }
            TriggerAwake();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (Instance == this)
        {
            TriggerStart();
        }
    }

    private void Update()
    {
        if (IsDebuging)
        {
            ProcessDebug();
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        if (Instance == this)
        {
            TriggerOnLevelLoad(level);
        }
    }

    private void LoadControllers()
    {
        _controllers = transform.GetComponentsInChildren<GameController>(true);
        _levelMetaInfo = LevelMetaInfo.Instance;
    }

    private void TriggerOnLevelLoad(int levelIndex)
    {
        foreach (var gameController in _controllers)
        {
            gameController.OnLevelLoaded(levelIndex);
        }
    }

    private void TriggerAwake()
    {
        foreach (var gameController in _controllers)
        {
            if (!ProcessController(gameController))
            {
                Destroy(gameController.gameObject);
                continue;
            }
            gameController.OnAwake();
        }
    }

    private void TriggerStart()
    {
        foreach (var gameController in _controllers)
        {
            if (ProcessController(gameController))
                gameController.OnStart();

            // Remove parent as link when finished. Parenting the objects allows for ease of access initially. 
            // Remove the link to ensure they function independently when the game is running.
            gameController.transform.parent = null;
        }
    }

    private bool ProcessController(GameController controller)
    {
        if (controller.IsLevelOnly && !_levelMetaInfo.IsGameLevel)
        {
            return false;
        }
        return true;
    }

    #region Debug

    private void ProcessDebug()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
           // Time.timeScale = 1;
           if(Time.timeScale != 1f)
            {
                Time.timeScale = 1f;
            }
            else
            {
                Time.timeScale = 5f;
            }
        }
        //if(Input.GetKeyDown(KeyCode.KeypadPlus))
        //{
        //    Time.timeScale++;
        //}
        //if (Input.GetKeyDown(KeyCode.KeypadMinus))
        //{
        //    Time.timeScale = Mathf.Max(Time.timeScale - 1, 1f);
        //}

        // Quest Debug
        if (Input.GetKey(KeyCode.F))
        {
            int questIndex = -1;

            if (Input.GetKeyDown(KeyCode.Alpha1)) questIndex = 0;
            if (Input.GetKeyDown(KeyCode.Alpha2)) questIndex = 1;
            if (Input.GetKeyDown(KeyCode.Alpha3)) questIndex = 2;
            if (Input.GetKeyDown(KeyCode.Alpha4)) questIndex = 3;
            if (Input.GetKeyDown(KeyCode.Alpha5)) questIndex = 4;
            if (Input.GetKeyDown(KeyCode.Alpha6)) questIndex = 5;
            if (Input.GetKeyDown(KeyCode.Alpha7)) questIndex = 6;
            if (Input.GetKeyDown(KeyCode.Alpha8)) questIndex = 7;
            if (Input.GetKeyDown(KeyCode.Alpha9)) questIndex = 8;

            if (questIndex != -1 && questIndex < QuestManager.Instance.Quests.Count)
            {
                QuestManager.Instance.Quests[questIndex].TriggerQuestComplete();
            }
        }
    }

    #endregion

}
