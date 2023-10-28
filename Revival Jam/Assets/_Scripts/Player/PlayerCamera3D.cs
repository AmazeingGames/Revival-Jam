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
    IEnumerator SetPosition(Vector3 positionToSet, Quaternion rotationToSet, bool isConnecting)
    {
        //For some reason if we set the position of the camera directly it breaks
        //Don't ask why coroutines magically fix everything
        for (int i = 0; i < 1;  i++)
        {
            yield return null;

            playerCamera.transform.SetPositionAndRotation(positionToSet, rotationToSet);

            Debug.Log($"Position is position to set : (x : {positionToSet.x == playerCamera.transform.position.x}, y : {positionToSet.y == playerCamera.transform.position.y}, z : {positionToSet.z == playerCamera.transform.position.z})");
        }

        if (isConnecting)
            yield break;

        //Makes sure the height of the camera stays the same when returning to the player's body
        //playerCamera.transform.position = new Vector3(playerCamera.transform.position.x, constantYPosition, playerCamera.transform.position.z);

        playerCamera.transform.position = cameraProxy.position;

        //Debug.Log($"Is y camera position greater than max y position : {playerCamera.transform.position.y > constantYPosition}");

    }
}
