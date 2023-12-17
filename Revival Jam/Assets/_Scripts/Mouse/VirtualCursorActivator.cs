using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;
using static FocusStation;
using UnityEngine.UI;

//Having so many of these could be costly in terms of performance?
//If so, it would be better to have a reference to all of the cursors and have this act as a singleton manager instead
public class VirtualCursorActivator : MonoBehaviour
{
    [SerializeField] ActiveState activeState;

    [SerializeField] VirtualInputModule virtualInput;
    [SerializeField] Animator cursorAnimator;
    [SerializeField] VirtualCursor virtualCursor;
    Image cursorImage;

    public static event Action<SetActiveCursorEventArgs> ActiveCursorSet;

    bool isActived;
    bool activeOnPause;
    bool HasTool => HotbarManager.Instance != null && HotbarManager.Instance.GetCurrentTools().Count > 0;

    public enum ActiveState { Menu = 0, Circuitry = 2, Arcade = 3, Interaction }

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

    private void Start()
    {
        cursorImage = virtualCursor.GetComponent<Image>();
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
                    case MenuManager.MenuState.Pause:
                        activeOnPause = isActived;
                        SetActiveCursor(false);
                        break;

                    case MenuManager.MenuState.GameResume:
                        SetActiveCursor(activeOnPause);
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

    //this is the world's worst code and I hate it
    //And it doesn't even work 100% of the time

    void SetActiveCursor(bool active)
    {
        //Debug.Log($"Set active cursor {active}");

        isActived = active;

        if (virtualCursor.MovementType == VirtualCursor.MouseType.Regular && MouseManager.Instance.HasRememberedPosition)
        {
            Debug.Log("Also disabled cursor");
            virtualInput.gameObject.SetActive(false);
        }
        else
            virtualInput.gameObject.SetActive(active);

        OnSetActiveCursor(active);
    }

    public void OnSetActiveCursor(bool active)
    {
        ActiveCursorSet?.Invoke(new SetActiveCursorEventArgs(gameObject, cursorAnimator, virtualCursor, cursorImage, active));
    }

    public class SetActiveCursorEventArgs
    {
        public bool SetActiveCursor { get; }
        public GameObject CallingObject { get; }
        public Animator CursorAnimator { get; }
        public VirtualCursor VirtualCursor { get; }
        public Image Image { get; }

        public SetActiveCursorEventArgs(GameObject callingObject, Animator cursorAnimator, VirtualCursor virtualCursor, Image cursorImage, bool setActive)
        {
            CursorAnimator = cursorAnimator;
            CallingObject = callingObject;
            SetActiveCursor = setActive;
            VirtualCursor = virtualCursor;
            Image = cursorImage;
        }
    }
}
