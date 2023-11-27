using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;
using static FocusStation;

//Having so many of these could be costly in terms of performance?
//If so, it would be better to have a reference to all of the cursors and have this act as a singleton manager instead
public class VirtualCursorActivator : MonoBehaviour
{
    [SerializeField] ActiveState activeState;

    [SerializeField] VirtualInputModule virtualInput;
    [SerializeField] Animator cursorAnimator;

    public static event Action<SetActiveCursorEventArgs> ActiveCursorSet;

    bool isActived;

    public enum ActiveState { MainMenu, Pause, Circuitry, Arcade }

    private void OnEnable()
    {
        FocusStation.ConnectToStation += HandleConnectToStation;
        MenuManager.OnMenuStateChange += HandleMenuStateChange; 
    }

    private void OnDisable()
    {
        FocusStation.ConnectToStation -= HandleConnectToStation;
        MenuManager.OnMenuStateChange -= HandleMenuStateChange;
    }

    //Having a cursor for main menu and a cursor for pause seems a little redundant
    void HandleMenuStateChange(MenuManager.MenuState newState)
    {
        switch (activeState)
        {
            case ActiveState.MainMenu:
                switch (newState)
                {
                    case MenuManager.MenuState.MainMenu:
                        SetActiveCursor(true);
                        break;

                    case MenuManager.MenuState.GameStart:
                        SetActiveCursor(false);
                        break;
                }
                break;

            case ActiveState.Pause:
                switch (newState)
                {
                    case MenuManager.MenuState.Pause:
                    case MenuManager.MenuState.Settings:
                        SetActiveCursor(true);
                        break;

                    default:
                        SetActiveCursor(false); 
                        break;
                }
                break;

            case ActiveState.Circuitry:
            case ActiveState.Arcade:
                SetActiveCursor(false);
                break;
        }
    }

    void HandleConnectToStation(ConnectEventArgs connectEventArgs)
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
            case ActiveState.Pause:
            case ActiveState.MainMenu:
                SetActiveCursor(false);
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
        virtualInput.gameObject.SetActive(active);

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
