using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FocusStation;

public class PlayerCamera3D : MonoBehaviour
{
    Vector3 cameraStartingPosition;
    Quaternion cameraStartingRotation;

    Camera playerCamera;
    float constantYPosition;

    void Start()
    {
        playerCamera = GetComponent<Camera>();
        constantYPosition = playerCamera.transform.position.y;
    }

    private void OnEnable()
    {
        ConnectToStation += HandleConnectToStation;
    }

    private void OnDisable()
    {
        ConnectToStation -= HandleConnectToStation;
    }

    void HandleConnectToStation(ConnectEventArgs connectEventArgs)
    {
        if (connectEventArgs.IsConnecting)
        {
            cameraStartingPosition = playerCamera.transform.position;
            cameraStartingRotation = playerCamera.transform.rotation;

            playerCamera.transform.SetPositionAndRotation(connectEventArgs.StationCamera.position, connectEventArgs.StationCamera.rotation);
        }
        else
        {
            playerCamera.transform.SetPositionAndRotation(cameraStartingPosition, cameraStartingRotation);
            playerCamera.transform.position = new Vector3(transform.position.x, constantYPosition, transform.position.z);
        }
    }
}