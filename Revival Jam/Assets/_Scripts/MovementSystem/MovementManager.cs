using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerFocus;

public class MovementManager : Singleton<MovementManager>
{
    [SerializeField] List<StationMoveData> movementData = new();

    [Header("Debug Settings")]
    [SerializeField] bool controlMovement = true;
    [SerializeField] FocusedOn startingStation = FocusedOn.Null;

    public readonly Dictionary<FocusedOn, StationMoveData> stationToData = new();

    public StationMoveData CurrentStationData { get; private set; }
    public static bool ControlMovement => Instance != null && Instance.controlMovement;

    private void Awake()
    {
        base.Awake();

        foreach (StationMoveData data in movementData)
        {
            if (data == null || data.StationType == FocusedOn.Null)
                continue;

            stationToData.Add(data.StationType, data);
        }
    }
}
