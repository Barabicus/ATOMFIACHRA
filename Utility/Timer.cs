using UnityEngine;
using System.Collections;

public class Timer
{
    private float _currentTime;
    private bool _isStopped = false;

    /// <summary>
    /// Get or set how long the tick time should take
    /// </summary>
    public float TickTime { get; set; }
    /// <summary>
    /// Start is stop the timer
    /// </summary>
    public bool IsStopped
    {
        get { return _isStopped; }
        set { _isStopped = value; }
    }
    /// <summary>
    /// Returns true if the timer can Tick
    /// </summary>
    public bool CanTick
    {
        get
        {
            if (IsStopped)
                return false;
            if (Time.time - _currentTime >= TickTime)
            {
                return true;
            }
            return false;
        }
    }
    /// <summary>
    /// Gets the current time represented as a normal. 0 being the start and 1 being the tick time.
    /// </summary>
    public float CurrentTimeNormal
    {
        get { return (Time.time - _currentTime) / TickTime; }
    }
    /// <summary>
    /// Returns how long the Timer has been ticking for
    /// </summary>
    public float CurrentTime
    {
        get { return (Time.time - _currentTime); }
    }
    /// <summary>
    /// Returns how long is left for a tick to occur. Returns 0f it is able to tick.
    /// </summary>
    public float RemainingTickTime
    {
        get { return Mathf.Max(TickTime - CurrentTime, 0f); }
    }
    /// <summary>
    /// Check to see if the timer has triggered a tick.
    /// If it can tick it will return true and reset the timer
    /// </summary>
    public bool CanTickAndReset()
    {
        if (Time.time - _currentTime >= TickTime)
        {
            _currentTime = Time.time;
            return true;
        }
        return false;
    }

    public void Reset()
    {
        _currentTime = Time.time;
    }

    public void ForceTickTime()
    {
        _currentTime -= TickTime;
    }

    public Timer(float tickTime)
    {
        _currentTime = Time.time;
        TickTime = tickTime;

    }

    public static implicit operator Timer(float time)
    {
        Timer timer = new Timer(time);
        return timer;
    }

    public override string ToString()
    {
        return "(" + TickTime + " : " + (Time.time - _currentTime) + ")";
    }

}
