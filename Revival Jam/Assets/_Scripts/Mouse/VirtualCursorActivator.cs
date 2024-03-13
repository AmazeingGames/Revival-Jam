using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
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

    bool isFirstTime = true;
    Vector2 mousePositionRemember = new();

    public enum ActiveState { Menu = 0, Circuitry = 2, Arcade = 3, Game }

    private void OnEnable()
    {
        FocusStation.ConnectToStation += HandleConnectToStation;
        MenuManager.OnMenuStateChange += HandleMenuStateChange;
        ArcadeGameManager.AfterArcadeStateChange += HandleArcadeStateChange;
        MovementManager.OnValidation += HandleMovementManagerValidation;
    }

    private void OnDisable()
    {
        FocusStation.ConnectToStation -= HandleConnectToStation;
        MenuManager.OnMenuStateChange -= HandleMenuStateChange;
        ArcadeGameManager.AfterArcadeStateChange -= HandleArcadeStateChange;
        MovementManager.OnValidation -= HandleMovementManagerValidation;
    }

    void HandleMenuStateChange(MenuManager.MenuState menuState)
    {
        switch (activeState)
        {
            case ActiveState.Menu:
                switch (menuState)
                {
                    // Menu: True
                    // Interaction: False
                    case MenuManager.MenuState.MainMenu:
                    case MenuManager.MenuState.Pause:
                    case MenuManager.MenuState.Settings:
                    case MenuManager.MenuState.Controls:
                    case MenuManager.MenuState.Credits:
                        SetActiveCursor(true);
                        break;

                    // Do Nothing
                    case MenuManager.MenuState.PreviousState:
                        break;

                    default:
                        SetActiveCursor(false);
                        break;
                }
                break;

            case ActiveState.Circuitry:
            case ActiveState.Arcade:
            case ActiveState.Game:
                switch (menuState)
                {
                    // Extra code is needed, because this checks only for when the game enters the pause *screen*, not when the game actually becomes paused
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
        PlayerFocus.FocusedOn convertedStation = activeState switch
        {
            ActiveState.Arcade      => PlayerFocus.FocusedOn.Arcade,
            ActiveState.Circuitry   => PlayerFocus.FocusedOn.Circuitry,
            _                       => PlayerFocus.FocusedOn.Null, 
        };

        switch (activeState)
        {
            case ActiveState.Circuitry  when MovementManager.Instance.ArrowMoveType == MovementManager.ArrowMovementType.Click:
            case ActiveState.Arcade     when MovementManager.Instance.ArrowMoveType == MovementManager.ArrowMovementType.Click:
                SetActiveCursor(false);
                break;

            case ActiveState.Game       when MovementManager.Instance.ArrowMoveType == MovementManager.ArrowMovementType.Click:
                SetActiveCursor(true);
                break;

            case ActiveState.Arcade:
            case ActiveState.Circuitry:
                if (connectEventArgs.LinkedStation == convertedStation)
                    SetActiveCursor(connectEventArgs.IsConnecting);
                break;

            case ActiveState.Game:
                switch (connectEventArgs.LinkedStation)
                {
                    case PlayerFocus.FocusedOn.Arcade:
                    case PlayerFocus.FocusedOn.Circuitry:
                        if (connectEventArgs.IsConnecting)
                            SetActiveCursor(false);
                        break;

                    default:
                        SetActiveCursor(HasTool || (ItemAndAbilityManager.Instance.AreUncollectedTools() || MovementManager.Instance.ArrowMoveType ==  MovementManager.ArrowMovementType.Click));
                        break;
                }
                break;
        }
    }

    // Gets the game in a working if we change any values while the game is running
    void HandleMovementManagerValidation()
        => HandleConnectToStation(new ConnectEventArgs(PlayerFocus.Instance.Focused, true, null));

    void SetActiveCursor(bool setActive)
    {
        bool wasActive = isActive;
        isActive = setActive;
        virtualInput.gameObject.SetActive(setActive);

        OnSetActiveCursor(setActive);

        //We don't want to set the mouse position the very first time we enter the game
        if (isFirstTime)
        {
            isFirstTime = false;
            return;
        }

        if (setActive && !wasActive)
        {
            StartCoroutine(SetRealMousePosition());

            if (CursorTest.Instance != null)
                CursorTest.Instance.transform.position = mousePositionRemember;
        }
        else if (!setActive)
        {
            mousePositionRemember = Mouse.current.position.value;
        }

        //Debug.Log($"SetActiveCursor {activeState} called with params of setActive : {setActive} | wasActive : {wasActive} mousePosition");
    }

    //Keeps the cursor positions consistent
    IEnumerator SetRealMousePosition()
    {
        const float margin = 50;
        const float maxTries = 5;

        string brokeReason = string.Empty;

        int tries = 0;
        for (; tries < maxTries; tries++)
        {
            var current = Mouse.current.position.value;
            Cursor.lockState = CursorLockMode.Confined;

            //Checks if current == target
            if ((current.x > mousePositionRemember.x - margin && current.x < mousePositionRemember.x + margin) && (current.y > mousePositionRemember.y - margin && current.y < mousePositionRemember.y + margin))
                break;

            if (tries >= maxTries)
                brokeReason += "Broke due to max tries. | ";
            
            //Otherwise, curent = target
            tries++;
            Mouse.current.WarpCursorPosition(mousePositionRemember);
            yield return null;
        }

        if (brokeReason == string.Empty)
            Debug.Log($"Successfuly set mouse position after {tries} tries!");
        else
            Debug.Log(brokeReason);
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
