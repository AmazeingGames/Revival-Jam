using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CircuitScreenBounds : Singleton<CircuitScreenBounds>
{
    [Header("Positive Bounds")]
    [SerializeField] Transform PositiveYBound;
    [SerializeField] Transform PositiveXBound;

    [Header("Negative Bounds")]
    [SerializeField] Transform NegativeYBound;
    [SerializeField] Transform NegativeXBound;

    public Vector2 PositveBounds { get; private set; }
    public Vector2 NegativeBounds { get; private set; }

    private void FixedUpdate()
    {
        PositveBounds = BoundsToVector(PositiveXBound, PositiveYBound);

        NegativeBounds = BoundsToVector(NegativeXBound, NegativeYBound);
    }

    //Converts the given Transform Bounds into a Vector2
    //Used to clamp the max/min position of the wires
    Vector2 BoundsToVector(Transform xBound, Transform yBound) => new(xBound.position.x, yBound.position.y);
}
