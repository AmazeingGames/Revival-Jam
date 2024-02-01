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
//Also, this system seems overcomplicated
public class VirtualCursorActivator : MonoBehaviour
{
    [SerializeField] ActiveState activeState;

    [SerializeField] VirtualInputModule virtualInput;
    [SerializeField] Animator cursorAnimator;
    [SerializeField] VirtualCursor virtualCursor;

    public static event Action<SetActiveCursorEventArgs> ActiveCursorSet;

    bool isActive;
    bool wasActiveOnPause;
    bool HasTool => HotbarManager.Instance != null && HotbarManager.Instance.GetCurrentTools().Count > 0;

    public enum ActiveState { Menu = 0, Circuitry = 2, Arcade = 3, Interaction }

    private void OnEnable()
    {
        FocusStation.ConnectToStation += HandleConnectToStation;
        MenuManager.OnMenuStateChange += HandleMenuStateChange;
        ArcadeGameManager.AfterArcadeStateChange += HandleArcadeStateChange;
    }

    private void OnDisable()
    {
        FocusStation.ConnectToStation -= HandleConnectToStation;
        MenuManager.OnMenuStateChange -= HandleMenuStateChange;
        ArcadeGameManager.AfterArcadeStateChange -= HandleArcadeStateChange;

    }

    void HandleMenuStateChange(MenuManager.MenuState newState)
    {
        switch (activeState)
        {
            case ActiveState.Menu:
                switch (newState)
                {
                    //Menu: True
                    //Interaction: False
                    case MenuManager.MenuState.MainMenu:
                    case MenuManager.MenuState.Pause:
                    case MenuManager.MenuState.Settings:
                        SetActiveCursor(true);
                        break;

                    case MenuManager.MenuState.PreviousState:
                        //Do nothing
                        break;

                    default:
                        SetActiveCursor(false);
                        break;
                }
                break;

            case ActiveState.Circuitry:
            case ActiveState.Arcade:
            case ActiveState.Interaction:
                switch (newState)
                {
                    //This needs to change to have this only occur when the game actually pauses; not when we enter the pause *screen*
                    case MenuManager.MenuState.Pause:
                        if (!wasActiveOnPause)
                        {
                            wasActiveOnPause = isActive;
                            SetActiveCursor(false);
                        }
                        break;

                    case MenuManager.MenuState.GameResume:
                        SetActiveCursor(wasActiveOnPause);
                        wasActiveOnPause = false;
                        break;
                }
                break;
        }
    }

    void HandleArcadeStateChange(ArcadeGameManager.ArcadeState newArcadeState)
    {
        switch (activeState)
        {
            case ActiveState.Arcade:
                switch (newArcadeState)
                {
                    case ArcadeGameManager.ArcadeState.StartLevel:
                        SetActiveCursor(false);
                        break;
                }
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
            case ActiveState.Menu:
                break;

            case ActiveState.Arcade:
            case ActiveState.Circuitry:
                if (connectEventArgs.LinkedStation == convertedStation)
                    SetActiveCursor(connectEventArgs.IsConnecting);
                break;

            case ActiveState.Interaction:
                switch (connectEventArgs.LinkedStation)
                {
                    case PlayerFocus.FocusedOn.Arcade:
                    case PlayerFocus.FocusedOn.Circuitry:
                        SetActiveCursor(false);
                        break;

                    default:
                        SetActiveCursor(HasTool);
                        break;
                }
                break;
        }
    }

    void SetActiveCursor(bool active)
    {
        //Debug.Log($"Set {activeState} active cursor {active}");

        isActive = active;
        virtualInput.gameObject.SetActive(active);

        OnSetActiveCursor(active);
    }

    public void OnSetActiveCursor(bool active)
    {
        ActiveCursorSet?.Invoke(new SetActiveCursorEventArgs(gameObject, cursorAnimator, virtualCursor, active));
    }

    public class SetActiveCursorEventArgs
    {
        public bool SetActiveCursor { get; }
        public GameObject CallingObject { get; }
        public Animator CursorAnimator { get; }
        public VirtualCursor VirtualCursor { get; }

        public SetActiveCursorEventArgs(GameObject callingObject, Animator cursorAnimator, VirtualCursor virtualCursor, bool setActive)
        {
            CursorAnimator = cursorAnimator;
            CallingObject = callingObject;
            SetActiveCursor = setActive;
            VirtualCursor = virtualCursor;
        }
    }
}
