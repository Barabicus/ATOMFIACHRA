using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUILoad : MonoBehaviour
{
    [SerializeField]
    private Image _progressBar;
    [SerializeField]
    private Text _loadingAmount;

    private int _loadingProgress;
    private AsyncOperation _async;
    private static string level;

    private const string LOADING_SCENE_ID = "LoadingWindow";
    /// <summary>
    /// Called when the actual loading scene starts.
    /// This will trigger the target level to be loaded.
    /// </summary>
    private void Start()
    {
        StartCoroutine(Load());
    }

    private void Update()
    {
        if (_async == null)
        {
            return;
        }
        _loadingProgress = (int)(_async.progress * 100f);
        _loadingAmount.text = _loadingProgress.ToString() + "%";
        // ProgressBar.anchorMax = new Vector2(GetPercent(_loadingProgress, 100f), ProgressBar.anchorMax.y);
        _progressBar.fillAmount = _async.progress;
    }

    public static void LoadLevel(string name)
    {
        level = name;
        // Check to see if a load was triggered from a game scene
        // and make sure to save the game before loading.
        if (LevelMetaInfo.Instance.IsGameLevel)
        {
            GameDataManager.Instance.UpdateGameData();
            SaveGameUtility.SaveToDisk(GameDataManager.GameData);
        }
        // Load the scene for handling loading transitions
        Application.LoadLevel(LOADING_SCENE_ID);
    }

    private IEnumerator Load()
    {
        _async = Application.LoadLevelAsync(level);
        yield return _async;
    }

}
