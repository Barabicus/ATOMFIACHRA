using UnityEngine;
using System.Collections;
using System;
public abstract class ToastObject<T> : MonoBehaviour, IToastable where T : class
{
    [SerializeField]
    private AudioClip _toastAppearedSound;

    private AudioSource _audioSource;

    public GameObject GameObject
    {
        get
        {
            return gameObject;
        }
    }
    public AudioClip ToastAppearedSound { get { return _toastAppearedSound; } }


    protected virtual void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    protected virtual void Start() { }
    protected virtual void Update() { }

    /// <summary>
    /// Passes in the argument obj and will create the associated toast based on the object passed in association to the toast type.
    /// </summary>
    /// <param name="obj"></param>
    public void SetupToast(object obj)
    {
        T t = obj as T;
        if (t == null)
        {
            Debug.LogError("toast object: " + obj + " is not of type: " + typeof(T));
        }
        DoSetupToast(t);
    }

    protected abstract void DoSetupToast(T obj);

}
