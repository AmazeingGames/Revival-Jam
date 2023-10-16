using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public enum RotationAxes { MouseXY, MouseX, MouseY }

    public RotationAxes axes = RotationAxes.MouseXY;

    public float sensitvityVer = 9f;
    public float sensitivityHor = 9f;

    public float minimumVert = -45f;
    public float maximumVert = 45f;

    float verticalRot = 0;

    bool isLocked = false;

    void CalcVertRot()
    {
        verticalRot -= Input.GetAxis("Mouse Y") * sensitvityVer;
        verticalRot = Mathf.Clamp(verticalRot, minimumVert, maximumVert);
    }

    float GetXRot()
    {
        return Input.GetAxis("Mouse X") * sensitivityHor;
    }

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody body = GetComponent<Rigidbody>();
        if(body != null)
        {
            body.freezeRotation = true;
        }
    }

    private void OnEnable()
    {
    #if DEBUG
        MouseManager.LockMouse += OnMouseLock;
    #endif
    }

    private void OnDisable()
    {
    #if DEBUG
        MouseManager.LockMouse -= OnMouseLock;
    #endif
    }

    // Update is called once per frame
    void Update()
    {
        Look();
    }

    void Look()
    {
        if (isLocked)
            return;

        if (axes == RotationAxes.MouseX)
        {
            transform.Rotate(0, GetXRot(), 0);
        }
        else if (axes == RotationAxes.MouseY)
        {
            CalcVertRot();

            float horizontalRot = transform.localEulerAngles.y;

            transform.localEulerAngles = new Vector3(verticalRot, horizontalRot, 0);
        }
        else
        {
            CalcVertRot();

            float delta = GetXRot();
            float horizontalRot = transform.localEulerAngles.y + delta;

            transform.localEulerAngles = new Vector3(verticalRot, horizontalRot, 0);
        }
    }

    void OnMouseLock(bool isMouseLocked)
    {
    #if DEBUG
        isLocked = !isMouseLocked;
    #endif
    }
}