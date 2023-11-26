using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;
using static FocusStation;

public class MouseManager : Singleton<MouseManager>
{
    [Header("Virtual Cursor")]
    [SerializeField] Vector3 cursorOffset;
    [SerializeField] float wireFollowSensitivity;
    [SerializeField] float mouseFollowSensitivity;
    [SerializeField] bool followMouseMovement;
    [SerializeField] Transform activeCursor;
    [SerializeField] Animator activeAnimator;

    [SerializeField] bool getRaw;
    [SerializeField] bool normalize;

    [SerializeField] bool moveMouse;
    //

    VirtualMouseInput virtualMouse;

    Vector2 variable;

    Wire wireToFollow;
    Vector2 lastWirePoint;
    Coroutine followWire;

    Mouse mouse;

    private void OnEnable()
    {
        Wire.GrabWire += HandleWireGrab;
    }

    private void OnDisable()
    {
        Wire.GrabWire -= HandleWireGrab;
    }

    private void Start()
    {
        Debug.Log("INSTANCE");
    }

    // Update is called once per frame
    void Update()
    {
        PlayCursorAnimations();
        SetCursorPosition();

        
    }

    //Plays the clicking animations for the activeCursor
    void PlayCursorAnimations()
    {
        if (!activeAnimator)
            return;

        if (Input.GetMouseButtonDown(0))
            activeAnimator.SetBool("IsPressed", true);

        if (Input.GetMouseButtonUp(0))
            activeAnimator.SetBool("IsPressed", false);
    }

    //Calls the proper activeCursor move function
    void SetCursorPosition()
    {
        if (!moveMouse)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }

        if (wireToFollow != null)
        {
            followWire ??= StartCoroutine(FollowWire());
            return;
        }

        if (followWire != null)
        {
            StopCoroutine(followWire);
            followWire = null;
        }

        FollowMouseMovement();
    }
    //Updates the virtual activeCursor to follow the movement of the grabbed wire
    IEnumerator FollowWire()
    {
        lastWirePoint = new Vector2(wireToFollow.transform.position.x, wireToFollow.transform.position.y);

        yield return null;

        while (wireToFollow != null)
        {
            Vector2 wireDifference = wireToFollow.transform.PositionalDifference(lastWirePoint);

            Debug.Log($"Difference x : {wireDifference.x} | difference y : {wireDifference.y}");

            activeCursor.FollowMovement(wireDifference, wireFollowSensitivity, false, ref variable);

            lastWirePoint = new Vector2(wireToFollow.transform.position.x, wireToFollow.transform.position.y);

            yield return null;
        }
    }

    //Updates the virtual activeCursor to follow the mouse
    void FollowMouse() => activeCursor.position = Input.mousePosition + cursorOffset;

    void FollowMouseMovement()
    {
        if (activeCursor == null)
            return;

        activeCursor.FollowMovement(TransformExtensions.GetMouseInput(getRaw, normalize), mouseFollowSensitivity, false, ref variable);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    //Gets a reference to the wire on grab
    void HandleWireGrab(Wire wire, bool isGrab)
    {
        Debug.Log("HandleWireGrab");

        wireToFollow = isGrab ? wire : null;
    }
}
