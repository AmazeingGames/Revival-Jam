using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    public static Action<bool> LockMouse;

    bool isMouseLocked = false;
    
    //Custom Cursor
    public Transform mCursorVisual;
    public Vector3 mDisplacement;
    [SerializeField] Animator cursorAnimator;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        mCursorVisual.position = Input.mousePosition + mDisplacement;

        if (Input.GetMouseButtonDown(0))
        {
            cursorAnimator.SetBool("IsPressed", true);
        } else if (Input.GetMouseButtonUp(0)) cursorAnimator.SetBool("IsPressed", false);


#if DEBUG
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isMouseLocked = !isMouseLocked;

            LockMouse?.Invoke(isMouseLocked);
        }

        Cursor.visible = !isMouseLocked;
        Cursor.lockState = isMouseLocked ? CursorLockMode.Locked : CursorLockMode.None;
#endif
    }
}
