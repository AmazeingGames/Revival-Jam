using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusStation : MonoBehaviour
{
    [SerializeField] PlayerFocus.FocusedOn linkedStation;
    [SerializeField] Transform stationCamera;
    bool containsPlayer;

    Vector3 cameraStartingPosition;
    Quaternion cameraStartingRotation;

    Camera playerCamera;
    VirtualScreen linkedScreen;

    private void OnEnable()
    {
        PlayerFocus.ConnectToStation += ContainsPlayer;
        VirtualScreen.FindStation += FindStationHandle;
    }

    private void OnDisable()
    {
        PlayerFocus.ConnectToStation -= ContainsPlayer;
        VirtualScreen.FindStation -= FindStationHandle;

    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (playerCamera == null)
        {
            playerCamera = FPSInput.Instance.PlayerCamera;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == null)
        {
            return;
        }

        Debug.Log($"Is player.Instance null {Player.Instance == null}");

        if (other.gameObject.CompareTag("Player"))
        {
            containsPlayer = true;
            Debug.Log("Contains Player!");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == null)
            return;

        if (collision.gameObject.CompareTag("Player"))
            containsPlayer = false;
    }

    public void ContainsPlayer(bool isConnecting)
    {
        Debug.Log($"Contains Player: {containsPlayer}");

        if (!containsPlayer)
        {
            Debug.Log($"Disabled virtual screen {linkedScreen.name}");
            linkedScreen.enabled = false;
            return;
        }
        linkedScreen.enabled = true;


        if (isConnecting)
        {
            cameraStartingPosition = playerCamera.transform.position;
            cameraStartingRotation = playerCamera.transform.rotation;

            playerCamera.transform.SetPositionAndRotation(stationCamera.transform.position, stationCamera.transform.rotation);

            PlayerFocus.Instance.ConnectedToStation(linkedStation);
        }
        else
        {
            playerCamera.transform.SetPositionAndRotation(cameraStartingPosition, cameraStartingRotation);

            PlayerFocus.Instance.ConnectedToStation(PlayerFocus.FocusedOn.Nothing);

            linkedScreen.enabled = true;
        }
    }

    void FindStationHandle(VirtualScreen sender, PlayerFocus.FocusedOn virtualScreenType)
    {
        if (virtualScreenType == linkedStation)
        {
            linkedScreen = sender;
            Debug.Log($"Found screen! Linked Screen null : {linkedScreen == null}");
        }
    }
}
