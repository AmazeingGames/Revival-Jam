using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : Singleton<MovementManager>
{
    [SerializeField] bool controlMovement;

    public static bool ControlMovement => Instance != null && Instance.controlMovement;
}
