using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using System.Dynamic;
using System.Security.Cryptography;
using System;
using static ReceptacleObject;
using static ControlsManager;
using static PlayerFocus;
using static AudioManager.OneShotSounds;

public class Wire : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] WireSettings wireSettings;

    public ReceptacleObject ConnectedReceptacle { get; private set; }

    ReceptacleObject overlappingReceptacle = null;

    public static event Action<ChangeControlsEventArgs> ConnectWireCheck;
    public static event Action<Wire, bool> GrabWire; 

    Transform autoPosition = null;
    bool isGrab;

    enum Pointer { Up, Down }

    void OnEnable()
    {
        FocusStation.ConnectToStation += HandleConnectToStation;
    }

    void OnDisable()
    {
        FocusStation.ConnectToStation -= HandleConnectToStation;
    }

    //Drags wire
    public void OnPointerDown(PointerEventData eventData)
    {
        if (PlayerFocus.Instance != null && PlayerFocus.Instance.Focused != FocusedOn.Circuitry)
            return;

        OnGrab(Pointer.Down);
    }

    //Drops & Connects wire
    public void OnPointerUp(PointerEventData eventData)
    {
        if (PlayerFocus.Instance != null && PlayerFocus.Instance.Focused != FocusedOn.Circuitry)
            return;

        DropWire();
    }

    void HandleConnectToStation(FocusStation.ConnectEventArgs connectEventArgs)
    {
        if (connectEventArgs.LinkedStation != FocusedOn.Circuitry)
            return;

        if (connectEventArgs.IsConnecting)
            return;

        DropWire();
    }

    //Stops wire mouse follow and connects it to an overlapping receptacle (if any)
    void DropWire()
    {
        OnGrab(Pointer.Up);
        WireToConnector();
    }
        
    //Called on pointer up/down
    void OnGrab(Pointer pointer)
    {
        AudioManager.OneShotSounds soundToPlay;

        switch (pointer)
        {
            case Pointer.Up:
                isGrab = false;
                soundToPlay = CircuitCablePlug;
                break;

            case Pointer.Down:
                isGrab = true;
                soundToPlay = CircuitCableUnplug;
                break;

            default:
                throw new Exception("Pointer Type Not Recognized");
        }

        AudioManager.TriggerAudioClip(soundToPlay, transform);

        GrabWire?.Invoke(this, isGrab);
    }


    //Replaces the old control with the new receptacle's control
    void WireToConnector()
    {
        Controls controlToRemove = ConnectedReceptacle == null ? Controls.Unknown : ConnectedReceptacle.LinkedControl;

        Controls controlToAdd = overlappingReceptacle == null ? Controls.Unknown : overlappingReceptacle.LinkedControl;

        ChangeControlsEventArgs controlsEventArgs = new(overlappingReceptacle, controlToAdd, controlToRemove);

        ConnectedReceptacle = overlappingReceptacle;

        ConnectWireCheck?.Invoke(controlsEventArgs);
    }

    //Manaully adds a control
    public void ManuallyConnect(ReceptacleObject receptacleToConnect)
    {
        autoPosition = receptacleToConnect.WirePosition;
        StartCoroutine(SetPositonManual());
    }

    //Used on game start to have certain wires start connected to the machine
    IEnumerator SetPositonManual()
    {
        for (int i = 0; i < 2; i++)
        {
            yield return new WaitForSeconds(.1f);

            transform.position = autoPosition.position;

            WireToConnector();
        }
        
        yield break;
    }

    void Awake()
    {
        if (ControlsManager.Instance != null)
            ControlsManager.Instance.Wires.Add(this);
        else
            Debug.LogWarning("ControlsManager.Instance is null");
    }

    private void OnTriggerStay(Collider other)
    {
        //if there's some issues with wire connections remove this.
        if (ConnectedReceptacle != null && !isGrab)
            return;

        if (other == null)
            return;

        if (!other.TryGetComponent<ReceptacleObject>(out var receptacleObject))
            return;

        overlappingReceptacle = receptacleObject;
    }

    void OnTriggerExit(Collider other)
    {
        if (other == null)
            return;

        if (overlappingReceptacle == null)
            return;

        if (other.gameObject != overlappingReceptacle.gameObject)
            return;

        Debug.Log("Left Receptacle");

        overlappingReceptacle = null;
    }

    public class ChangeControlsEventArgs
    {
        public ReceptacleObject NewWireReceptacle { get; private set; }
        public Controls ControlToAdd { get; private set; }
        public Controls ControlToRemove { get; private set; }

        public ChangeControlsEventArgs(ReceptacleObject _newWireReceptacle, Controls _controlToAdd = Controls.Unknown, Controls _controlToRemove = Controls.Unknown)
        {
            NewWireReceptacle = _newWireReceptacle;
            ControlToAdd = _controlToAdd;
            ControlToRemove = _controlToRemove;
        }

        public void SetControlToAdd(Controls controlToAdd) => ControlToAdd = controlToAdd;
    }
}
