using UnityEngine;
using System.Collections;

public interface IToastable
{
    void SetupToast(object obj);
    GameObject GameObject { get; }
    AudioClip ToastAppearedSound { get; }
}
