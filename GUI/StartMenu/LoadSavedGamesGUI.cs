using UnityEngine;
using System.Collections;

public class LoadSavedGamesGUI : MonoBehaviour
{
    [SerializeField]
    private Transform _container;

    [SerializeField] private SavedGameGUI _savePrefab;

    private void OnEnable()
    {
        // Empty the container
        foreach (Transform cont in _container.transform)
        {
            Destroy(cont.gameObject);
        }
        var saves = SaveGameUtility.GetAllSaves();

        foreach (var gameData in saves)
        {
            var sgg = Instantiate(_savePrefab);
            sgg.transform.parent = _container.transform;
            sgg.GameData = gameData;
        }

    }

}
