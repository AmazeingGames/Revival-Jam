using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathExtensions
{
    //Given a value that constrains to a given range, converts the value to a new given range, keeping the relative value consistent between ranges 
    public static float ConvertToNewRange(this float value, float oldMin, float oldMax, float newMin, float newMax)
    {
        var oldRange = (oldMax - oldMin);
        var newRange = (newMax - newMin);
        
        float newValue = ((((value - oldMin) * newRange) / oldRange) + newMin);

        return newValue;
    }

    //Overload using Vectors instead of floats
    public static float ConvertToNewRange(this float value, Vector2 oldRange, Vector2 newRange) 
        => ConvertToNewRange(value, oldRange.x, oldRange.y, newRange.x, newRange.y);

}
