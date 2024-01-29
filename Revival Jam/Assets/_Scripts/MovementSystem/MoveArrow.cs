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

    List<KeyCode> keyCodes = new();

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
        SetMovementKeys();
        SetSprite();
    }

    //Move this outside of move arrow class
    void SetMovementKeys()
    {
        switch (arrowDirection)
        {
            case Direction.Up:
                keyCodes.Add(KeyCode.W);
                keyCodes.Add(KeyCode.UpArrow);
                break;

            case Direction.Down:
                keyCodes.Add(KeyCode.S);
                keyCodes.Add(KeyCode.DownArrow);
                break;

            case Direction.Left:
                keyCodes.Add(KeyCode.A);
                keyCodes.Add(KeyCode.LeftArrow);
                break;

            case Direction.Right:
                keyCodes.Add(KeyCode.D);
                keyCodes.Add(KeyCode.RightArrow);
                break;
        }
    }

    private void Update()
    {
        MovementKeysCheck();
    }

    void MovementKeysCheck()
    {
        if (shouldBeDisabled || MenuManager.Instance.IsInMenu)
            return;

        for (int i = 0; i < keyCodes.Count; i++)
        {
            if (Input.GetKeyDown(keyCodes[i]))
                OnClick();
        }
    }

    public override void OnClick()
    {
        base.OnClick();
        StartCoroutine(MovementManager.Instance.ConnectToStationMethod(connectingStation));
    }

    void SetSprite() => image.sprite = arrowSpriteData.DirectionToSprite(arrowDirection); 

    void HandleMenuStateChange(MenuManager.MenuState newMenuState)
    {
        switch (newMenuState)
        {
            case MenuManager.MenuState.Settings:
            case MenuManager.MenuState.MainMenu:
            case MenuManager.MenuState.Pause:
                SetAble(false);
                break;

            case MenuManager.MenuState.GameResume:
                SetAble(!shouldBeDisabled);
                break;
        }
    }

    void HandleConnectToStation(ConnectEventArgs eventArgs)
    {
        Debug.Log("CONNECT TO STATION");

        if (!eventArgs.IsConnecting)
            return;

        MovementManager.Instance.stationToData.TryGetValue(eventArgs.LinkedStation, out var data);

        if (data != null)
        {
            connectingStation = data.DirectionToStation(arrowDirection);
            //Debug.Log($"{arrowDirection} Arrow : Set new connecting station ({connectingStation}) from newly connected station data ({data}) of type {data.StationType}");
        }
        else
            Debug.Log("New Station data is null!");

        //If the direction doesn't lead to a station, then disable the arrow; otherwise enable it
        shouldBeDisabled = connectingStation == FocusedOn.Null;
        SetAble(!shouldBeDisabled);
    }

    void SetAble(bool setCondition)
    {
        image.enabled = setCondition;
        image.raycastTarget = setCondition;
    }
}
