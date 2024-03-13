using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerFocus;

public class MovementManager : Singleton<MovementManager>
{
    public enum ArrowMovementType { Click, WASD, Both }
    public enum ArrowDisplayType { Arrow, Cursor, Both, None }

    [SerializeField] List<StationMoveData> movementData = new();

    [field: Header("Settings")]
    [field: SerializeField] public ArrowMovementType ArrowMoveType { get; private set; }
    [field: SerializeField] public ArrowDisplayType ArrowVisualsType { get; private set; }

    [SerializeField] bool controlMovement = true;
    [SerializeField] FocusedOn startingStation = FocusedOn.Null;

    public readonly Dictionary<FocusedOn, StationMoveData> stationToData = new();
    public static event Action<FocusedOn> ConnectToStation;
    public static bool ControlMovement => Instance != null && Instance.controlMovement;
    public static event Action OnValidation;

    private void Start()
        => InitializeStationDictionary();
    private void OnEnable()
        => GameManager.AfterStateChange += HandleGameStart;
    private void OnDisable()
        => GameManager.AfterStateChange -= HandleGameStart;

    // Connects to a station on game start
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
        CallConnectToStation(startingStation);
    }

    // Gives public access to invoke the connectToStation event
    public void CallConnectToStation(FocusedOn stationToConnect)
    {
        Debug.Log($"connecting to : {stationToConnect}");

        ConnectToStation?.Invoke(stationToConnect);
    }

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

    private void OnValidate()
        => OnValidation?.Invoke();
}
