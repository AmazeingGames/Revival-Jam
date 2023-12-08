using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerFocus;
using static AudioManager;
using UnityEngine.EventSystems;

public class FocusStation : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] FocusedOn linkedStation;
    [SerializeField] Transform stationCamera;

    [Header("Sound FX")]
    [SerializeField] EventSounds stationEnterSound = EventSounds.Null;

    [Header("Debug")]
    [SerializeField] KeyCode stationKey;

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
        MoveArrow.ConnectToStation += HandleMoveFocusAttempt;

        FocusAttempt += HandlePlayerFocusAttempt;
        VirtualScreen.FindStation += HandleFindStation;
    }

    private void OnDisable()
    {
        MoveArrow.ConnectToStation -= HandleMoveFocusAttempt;

        FocusAttempt -= HandlePlayerFocusAttempt;
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

    public void HandlePlayerFocusAttempt(bool isConnecting)
    {
        if (linkedScreen == null)
            return;

        //Neither connecting nor disconnecting
        if (PlayerFocus.Instance.ClosestStation != this && PlayerFocus.Instance.Focused != linkedStation)
        {
            linkedScreen.enabled = false;
            return;
        }

        //Either connecting or disconnecting
        linkedScreen.enabled = true;
        ConnectToStation?.Invoke(new ConnectEventArgs(linkedStation, isConnecting, stationCamera));

        if (isConnecting)
            TriggerAudioClip(stationEnterSound, transform);
    }

    public void HandleMoveFocusAttempt(FocusedOn stationType)
    {
        if (linkedScreen == null)
            return;

        //Neither connecting nor disconnecting
        if (stationType != linkedStation && PlayerFocus.Instance.Focused != linkedStation)
        {
            linkedScreen.enabled = false;
            return;
        }

        bool isConnecting = stationType == linkedStation;

        //Either connecting or disconnecting
        linkedScreen.enabled = true;
        ConnectToStation?.Invoke(new ConnectEventArgs(linkedStation, isConnecting, stationCamera));

        if (isConnecting)
            TriggerAudioClip(stationEnterSound, transform);
    }

    void HandleFindStation(VirtualScreen sender, FocusedOn virtualScreenType)
    {
        if (virtualScreenType == linkedStation)
        {
            linkedScreen = sender;
            Debug.Log($"Found screen! Linked Screen null : {linkedScreen == null}");

            linkedScreen.enabled = false;
        }
    }

    void OnPlayerEnter(bool playerEntering)
    {
        StationEnter?.Invoke(this, playerEntering);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Mouse clicked interface");
    }
}
