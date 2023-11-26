using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using UnityEngine;
using static FocusStation;

public class VirtualCursorActivator : MonoBehaviour
{
    [SerializeField] ActiveState activeState;

    [SerializeField] VirtualCursor virtualCursor;
    [SerializeField] Animator cursorAnimator;

    public static event Action<SetActiveCursorEventArgs> ActiveCursorSet;

    bool isActived;

    public enum ActiveState { MainGame, Circuitry, Arcade }

    private void OnEnable()
    {
        FocusStation.ConnectToStation += HandleConnectToStation;
        MainMenu.OnMenuStateChange += HandleMenuStateChange; 
    }

    private void OnDisable()
    {
        FocusStation.ConnectToStation -= HandleConnectToStation;
        MainMenu.OnMenuStateChange -= HandleMenuStateChange;
    }

    void HandleMenuStateChange(MainMenu.MenuState newState)
    {
        switch (activeState)
        {
            case ActiveState.MainGame:
                if (newState == MainMenu.MenuState.MainMenu || newState == MainMenu.MenuState.GameStart)
                    SetActiveCursor(true);
                break;

            case ActiveState.Circuitry:
            case ActiveState.Arcade:
                SetActiveCursor(false);
                break;
        }
    }

    void HandleConnectToStation(FocusStation.ConnectEventArgs connectEventArgs)
    {
        Debug.Log($"Cursor | Handled connect to Station {connectEventArgs.LinkedStation} | isConnecting {connectEventArgs.IsConnecting}");

        PlayerFocus.FocusedOn convertedStation = activeState switch
        {
            ActiveState.Arcade      => PlayerFocus.FocusedOn.Arcade,
            ActiveState.Circuitry   => PlayerFocus.FocusedOn.Circuitry,
            _                       => PlayerFocus.FocusedOn.Null, 
        };

        switch (activeState)
        {
            case ActiveState.MainGame:
                SetActiveCursor(!connectEventArgs.IsConnecting);
                break;

            case ActiveState.Arcade:
            case ActiveState.Circuitry:
                if (connectEventArgs.LinkedStation == convertedStation)
                    SetActiveCursor(connectEventArgs.IsConnecting);
                break;
        }
    }

    void SetActiveCursor(bool active)
    {
        Debug.Log($"Set active cursor {active}");

        isActived = active;
        virtualCursor.gameObject.SetActive(active);

        OnSetActiveCursor(active);
    }

    public void OnSetActiveCursor(bool active)
    {
        ActiveCursorSet?.Invoke(new SetActiveCursorEventArgs(gameObject, cursorAnimator, active));
    }

    public class SetActiveCursorEventArgs
    {
        public bool SetActiveCursor { get; }
        public GameObject CallingObject { get; }
        public Animator CursorAnimator { get; }


        public SetActiveCursorEventArgs(GameObject callingObject, Animator cursorAnimator, bool setActive)
        {
            CursorAnimator = cursorAnimator;
            CallingObject = callingObject;
            SetActiveCursor = setActive;
        }
    }
}
