using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FocusStation;

public class MouseManager : MonoBehaviour
{
    [Header("Visual Cursor")]
    [SerializeField] Transform cursor;
    [SerializeField] Vector3 cursorOffset;
    [SerializeField] Animator animator;
    [SerializeField] float wireFollowSensitivity;

    Wire wireToFollow;

    Vector2 lastWirePoint;

    Coroutine followWire;
    private void OnEnable()
    {
        Wire.GrabWire += HandleWireGrab;
    }

    private void OnDisable()
    {
        Wire.GrabWire -= HandleWireGrab;
    }

    void Start()
    {
        //Cursor.lockState = CursorLockMode.Confined;
    }

    // Update is called once per frame
    void Update()
    {
        PlayCursorAnimations();
        SetCursorPosition();
        ToggleMouseSettings();
    }

    //Plays the clicking animations for the cursor
    void PlayCursorAnimations()
    {
        if (!animator)
            return;

        if (Input.GetMouseButtonDown(0))
            animator.SetBool("IsPressed", true);

        if (Input.GetMouseButtonUp(0))
            animator.SetBool("IsPressed", false);
    }

    //Calls the proper cursor move function
    void SetCursorPosition()
    {
        if (wireToFollow == null)
        {
            if (followWire != null)
            {
                StopCoroutine(followWire);
                followWire = null;
            }
            
            FollowMouse();
        }
        else followWire ??= StartCoroutine(FollowWire());
    }

    //Updates the virtual cursor to follow the movement of the grabbed wire
    IEnumerator FollowWire()
    {
        lastWirePoint = new Vector2(wireToFollow.transform.position.x, wireToFollow.transform.position.y);

        yield return null;

        while (true)
        {
            float differenceX = wireToFollow.transform.position.x - lastWirePoint.x;
            float differenceY = wireToFollow.transform.position.y - lastWirePoint.y;

            Vector2 wireDifference = new(differenceX, differenceY);

            Debug.Log($"Difference x : {differenceX} | difference y : {differenceY}");

            wireToFollow.FollowMovement(cursor, wireFollowSensitivity, setPosition: false, movementToFollow: wireDifference);

            lastWirePoint = new Vector2(wireToFollow.transform.position.x, wireToFollow.transform.position.y);

            yield return null;
        }
    }

    //Updates the virtual cursor to follow the mouse
    void FollowMouse() => cursor.position = Input.mousePosition + cursorOffset;

    //Change mouse visibility for debugging
    void ToggleMouseSettings()
    {
        #if DEBUG
        if (Input.GetKeyDown(KeyCode.Escape))
            Cursor.visible = !Cursor.visible;
        #endif
    }

    //Gets a reference to the wire on grab
    void HandleWireGrab(Wire wire, bool isGrab)
    {
        Debug.Log("HandleWireGrab");

        wireToFollow = isGrab ? wire : null;
    }
}
