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

    private void OnEnable()
    {
        Wire.GrabWire += HandleWireGrab;
    }

    private void OnDisable()
    {
        Wire.GrabWire -= HandleWireGrab;
    }

    // Update is called once per frame
    void Update()
    {
        PlayCursorAnimations();
        SetCursorPosition();
        ToggleMouseSettings();
    }

    //Sets cursor animations on click
    void PlayCursorAnimations()
    {
        if (!animator)
            return;

        if (Input.GetMouseButtonDown(0))
            animator.SetBool("IsPressed", true);

        if (Input.GetMouseButtonUp(0))
            animator.SetBool("IsPressed", false);
    }

    //Sets cursor to follow mouse
    void SetCursorPosition()
    {
        if (wireToFollow == null)
            FollowMouse();
        else
            FollowWire();
    }

    void FollowWire()
    {
        wireToFollow.FollowMouse(cursor, wireFollowSensitivity, false);
    }

    void FollowMouse() => cursor.position = Input.mousePosition + cursorOffset;

    //Change mouse visibility for debugging
    void ToggleMouseSettings()
    {
        #if DEBUG
        if (Input.GetKeyDown(KeyCode.Escape))
            Cursor.visible = !Cursor.visible;
        #endif
    }

    void HandleWireGrab(Wire wire, bool isGrab)
    {
        Debug.Log("HandleWireGrab");

        wireToFollow = isGrab ? wire : null;
    }
}
