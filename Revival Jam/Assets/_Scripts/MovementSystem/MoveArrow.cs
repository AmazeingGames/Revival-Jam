using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using static StationMoveData;
using static FocusStation;
using static PlayerFocus;

public class MoveArrow : UIButtonBase
{
    [SerializeField] Direction arrowDirection;
    [SerializeField] 

    FocusedOn connectingStation;

    public static event Action<FocusedOn> ConnectToStation;

    private void OnEnable()
    {
        FocusStation.ConnectToStation += HandleConnectToStation; 
    }

    private void OnDisable()
    {
        FocusStation.ConnectToStation -= HandleConnectToStation;
    }

    public override void OnClick()
    {
        base.OnClick();

        ConnectToStation?.Invoke(connectingStation);
    }

    void SetSprite()
    {

    }

    void HandleConnectToStation(ConnectEventArgs eventArgs)
    {
        if (!eventArgs.IsConnecting)
            return;

        MovementManager.Instance.stationToData.TryGetValue(eventArgs.LinkedStation, out var data);

        if (data != null)
        {
            connectingStation = data.DirectionToStation(arrowDirection);
            Debug.Log($"{arrowDirection} Arrow : Set new connecting station ({connectingStation}) from newly connected station data ({data}) of type {data.StationType}");
        }
        else
            Debug.Log("New Station data is null!");
    }
}
