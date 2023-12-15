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

    public static bool ControlMovement => Instance != null && Instance.controlMovement;

    public static event Action<FocusedOn> ConnectToStation;

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

    private void OnEnable()
    {
        GameManager.AfterStateChange += HandleGameStateChange;
    }

    private void OnDisable()
    {
        GameManager.AfterStateChange -= HandleGameStateChange;
    }

    void HandleGameStateChange(GameManager.GameState newGameState)
    {
        switch (newGameState)
        {
            case GameManager.GameState.StartGame:
                OnGameStart();
                break;
        }
    }

    //Connect to starting station, 
    void OnGameStart()
    {
        controlMovement = true;
        StartCoroutine(CallConnectToStation(startingStation));
    }

    public IEnumerator CallConnectToStation(FocusedOn stationToConnect)
    {
        yield return new WaitForSeconds(.1f);
        ConnectToStation?.Invoke(stationToConnect);
    }
}
