using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using static StationMoveData;
using static FocusStation;
using static PlayerFocus;
using UnityEngine.UI;
using static MovementManager;
using System.Xml;
using System.Runtime.CompilerServices;

public class MoveArrow : UIButtonBase
{
    [Header("Arrow Properties")]
    [SerializeField] Direction arrowDirection;
    [SerializeField] ArrowSpriteData arrowSpriteData;

    [Header("Components")]
    [SerializeField] Image raycastImage;
    [SerializeField] Image image;

    readonly List<KeyCode> inputKeys = new();

    FocusedOn connectingStation;

    bool shouldBeDisabled;
    SpriteSwap gameCursor;

    private void OnEnable()
    {
        FocusStation.ConnectToStation += HandleConnectToStation;
        MenuManager.OnMenuStateChange += HandleMenuStateChange;
        MovementManager.OnValidation += HandleMovementValidation;
        GameMouse.GetReference += HandleGetReference;
    }

    private void OnDisable()
    {
        FocusStation.ConnectToStation -= HandleConnectToStation;
        MenuManager.OnMenuStateChange -= HandleMenuStateChange;
        MovementManager.OnValidation -= HandleMovementValidation;
        GameMouse.GetReference -= HandleGetReference;
    }

    private void Start()
    {
        image.sprite = arrowSpriteData.DirectionToSprite(arrowDirection);

        SetInputKeys();
        HandleMovementValidation();
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

    // Moves the player on WASD
    void InputCheck()
    {
        if (shouldBeDisabled || MenuManager.Instance.IsInMenu || MovementManager.Instance.ArrowMoveType == MovementManager.ArrowMovementType.Click)
            return;

        for (int i = 0; i < inputKeys.Count; i++)
        {
            if (Input.GetKeyDown(inputKeys[i]))
                MovementManager.Instance.CallConnectToStation(connectingStation);
        }
    }

    // Moves the player on mouse clicks
    public override void OnClick()
    {
        if (!CanBeClicked())
            return;

        base.OnClick(); 
        MovementManager.Instance.CallConnectToStation(connectingStation);
    }

    public override bool CanBeClicked()
        => MovementManager.Instance.ArrowMoveType != MovementManager.ArrowMovementType.WASD;

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

    // Sets the arrow's visuals and ability to be clicked
    void SetAble(bool shouldBeEnabled)
    {
        image.enabled = shouldBeEnabled;
        image.raycastTarget = shouldBeEnabled;

        raycastImage.enabled = shouldBeEnabled;
        raycastImage.raycastTarget = shouldBeEnabled;
    }

    // Secondary way to enable/disable visuals
    // Set via the Movement Manager's settings
    void HandleMovementValidation()
        => image.gameObject.SetActive(MovementManager.Instance.ArrowVisualsType == ArrowDisplayType.Arrow || MovementManager.Instance.ArrowVisualsType == ArrowDisplayType.Both);

    void HandleGetReference(SpriteSwap spriteSwap)
        => gameCursor = spriteSwap;

    public override void OnEnter()
    {
        if (MovementManager.Instance.ArrowVisualsType == ArrowDisplayType.Arrow || MovementManager.Instance.ArrowVisualsType == ArrowDisplayType.None)
            return;
        
        gameCursor.SetTemporaryVisuals(image.sprite, .7f);
    }

    public override void OnExit()
    {
        gameCursor.SetRegularVisuals();
    }
}

