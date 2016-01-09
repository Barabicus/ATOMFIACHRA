using UnityEngine;
using System.Collections;

public class GameMusicManager : GameController
{

    private AudioSource _audioSource;

    private int qSamples = 1024;  // array size
    private float refValue = 0.1f; // RMS value for 0 dB
    private float rmsValue;   // sound level - RMS
    private float dbValue;    // sound level - dB

    private float[] samples; // audio samples

    public float RMSValue
    {
        get
        {
            GetVolume();
            return rmsValue;
        }
    }

    public float DBValue
    {
        get
        {
            GetVolume();
            return dbValue;
        }
    }

    public static GameMusicManager Instance { get; private set; }

    public override void OnAwake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Warning multiple Game Music Managers!");
        }
        base.OnAwake();
        _audioSource = GameMainReferences.Instance.RTSCamera.GetComponent<AudioSource>();
        samples = new float[qSamples];
    }

    public override void OnStart()
    {
        base.OnStart();
        ChooseMusicTrack();
    }

    private void ChooseMusicTrack()
    {
        if (LevelMetaInfo.Instance.MusicClips.Length == 0)
        {
            return;
        }
        var ac = LevelMetaInfo.Instance.MusicClips[Random.Range(0, LevelMetaInfo.Instance.MusicClips.Length)];
        _audioSource.clip = ac;
        _audioSource.Play();
        // Choose new track
        Invoke("ChooseMusicTrack", ac.length);
    }

    private void GetVolume()
    {
        _audioSource.GetOutputData(samples, 0); // fill array with samples
        int i;
        var sum = 0f;
        for (i = 0; i < qSamples; i++)
        {
            sum += samples[i] * samples[i]; // sum squared samples
        }
        rmsValue = Mathf.Sqrt(sum / qSamples); // rms = square root of average
        dbValue = 20 * Mathf.Log10(rmsValue / refValue); // calculate dB
        if (dbValue < -160) dbValue = -160; // clamp it to -160dB min
    }

    //void Update()
    //{
    //    float[] spectrum = _audioSource.GetSpectrumData(1024, 0, FFTWindow.BlackmanHarris);
    //    int i = 1;
    //    while (i < 1023)
    //    {
    //        Debug.DrawLine(new Vector3(i - 1, spectrum[i] + 10, 0), new Vector3(i, spectrum[i + 1] + 10, 0), Color.red);
    //        Debug.DrawLine(new Vector3(i - 1, Mathf.Log(spectrum[i - 1]) + 10, 2), new Vector3(i, Mathf.Log(spectrum[i]) + 10, 2), Color.cyan);
    //        Debug.DrawLine(new Vector3(Mathf.Log(i - 1), spectrum[i - 1] - 10, 1), new Vector3(Mathf.Log(i), spectrum[i] - 10, 1), Color.green);
    //        Debug.DrawLine(new Vector3(Mathf.Log(i - 1), Mathf.Log(spectrum[i - 1]), 3), new Vector3(Mathf.Log(i), Mathf.Log(spectrum[i]), 3), Color.yellow);
    //        i++;
    //    }
    //}

}
