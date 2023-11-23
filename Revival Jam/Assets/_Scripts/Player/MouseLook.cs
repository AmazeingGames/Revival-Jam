using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FocusStation;

public class MouseLook : MonoBehaviour
{
    public enum RotationAxes { MouseXY, MouseX, MouseY }


    public RotationAxes axes = RotationAxes.MouseXY;

    public float sensitvityVer = 9f;
    public float sensitivityHor = 9f;

    public float minimumVert = -45f;
    public float maximumVert = 45f;

    float verticalRot = 0;

    [SerializeField] float timeBetweenLockChecks = .25f;

    bool isLocked = false;
    bool escape = false;

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
        if(TryGetComponent<Rigidbody>(out var body))
        {
            body.freezeRotation = true;
        }

        StartCoroutine(IsLockedCheck());
    }

    private void OnEnable()
    {
        ConnectToStation += HandleConnectToStation;
    }

    private void OnDisable()
    {
        ConnectToStation -= HandleConnectToStation;
    }

    //Makes sure isLocked stays properly synced with the game
    //Note for Performance: Running for every instance of MouseLook takes up unnecessary performance.
    IEnumerator IsLockedCheck()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenLockChecks);

            if (PlayerFocus.Instance == null)
                continue;

            var shouldLock = PlayerFocus.Instance.Focused != PlayerFocus.FocusedOn.Nothing;

            if (isLocked != shouldLock)
            {
                isLocked = shouldLock;
                Debug.LogWarning("IsLocked got Desynced. Fixing IsLocked");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            escape = !escape;

        Look();
    }

    //Rotates the attached transform with the movement of the mouse and only on specified axes
    //Only when not focused and not paused
    void Look()
    {
        if (isLocked || escape)
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

    //Locks camera movement while focused on an interface
    void HandleConnectToStation(ConnectEventArgs connectEventArgs)
    {
        isLocked = connectEventArgs.IsConnecting;
    }
}
