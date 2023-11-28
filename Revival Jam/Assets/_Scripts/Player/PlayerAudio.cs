using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerFocus;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] StudioListener listener;
    [SerializeField] bool not;

    private void OnEnable()
    {
        FocusStation.ConnectToStation += HandleConnectToStation;
    }

    private void OnDisable()
    {
        FocusStation.ConnectToStation -= HandleConnectToStation;
    }

    //Disables the Arcade Game Audio via the Arcade Player's listener
    public void HandleConnectToStation(FocusStation.ConnectEventArgs connectEventArgs)
    {
        switch (connectEventArgs.LinkedStation)
        {
            case FocusedOn.Arcade:
                listener.enabled = connectEventArgs.IsConnecting;

                if (not)
                    listener.enabled = !listener.enabled;
                break;
        }   
    }
}
