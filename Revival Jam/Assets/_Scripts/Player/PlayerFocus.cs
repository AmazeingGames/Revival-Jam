using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerFocus : Singleton<PlayerFocus>
{
    public FocusedOn Focused { get; private set; } = FocusedOn.Nothing;

    public enum FocusedOn { Circuitry, Arcade, Nothing }

    public FocusStation ClosestStation { get; private set; } = null;

    public static event Action<bool> FocusAttempt;

    private void OnEnable()
    {
        FocusStation.ConnectToStation += HandleConnectToStation;
        FocusStation.StationEnter += HandleStationEnter;
    }

    private void OnDisable()
    {
        FocusStation.ConnectToStation -= HandleConnectToStation;
        FocusStation.StationEnter -= HandleStationEnter;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Focus"))
        {
            if (Focused == FocusedOn.Nothing)
                OnCheckStation(true);
            else
                OnCheckStation(false);
        } 
    }

    void OnCheckStation(bool isConnecting)
    {
        FocusAttempt?.Invoke(isConnecting);
    }

    public void HandleConnectToStation(FocusStation.ConnectEventArgs connectEventArgs)
    {
        Focused = connectEventArgs.IsConnecting switch
        {
            true    => connectEventArgs.LinkedStation,
            _       => FocusedOn.Nothing,
        };
    }

    public void HandleStationEnter(FocusStation focusStation, bool isEntering)
    {
        ClosestStation = isEntering switch
        {
            true    => focusStation,
            _       => null,
        };
    }

    public static bool IsFocusedOn(FocusedOn focusedOn)
    {
        if (Instance == null)
        {
            //Debug.Log("Instance is null");
        }

        return (Instance == null || focusedOn == Instance.Focused);
    }
}
