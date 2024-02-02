using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;
using static FocusStation;

//Rename: "CursorManager"
public class MouseManager : Singleton<MouseManager>
{
    [Header("Virtual Cursor")]
    [SerializeField] Vector3 cursorOffset;
    [SerializeField] float mouseFollowSensitivity;

    [SerializeField] bool getRaw;
    [SerializeField] bool normalize;

    [SerializeField] float moveDelay = .1f;
    [SerializeField] EventSystem mouseEventSystem;

    [field: Header("Debug")]
    [SerializeField] bool useVirtualMouseMovement;
    [SerializeField] bool alwaysUseVirtualMovement;


    bool isDelayOver;

    Wire wireToFollow;
    Coroutine followWire;

    Transform ActiveCursorTransform => activeCursor == null ? null : activeCursor.transform;
    VirtualCursor activeCursor;
    Animator activeAnimator;

    bool hasActiveCursor = false;

    private void OnEnable()
    {
        Wire.GrabWire += HandleWireGrab;
        VirtualCursorActivator.ActiveCursorSet += HandleSetActiveCursor;
    }

    private void OnDisable()
    {
        Wire.GrabWire -= HandleWireGrab;
        VirtualCursorActivator.ActiveCursorSet -= HandleSetActiveCursor;
    }

    //Purpose is to fix bug where mouse is very sensitive on game start
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(moveDelay);

        isDelayOver = true;

        Debug.Log("Hello World!");
    }


    // Update is called once per frame
    void Update()
    {
        VirtualCheck();

        PlayCursorAnimations();
        MoveCursor();
    }

    //Prepares the game to use either the virtual mouse or actual mouse, for both movement and input
    void VirtualCheck()
    {
        if (!hasActiveCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            return;
        }

        if (activeCursor != null)
        {
            activeCursor.CursorEventSystem.enabled = useVirtualMouseMovement;
            activeCursor.CursorClamper.enabled = useVirtualMouseMovement;
            activeCursor.ParentCanvas.renderMode = useVirtualMouseMovement ? RenderMode.ScreenSpaceCamera : RenderMode.ScreenSpaceOverlay;
        }
        if (mouseEventSystem != null)
            mouseEventSystem.gameObject.SetActive(!useVirtualMouseMovement);

        if (useVirtualMouseMovement)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
        }
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

    //Calls the proper move function for the active cursor
    void MoveCursor()
    {
        //Makes sure the game has started and that the mouse manager has a valid cursor
        if (!isDelayOver || !hasActiveCursor)
        {
            Cursor.visible = false;
            return;
        }

        //If we're holding a wire, uses the wire follow movement; otherwise stops wire movement
        if (wireToFollow != null)
        {
            followWire ??= StartCoroutine(FollowWire());
            return;
        }
        else if (followWire != null)
        {
            StopCoroutine(followWire);
            followWire = null;
        }

        //Either uses the mouse movement or mouse's actual position
        if (useVirtualMouseMovement)
            FollowMouseMovement();
        else
            FollowMousePosition();
    }

    //Instead of making this so complicated, we could just make the wire a child of the cursor and move it as normal
    //Updates the virtual activeCursor to follow the *position of the grabbed wire
    IEnumerator FollowWire()
    {
        yield return null;

        while (wireToFollow != null)
        {
            var wirePosition = wireToFollow.transform.position;
            wirePosition.z = ActiveCursorTransform.position.z;

            ActiveCursorTransform.transform.position = wirePosition;
            yield return null;
        }
    }

    //Updates the visual cursor's transform to the mouse | Regular mouse movement
    void FollowMousePosition()
    {
        if (ActiveCursorTransform == null)
            return;

        Vector3 newMousePosition = Input.mousePosition + cursorOffset;

        newMousePosition.z = ActiveCursorTransform.position.z;

        ActiveCursorTransform.position = newMousePosition;
    }

    //Updates the virtual cursor's position to follow the movement of the mouse | Virtual mouse movement
    void FollowMouseMovement()
    {
        if (ActiveCursorTransform == null)
            return;

        var sensitivity = mouseFollowSensitivity * SettingsManager.Instance.MouseSensitivity;

        ActiveCursorTransform.FollowMovement(TransformExtensions.GetMouseInput(getRaw, normalize), sensitivity, false, out _);
    }

    //Gets a reference to the wire on grab
    void HandleWireGrab(Wire wire, bool isGrab)
    {
        wireToFollow = isGrab ? wire : null;
    }

    void HandleSetActiveCursor(VirtualCursorActivator.SetActiveCursorEventArgs cursorEventArgs)
    {
        if (cursorEventArgs.SetActiveCursor)
        {
            hasActiveCursor = true;

            activeAnimator = cursorEventArgs.CursorAnimator;
            activeCursor = cursorEventArgs.VirtualCursor;

            if (alwaysUseVirtualMovement)
                useVirtualMouseMovement = true;
            else
                useVirtualMouseMovement = cursorEventArgs.VirtualCursor.MovementType == VirtualCursor.MouseType.Virtual;

            return;
        }
        
        if (activeAnimator == cursorEventArgs.CursorAnimator)
        {
            hasActiveCursor = false;

            activeAnimator = null;
            activeCursor = null;
        }
    }
}
