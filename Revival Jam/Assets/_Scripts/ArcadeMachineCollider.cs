using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcadeMachineCollider : MonoBehaviour
{
    BoxCollider boxCollider;

    private void OnEnable()
    {
        FocusStation.ConnectToStation += HandleConnectToStation;
    }

    private void OnDisable()
    {
        FocusStation.ConnectToStation -= HandleConnectToStation;
    }

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    //Turns off collider, since it blocks raycasting
    void HandleConnectToStation(FocusStation.ConnectEventArgs connectEventArgs)
    {
        if (connectEventArgs == null)
            return;

        boxCollider.enabled = !connectEventArgs.IsConnecting;
    }
}
