using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FocusStation;

public class MouseManager : MonoBehaviour
{
    [SerializeField] Transform cursor;
    [SerializeField] Vector3 cursorOffset;
    [SerializeField] Animator animator;

    bool isMouseLocked = false;
    
    private void OnEnable()
    {
        ConnectToStation += HandleConnectToStation;    
    }

    private void OnDisable()
    {
        ConnectToStation -= HandleConnectToStation;
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;

        LockMouse(false);   
    }

    void HandleConnectToStation(ConnectEventArgs connectEventArgs)
    {
        LockMouse(!connectEventArgs.IsConnecting);
    }

    void LockMouse(bool lockMouse)
    {
        switch (lockMouse)
        {
            case true:
                Cursor.lockState = CursorLockMode.Locked;
                
                break;
            
            case false:
                Cursor.lockState = CursorLockMode.None;
                break;
        }

        cursor.gameObject.SetActive(!lockMouse);
       
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            if (animator)
                animator.SetBool("IsPressed", true);

        if (Input.GetMouseButtonUp(0))
            if (animator)
                animator.SetBool("IsPressed", false);

    #if DEBUG
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isMouseLocked = !isMouseLocked;
        }

        //Cursor.visible = !isMouseLocked;
        Cursor.lockState = isMouseLocked ? CursorLockMode.Locked : CursorLockMode.None;
#endif

        cursor.position = Input.mousePosition + cursorOffset;
    }
}
