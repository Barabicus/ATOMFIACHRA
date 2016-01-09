using UnityEngine;
using System.Collections;
using UnityEditor;

public class EssentialsWindow : EditorWindow
{
    EssentialObjects _essentialObjects;

    private bool _isGameLevel = true;

    [MenuItem("Window/Essential GameObjects")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(EssentialsWindow));
    }

    public void OnEnable()
    {
        Find();
    }

    void Find()
    {
        var pe = Resources.LoadAll<EssentialObjects>(ConfigPathLocations.ESSENTIAL_OBJECTS);

        if (pe.Length > 0)
            _essentialObjects = pe[0];
        else
        {
            Debug.Log("Essential Objects could not be found!");
            return;
        }
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        _essentialObjects = EditorGUILayout.ObjectField(_essentialObjects, typeof(EssentialObjects)) as EssentialObjects;
        if (GUILayout.Button(new GUIContent("Find")))
        {
            Find();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginVertical();

        if (_essentialObjects == null)
            GUI.backgroundColor = Color.red;
        else
            GUI.backgroundColor = Color.green;
        _isGameLevel = GUILayout.Toggle(_isGameLevel, new GUIContent("Is Game Level"));
        if (GUILayout.Button(new GUIContent("Create Assets")) && _essentialObjects != null)
        {
            LoadObjects();
        }

        GUILayout.EndVertical();
    }
    /// <summary>
    /// T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private T CheckAndCreate<T>(T prefab) where T : Object
    {
        if (!HasSceneObject<T>())
        {
            Debug.Log("Created Object " + typeof(T));
            return PrefabUtility.InstantiatePrefab(prefab) as T;
        }
        return null;
    }

    private bool HasSceneObject<T>() where T : Object
    {
        return GameObject.FindObjectOfType<T>() != null;
    }

    private void LoadObjects()
    {
        // Create level meta object
        if (!HasSceneObject<LevelMetaInfo>())
        {
            GameObject metaInfo = new GameObject("Level Meta Info");
            var info = metaInfo.AddComponent<LevelMetaInfo>();
            info.IsGameLevel = _isGameLevel;
            Debug.Log("Created Level Meta Info");
        }
        GameMainReferences mainReferences = null;
        if (!HasSceneObject<GameMainController>())
        {
            mainReferences = CheckAndCreate(_essentialObjects.MainGameController).GetComponentInChildren<GameMainReferences>();
        }
        else
        {
            mainReferences = GameObject.FindObjectOfType<GameMainController>().GetComponentInChildren<GameMainReferences>();
        }

        if (_isGameLevel)
        {
            CheckAndCreate(_essentialObjects.Player);
            CheckAndCreate(_essentialObjects.Camera);
            CheckAndCreate(_essentialObjects.GameplayGUI);

            GameObject playerSpawnPoint = new GameObject("Player Default SpawnPoint");
            playerSpawnPoint.AddComponent<PlayerSpawnPoint>();

            mainReferences.PlayerSpawnPoint = playerSpawnPoint.GetComponent<PlayerSpawnPoint>();
        }
    }

}
