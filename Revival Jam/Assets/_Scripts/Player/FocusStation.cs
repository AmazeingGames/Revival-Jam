using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerFocus;

public class FocusStation : MonoBehaviour
{
    [SerializeField] FocusedOn linkedStation;
    [SerializeField] Transform stationCamera;
       
    VirtualScreen linkedScreen;

    public static event Action<FocusStation, bool> StationEnter;
    public static event Action<ConnectEventArgs> ConnectToStation;

    public class ConnectEventArgs : EventArgs
    {
        public FocusedOn LinkedStation { get; private set; }
        public bool IsConnecting { get; private set; }
        public Transform StationCamera { get; private set; }

        public ConnectEventArgs(FocusedOn _linkedStation, bool _isConnecting, Transform stationCamera)
        {
            LinkedStation = _linkedStation;
            IsConnecting = _isConnecting;
            StationCamera = stationCamera;
        } 
    }

    private void OnEnable()
    {
        FocusAttempt += HandleFocusAttempt;
        VirtualScreen.FindStation += HandleFindStation;
    }

    private void OnDisable()
    {
        FocusAttempt -= HandleFocusAttempt;
        VirtualScreen.FindStation -= HandleFindStation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == null)
            return;

        if (!other.gameObject.CompareTag("Player"))
            return;

        OnPlayerEnter(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == null)
            return;

        if (!other.gameObject.CompareTag("Player"))
            return;

        OnPlayerEnter(false);
    }

    public void HandleFocusAttempt(bool isConnecting)
    {
        if (PlayerFocus.Instance.ClosestStation != this && PlayerFocus.Instance.Focused != linkedStation)
        {
            Debug.Log($"Disabled virtual screen {linkedScreen.name}");
            linkedScreen.enabled = false;
            return;
        }

        linkedScreen.enabled = true;

        ConnectToStation?.Invoke(new ConnectEventArgs(linkedStation, isConnecting, stationCamera));

        if (isConnecting)
        {
            Debug.Log($"Player connecting to {linkedScreen} station");
        }
        else
        {
            Debug.Log($"Player disconnecting from {linkedScreen} station");
        }
    }

    void HandleFindStation(VirtualScreen sender, FocusedOn virtualScreenType)
    {
        if (virtualScreenType == linkedStation)
        {
            linkedScreen = sender;
            Debug.Log($"Found screen! Linked Screen null : {linkedScreen == null}");
        }
    }

    void OnPlayerEnter(bool playerEntering)
    {
        StationEnter?.Invoke(this, playerEntering);
    }
}
