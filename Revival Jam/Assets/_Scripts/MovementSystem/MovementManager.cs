using System;
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
    public static event Action<FocusedOn> ConnectToStation;
    public static bool ControlMovement => Instance != null && Instance.controlMovement;

    private void Awake()
    {
        base.Awake();

        InitializeStationDictionary();
    }

    private void OnEnable()
        => GameManager.AfterStateChange += HandleGameStart;
    private void OnDisable()
        => GameManager.AfterStateChange -= HandleGameStart;

    // Starts off the game connected to a station
    void HandleGameStart(GameManager.GameState newGameState)
    {
        if (newGameState == GameManager.GameState.StartGame)
        {
            controlMovement = true;
            StartCoroutine(OnStart());
        }
    }

    // Connects to the starting station
    // Yield exists to give time for scene to load
    IEnumerator OnStart()
    {
        yield return new WaitForSeconds(.1f);
        ConnectToStation?.Invoke(startingStation);
    }

    public void CallConnectToStation(FocusedOn stationToConnect)
        => ConnectToStation?.Invoke(stationToConnect);

    // Fills out the Dictionary with data from the StationData list
    void InitializeStationDictionary()
    {
        foreach (StationMoveData data in movementData)
        {
            if (data == null || data.StationType == FocusedOn.Null)
                throw new Exception("Station data not configured properly or does not exist");
            stationToData.Add(data.StationType, data);
        }
    }
}
