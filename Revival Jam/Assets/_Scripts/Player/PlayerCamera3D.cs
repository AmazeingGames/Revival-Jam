using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FocusStation;

public class PlayerCamera3D : MonoBehaviour
{
    [SerializeField] Transform cameraProxy;

    Vector3 cameraStartingPosition;
    Quaternion cameraStartingRotation;

    Camera playerCamera;
    //float constantYPosition;

    void Start()
    {
        playerCamera = GetComponent<Camera>();

        //Note we actually don't need or care about these values on start; their only purpose on start is for debugging reasons
        cameraStartingPosition = playerCamera.transform.position;
        cameraStartingRotation = playerCamera.transform.rotation;

        cameraProxy.position = playerCamera.transform.position;

    }

    private void OnEnable()
    {
        ConnectToStation += HandleConnectToStation;
    }

    private void OnDisable()
    {
        ConnectToStation -= HandleConnectToStation;
    }

    //Responsible for moving the camera when focusing/unfocusing
    void HandleConnectToStation(ConnectEventArgs connectEventArgs)
    {
        Vector3 positionToSet;
        Quaternion rotationToSet;

        if (connectEventArgs.IsConnecting)
        {
            cameraStartingPosition = playerCamera.transform.position;
            cameraStartingRotation = playerCamera.transform.rotation;

            positionToSet = connectEventArgs.StationCamera.position;
            rotationToSet = connectEventArgs.StationCamera.rotation;
        }
        else
        {
            positionToSet = cameraProxy.position;
            rotationToSet = cameraStartingRotation;
        }

        StartCoroutine(SetPosition(positionToSet, rotationToSet, connectEventArgs.IsConnecting));
    }

    //Sets the Camera's Position to the given transform
    //This needs a delay to work properly, hence the coroutine
    IEnumerator SetPosition(Vector3 positionToSet, Quaternion rotationToSet, bool isConnecting)
    {
        yield return null;

        playerCamera.transform.SetPositionAndRotation(positionToSet, rotationToSet);

        if (isConnecting)
            yield break;

        playerCamera.transform.position = cameraProxy.position;
    }
}
