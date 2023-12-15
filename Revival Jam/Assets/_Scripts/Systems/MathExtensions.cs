using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public static class MathExtensions
{
    public static float ConvertToNewRange(this float value, float oldMin, float oldMax, float newMin, float newMax)
    {
        var oldRange = (oldMax - oldMin);
        var newRange = (newMax - newMin);
        
        float newValue = ((((value - oldMin) * newRange) / oldRange) + newMin);

        return newValue;
    }

    public static float ConvertToNewRange(this float value, Vector2 oldRange, Vector2 newRange)
    {
        return ConvertToNewRange(value, oldRange.x, oldRange.y, newRange.x, newRange.y);
    }
}
