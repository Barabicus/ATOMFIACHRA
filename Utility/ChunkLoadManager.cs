using UnityEngine;
using System.Collections;

public class ChunkLoadManager : MonoBehaviour {


    void Update()
    {
        foreach(Transform t in transform)
        {
            if (Vector3.Distance(GameMainReferences.Instance.PlayerController.transform.position, t.position) <= 200f)
            {
                t.gameObject.SetActive(true);
            }
            else
            {
                t.gameObject.SetActive(false);
            }
        }
    }
}
