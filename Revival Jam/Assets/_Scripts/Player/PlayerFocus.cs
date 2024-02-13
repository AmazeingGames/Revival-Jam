using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//Potential Rename: "Focus Manager"
public class PlayerFocus : Singleton<PlayerFocus>
{
    public FocusedOn Focused { get; private set; } = FocusedOn.Nothing;
    public FocusedOn PreviouslyFocusedOn { get; private set; } = FocusedOn.Nothing;


    public enum FocusedOn { Circuitry, Arcade, Nothing, Null, FrontView, BackView, RightView, LeftView }

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
        if (!MovementManager.ControlMovement)
        {
            if (Input.GetButtonDown("Focus"))
                OnCheckStation(Focused == FocusedOn.Nothing);
        }
    }

    void OnCheckStation(bool isConnecting)
    {
        FocusAttempt?.Invoke(isConnecting);
    }

    public void HandleConnectToStation(FocusStation.ConnectEventArgs connectEventArgs)
    {
        PreviouslyFocusedOn = Focused;
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

    public static bool IsFocusedOn(FocusedOn focusedOn) => (Instance == null || focusedOn == Instance.Focused);
}
