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
using static AudioManager.EventSounds;

public class Wire : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] WireSettings wireSettings;

    public Vector2 AddAmount { get; private set; }

    bool shouldFollowMouse = false;

    public ReceptacleObject ConnectedReceptacle { get; private set; }

    ReceptacleObject overlappingReceptacle = null;

    public static event Action<ChangeControlsEventArgs> ConnectWireCheck;
    public static event Action<Wire, bool> GrabWire; 

    Transform autoPosition = null;

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
        bool isGrab;
        AudioManager.EventSounds soundToPlay;

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

        SetMouseFollow(isGrab);
        GrabWire?.Invoke(this, isGrab);
    }

    //Preps the cursor for wire follow
    void SetMouseFollow(bool shouldFollowMouse)
    {
        this.shouldFollowMouse = shouldFollowMouse;

        if (wireSettings.HideMouse)
            UnityEngine.Cursor.visible = !shouldFollowMouse;
        
        if (wireSettings.LockMouse)
            UnityEngine.Cursor.lockState = shouldFollowMouse ? CursorLockMode.Locked : CursorLockMode.Confined;
    }

    //Adds the control from the new overlapping receptacle
    //Removes the control from the old receptacle
    void WireToConnector()
    {
        Controls controlToRemove = ConnectedReceptacle == null ? Controls.Unknown : ConnectedReceptacle.LinkedControl;

        //ReceptacleObject receptacleScript = overlappingReceptacle == null ? null : overlappingReceptacle.GetComponent<ReceptacleObject>();

        Controls controlToAdd = overlappingReceptacle == null ? Controls.Unknown : overlappingReceptacle.LinkedControl;

        ChangeControlsEventArgs controlsEventArgs = new(overlappingReceptacle, controlToAdd, controlToRemove);

        ConnectedReceptacle = overlappingReceptacle;

        Debug.Log($"is overlappingReceptacle null : {overlappingReceptacle == null}");

        ConnectWireCheck?.Invoke(controlsEventArgs);
    }

    //Given a recpetacle, manually adds that control
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

    // Update is called once per frame
    void Update()
    {
        FollowMovement(transform, wireSettings.Sensitivity, wireSettings.SetPosition, GetMouseInput());
    }

    //Updates the wire along with the mouse movement
    //Either sets the position directly or adds the position
    public void FollowMovement(Transform followingTransform, float sensitivity, bool setPosition, Vector2 movementToFollow)
    {
        if (!shouldFollowMouse)
            return;

        float addXAmount;
        float addYAmount;

        //Gets the amount to move the wires by
        addXAmount = (movementToFollow.x / 188) * Time.deltaTime * sensitivity;
        addYAmount = (movementToFollow.y / 188) * Time.deltaTime * sensitivity;

        AddAmount = new Vector2(addXAmount, addYAmount);

        Vector2 newPosition = new(followingTransform.position.x + addXAmount, followingTransform.position.y + addYAmount);

        //Updates the transform position
        if (setPosition)
        {
            followingTransform.position = new Vector3(newPosition.x, newPosition.y, followingTransform.position.z);
            ClampToBounds(followingTransform);
        }
        else
            followingTransform.position += new Vector3(addXAmount, addYAmount, 0);
    }

    //Gets the (normalized) (raw) input of the mouse
    Vector2 GetMouseInput()
    {
        float mouseX;
        float mouseY;

        if (wireSettings.GetRawAxis)
        {
            mouseX = Input.GetAxisRaw("Mouse X");
            mouseY = Input.GetAxisRaw("Mouse Y");
        }
        else
        {
            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");
        }

        Vector2 mouseInput = new(mouseX, mouseY);

        //Always normalize vector; makes movement s m o o t h
        if (wireSettings.NormalizeVector)
            mouseInput = mouseInput.normalized;

       //Debug.Log($"Mouse x : {mouseX} | Mouse y : {mouseY}");

        return mouseInput;
    }

    public void ClampToBounds(Transform transformToClamp)
    {
        Vector3 clampedPosition = transformToClamp.position;

        clampedPosition.x = Mathf.Clamp(clampedPosition.x, CircuitScreenBounds.Instance.NegativeBounds.x, CircuitScreenBounds.Instance.PositveBounds.x);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, CircuitScreenBounds.Instance.NegativeBounds.y, CircuitScreenBounds.Instance.PositveBounds.y);

        transformToClamp.position = clampedPosition;
    }


    void OnTriggerEnter(Collider other)
    {
        if (other == null) 
            return;

        if (!other.TryGetComponent<ReceptacleObject>(out var receptacleObject))
            return;

        Debug.Log("Over Receptacle");

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
