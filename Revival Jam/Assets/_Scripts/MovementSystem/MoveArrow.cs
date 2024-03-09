using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using static StationMoveData;
using static FocusStation;
using static PlayerFocus;
using UnityEngine.UI;
using System.Xml;

public class MoveArrow : UIButtonBase
{
    [Header("Arrow Properties")]
    [SerializeField] Direction arrowDirection;
    [SerializeField] ArrowSpriteData arrowSpriteData;

    [Header("Components")]
    [SerializeField] Image image;

    readonly List<KeyCode> inputKeys = new();

    FocusedOn connectingStation;

    bool shouldBeDisabled;


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
        image.sprite = arrowSpriteData.DirectionToSprite(arrowDirection);

        SetInputKeys();
    }

    // Sets the input keys that trigger the move event
    void SetInputKeys()
    {
        switch (arrowDirection)
        {
            case Direction.Up:
                inputKeys.Add(KeyCode.W);
                inputKeys.Add(KeyCode.UpArrow);
                break;

            case Direction.Down:
                inputKeys.Add(KeyCode.S);
                inputKeys.Add(KeyCode.DownArrow);
                break;

            case Direction.Left:
                inputKeys.Add(KeyCode.A);
                inputKeys.Add(KeyCode.LeftArrow);
                break;

            case Direction.Right:
                inputKeys.Add(KeyCode.D);
                inputKeys.Add(KeyCode.RightArrow);
                break;
        }
    }

    private void Update()
        => InputCheck();

    // Triggers the move event if the player presses a move direction
    void InputCheck()
    {
        if (shouldBeDisabled || MenuManager.Instance.IsInMenu)
            return;

        for (int i = 0; i < inputKeys.Count; i++)
        {
            if (Input.GetKeyDown(inputKeys[i]))
                MovementManager.Instance.CallConnectToStation(connectingStation);
        }
    }

    // On menu change:
        // Disables the arrows while in a menu
        // Sets the arrows able while in a game, if they were already able
    void HandleMenuStateChange(MenuManager.MenuState newMenuState)
    {
        switch (newMenuState)
        {
            case MenuManager.MenuState.GameStart:
            case MenuManager.MenuState.GameResume:
                SetAble(!shouldBeDisabled);
                break;

            default:
                SetAble(false);
                break;
        }
    }

    // On connect: 
        // Gets a reference to the new station's data
        // Checks if this arrow still needs to be enabled
    void HandleConnectToStation(ConnectEventArgs eventArgs)
    {
        if (!eventArgs.IsConnecting)
            return;

        MovementManager.Instance.stationToData.TryGetValue(eventArgs.LinkedStation, out var data);

        if (data != null)
            connectingStation = data.DirectionToStation(arrowDirection);
        else
            throw new NullReferenceException("New Station data is null!");

        shouldBeDisabled = connectingStation == FocusedOn.Null;
        SetAble(!shouldBeDisabled);
    }

    // Ables the arrow's visual and clikability
    void SetAble(bool shouldBeEnabled)
    {
        image.enabled = shouldBeEnabled;
        image.raycastTarget = shouldBeEnabled;
    }
}
