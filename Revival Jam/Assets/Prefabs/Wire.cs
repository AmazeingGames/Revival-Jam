using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;
using System.Dynamic;
using System.Security.Cryptography;
using System;
using static ReceptacleObject;
using static ControlsManager;

public class Wire : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] float sensitivity;
    [SerializeField] string receptacleTag;

    bool shouldFollowMouse = false;

    private Vector2? lastMousePoint = null;

    ReceptacleObject connectedReceptacle;

    ReceptacleObject overlappingReceptacle = null;

    public static event Action<ChangeControlsEventArgs> ConnectWireCheck;

    Transform autoPosition = null;

    public void OnPointerDown(PointerEventData eventData)
    {
        shouldFollowMouse = true;

        lastMousePoint = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    }

    //TO DO: MAKE SURE THIS ADDS AND REMOVES ABILITES PROPERLY
    public void OnPointerUp(PointerEventData eventData)
    {
        shouldFollowMouse = false;

        WireToConnector();
    }

    //'WireToControl' name instead (?)
    void WireToConnector()
    {
        Controls controlToRemove = connectedReceptacle == null ? Controls.Unknown : connectedReceptacle.LinkedControl;

        //ReceptacleObject receptacleScript = overlappingReceptacle == null ? null : overlappingReceptacle.GetComponent<ReceptacleObject>();

        Controls controlToAdd = overlappingReceptacle == null ? Controls.Unknown : overlappingReceptacle.LinkedControl;

        ChangeControlsEventArgs controlsEventArgs = new(overlappingReceptacle, controlToAdd, controlToRemove);

        ConnectWireCheck?.Invoke(controlsEventArgs);

        connectedReceptacle = overlappingReceptacle;
    }

    public void ManuallyConnect(ReceptacleObject receptacleToConnect)
    {
        StartCoroutine(SetPositonManual());
        autoPosition = receptacleToConnect.WirePosition;

    }

    IEnumerator SetPositonManual()
    {
        for (int i = 0; i < 2; i++)
        {
            yield return null;

            transform.position = autoPosition.position;

            WireToConnector();

        }
        
        yield break;
    }

    void Awake()
    {
        ControlsManager.Instance.Wires.Add(this);
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

        float differenceX = Input.mousePosition.x - lastMousePoint.Value.x;

        float differenceY = Input.mousePosition.y - lastMousePoint.Value.y;

        transform.position = new Vector3(transform.position.x + (differenceX / 188) * Time.deltaTime * sensitivity, transform.position.y + (differenceY / 188) * Time.deltaTime * sensitivity, transform.position.z);

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
        if (other.gameObject == overlappingReceptacle.gameObject)
        {
            Debug.Log("Left Receptacle");
            overlappingReceptacle = null;
        }
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
