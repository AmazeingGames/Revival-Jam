using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    public static Action<bool> LockMouse;

    bool isMouseLocked = false;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
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
