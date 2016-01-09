using UnityEngine;
using System.Collections;

public class QuestTasksOverlayCamera : MonoBehaviour
{
    public static QuestTasksOverlayCamera Instance { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    private void Update()
    {

    }
}
