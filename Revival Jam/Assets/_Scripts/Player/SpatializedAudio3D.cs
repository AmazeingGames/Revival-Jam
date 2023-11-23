using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerFocus;
using static FMODUnity.RuntimeUtils;


public class SpatializedAudio3D : MonoBehaviour
{
    [SerializeField] Camera playerCamera;
    [SerializeField] bool usePosition;
    [SerializeField] bool usePlayerPosition;

    FMOD.ATTRIBUTES_3D attributes = new();

    bool shouldListen;


    private void Start()
    {
        EnforceLibraryOrder();
    }

    // Update is called once per frame
    void Update()
    { 
        if (Player.Instance == null)
            return;

        if (!shouldListen)
            return;

        attributes.position = FMODUnity.RuntimeUtils.ToFMODVector(Player.Instance.transform.position);
        attributes.forward = FMODUnity.RuntimeUtils.ToFMODVector(playerCamera.transform.forward);
        attributes.up = FMODUnity.RuntimeUtils.ToFMODVector(playerCamera.transform.up);

        var position = To3DAttributes(ArcadeSoundEmitter.Transform).position;
        var playerPosition = To3DAttributes(transform).position;
        
        if (usePosition)
            FMODUnity.RuntimeManager.StudioSystem.setListenerAttributes(0, attributes, position);
        else if (usePlayerPosition)
            FMODUnity.RuntimeManager.StudioSystem.setListenerAttributes(0, attributes, playerPosition);
        else
            FMODUnity.RuntimeManager.StudioSystem.setListenerAttributes(0, attributes);

        Debug.Log("playing");
    }

    private void OnEnable()
    {
        FocusStation.ConnectToStation += HandleConnectToStation;
    }

    private void OnDisable()
    {
        FocusStation.ConnectToStation -= HandleConnectToStation;
    }

    public void HandleConnectToStation(FocusStation.ConnectEventArgs connectEventArgs)
    {
        switch (connectEventArgs.LinkedStation)
        {
            case FocusedOn.Arcade:
                shouldListen = !connectEventArgs.IsConnecting;
                break;
        }
    }
}
