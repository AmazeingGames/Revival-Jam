using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StationMoveData;

[CreateAssetMenu(menuName = "Arrow Sprite Data")]
public class ArrowSpriteData : ScriptableObject
{
    [SerializeField] Sprite upArrow;
    [SerializeField] Sprite downArrow;
    [SerializeField] Sprite leftArrow;
    [SerializeField] Sprite rightArrow;

    public Sprite DirectionToSprite(Direction direction)
    {
        return direction switch
        {
            Direction.Up => upArrow,
            Direction.Down => downArrow,
            Direction.Left => leftArrow,
            Direction.Right => rightArrow,
            _ => throw new Exception("Switch case not accounted for.")
        };
    }
}
