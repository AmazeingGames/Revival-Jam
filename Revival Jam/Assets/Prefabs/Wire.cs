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

    bool shouldFollowMouse = false;

    ReceptacleObject connectedReceptacle;

    ReceptacleObject overlappingReceptacle = null;

    public static event Action<ChangeControlsEventArgs> ConnectWireCheck;
    public static event Action<Wire, bool> GrabWire; 

    Transform autoPosition = null;

    enum Pointer { Up, Down }

    //Drags wire on mouse down
    public void OnPointerDown(PointerEventData eventData)
    {
        if (PlayerFocus.Instance != null && PlayerFocus.Instance.Focused != FocusedOn.Circuitry)
            return;

        if (AudioManager.Instance != null)
            AudioManager.Instance.TriggerAudioClip(CircuitCableUnplug, transform);

        OnGrab(Pointer.Down);
    }

    //Lets go of wire on mouse up
    public void OnPointerUp(PointerEventData eventData)
    {
        if (PlayerFocus.Instance != null && PlayerFocus.Instance.Focused != FocusedOn.Circuitry)
            return;

        if (AudioManager.Instance != null)
            AudioManager.Instance.TriggerAudioClip(CircuitCablePlug, transform);

        OnGrab(Pointer.Up);
        WireToConnector();
    }

    //Called on pointer up/down
    void OnGrab(Pointer pointer)
    {
        bool isGrab = pointer switch
        {
            Pointer.Down => true,
            _ => false,
        };

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
        Controls controlToRemove = connectedReceptacle == null ? Controls.Unknown : connectedReceptacle.LinkedControl;

        //ReceptacleObject receptacleScript = overlappingReceptacle == null ? null : overlappingReceptacle.GetComponent<ReceptacleObject>();

        Controls controlToAdd = overlappingReceptacle == null ? Controls.Unknown : overlappingReceptacle.LinkedControl;

        ChangeControlsEventArgs controlsEventArgs = new(overlappingReceptacle, controlToAdd, controlToRemove);

        connectedReceptacle = overlappingReceptacle;

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
        FollowMouse(transform, wireSettings.Sensitivity, wireSettings.SetPosition);
    }

    //Updates the wire along with the mouse movement
    //Either sets the position directly or adds the position
    public void FollowMouse(Transform followingTransform, float sensitivity, bool setPosition)
    {
        if (!shouldFollowMouse)
            return;

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

        Debug.Log($"Mouse x : {mouseX} | Mouse y : {mouseY}");

        //Gets the amount to move the wires by
        float addXAmount = (mouseInput.x / 188) * Time.deltaTime * sensitivity;
        float addYAmount = (mouseInput.y / 188) * Time.deltaTime * sensitivity;

        float newXPosition = followingTransform.position.x + addXAmount;
        float newYPosition = followingTransform.position.y + addYAmount;

        //Makes sure wires stay between the bounds
        newXPosition = Mathf.Clamp(newXPosition, CircuitScreenBounds.Instance.NegativeBounds.x, CircuitScreenBounds.Instance.PositveBounds.x);
        newYPosition = Mathf.Clamp(newYPosition, CircuitScreenBounds.Instance.NegativeBounds.y, CircuitScreenBounds.Instance.PositveBounds.y);

        //Either adds the amount to move or sets the position directly
        //Not really a difference between the two; in fact adding is move versatile
        if (setPosition)
            followingTransform.position = new Vector3(newXPosition, newYPosition, followingTransform.position.z);
        else
            followingTransform.position += new Vector3(addXAmount, addYAmount, 0);
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
