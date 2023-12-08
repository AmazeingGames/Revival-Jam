using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static PlayerFocus;

[CreateAssetMenu(menuName = "Station Data")]
public class StationMoveData : ScriptableObject
{
    public enum Direction { Up, Down, Left, Right }

    [field: Header("Current Station")]
    [field: SerializeField] public FocusedOn StationType { get; private set; } = FocusedOn.Null;

    [field: Header("Move Arrow Directions")]
    [field: SerializeField] private FocusedOn UpArrowStation { get; set; } = FocusedOn.Null;
    [field: SerializeField] private FocusedOn DownArrowStation { get; set; } = FocusedOn.Null;
    [field: SerializeField] private FocusedOn RightArrowStation { get; set; } = FocusedOn.Null;
    [field: SerializeField] private FocusedOn LeftArrowStation { get; set; } = FocusedOn.Null;

    
    public FocusedOn DirectionToStation(Direction direction)
    {
        return direction switch
        {
            Direction.Up => UpArrowStation,
            Direction.Down => DownArrowStation,
            Direction.Left => LeftArrowStation,
            Direction.Right => RightArrowStation,
            _ => throw new Exception("Switch case not accounted for.")
        };
    }
}
