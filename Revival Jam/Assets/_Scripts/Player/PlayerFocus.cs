using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerFocus : Singleton<PlayerFocus>
{
    public FocusedOn Focused { get; private set; } = FocusedOn.Nothing;

    public enum FocusedOn { Circuitry, Arcade, Nothing }

    public static event Action<bool> FocusAttempt;

    private void OnEnable()
    {
        FocusStation.ConnectToStation += HandleConnectToStation;

    }

    private void OnDisable()
    {
        FocusStation.ConnectToStation -= HandleConnectToStation;
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

    /*
    public void OnStationEnter(GameObject focusStation)
    {

    }

    public void OnStationExit(GameObject focusStation)
    {

    }
    */
}
