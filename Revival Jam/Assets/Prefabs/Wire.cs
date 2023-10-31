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
using UnityEngine.InputSystem;

public class Wire : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] float sensitivity;
    [SerializeField] string receptacleTag;

    [SerializeField] bool shouldSkipUpdates;
    [SerializeField] float timeBetweenUpdateSkips;

    bool shouldFollowMouse = false;

    private Vector2? lastMousePoint = null;

    ReceptacleObject connectedReceptacle;

    ReceptacleObject overlappingReceptacle = null;

    public static event Action<ChangeControlsEventArgs> ConnectWireCheck;

    Transform autoPosition = null;

    float timer;
    Vector2 grabPosition;
    bool skipUpdate = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (PlayerFocus.Instance != null && PlayerFocus.Instance.Focused != FocusedOn.Circuitry)
            return;

        if (AudioManager.Instance != null)
            AudioManager.Instance.TriggerAudioClip(CircuitCableUnplug, transform);

        SetMouseFollow(true);

        lastMousePoint = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (PlayerFocus.Instance != null && PlayerFocus.Instance.Focused != FocusedOn.Circuitry)
            return;

        if (AudioManager.Instance != null)
            AudioManager.Instance.TriggerAudioClip(CircuitCablePlug, transform);

        SetMouseFollow(false);

        WireToConnector();
    }

    void SetMouseFollow(bool shouldFollowMouse)
    {
        this.shouldFollowMouse = shouldFollowMouse;

        UnityEngine.Cursor.visible = !shouldFollowMouse;
        
        //UnityEngine.Cursor.lockState = CursorLockMode.Confined;

        grabPosition = Input.mousePosition;
    }

    //'WireToControl' name instead (?)
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
        FollowMouse();
    }

    void FollowMouse()
    {
        if (!shouldFollowMouse)
            return;
        
        //Compares the mouse's current position with its last position
        float differenceX = Input.mousePosition.x - lastMousePoint.Value.x;
        float differenceY = Input.mousePosition.y - lastMousePoint.Value.y;

        //Updates the wire position with the mouse movement
        float newXPosition = transform.position.x + (differenceX / 188) * Time.deltaTime * sensitivity;
        float newYPosition = transform.position.y + (differenceY / 188) * Time.deltaTime * sensitivity;

        //Makes sure wires stay between the bounds
        newXPosition = Mathf.Clamp(newXPosition, CircuitScreenBounds.Instance.NegativeBounds.x, CircuitScreenBounds.Instance.PositveBounds.x);
        newYPosition = Mathf.Clamp(newYPosition, CircuitScreenBounds.Instance.NegativeBounds.y, CircuitScreenBounds.Instance.PositveBounds.y);

        if (skipUpdate)
            skipUpdate = false;
        else
            transform.position = new Vector3(newXPosition, newYPosition, transform.position.z);

        if (shouldSkipUpdates)
            timer += Time.deltaTime;

        if (timer > timeBetweenUpdateSkips)
        {
            Debug.Log("teleport");

            timer = 0;
            skipUpdate = true;
            Mouse.current.WarpCursorPosition((Vector2)grabPosition);
        }

        lastMousePoint = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
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
