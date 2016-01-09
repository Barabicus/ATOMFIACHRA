using UnityEngine;
using System.Collections;

public static class MathUtility  {
    /// <summary>
    /// Returns a percent from value to max. For example a value of 50 with a max of 100 will return 0.5
    /// Percents are returned with 1 representing 100%
    /// </summary>
    /// <param name="value"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static float GetPercent(float value, float max)
    {
        return (value / max);
    }

    /// <summary>
    /// Returns a ratio value from min to max. This returns a ratio in between the two numbers for example 0 - 100 and a ratio of
    /// 0.5 will return 50 like a percent would but 50 - 100 and a ratio of 0.5 will return 75 which is the mid point ratio between 50 and 100.
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="ratio"></param>
    /// <returns></returns>
    public static float GetRatioValue(float min, float max, float ratio)
    {
        return (ratio * (max - min)) + min;
    }
}
