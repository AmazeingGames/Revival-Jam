using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerFocus : Singleton<PlayerFocus>
{
    public static FocusedOn Focused { get; private set; } = FocusedOn.Nothing;

    public enum FocusedOn { Circuitry, Arcade, Nothing }

    public static event Action<bool> ConnectToStation;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Focus"))
        {
            if (Focused == FocusedOn.Nothing)
                OnCheckStation(true);
            else
                OnCheckStation(false);
        } 
    }

    void OnCheckStation(bool isConnecting)
    {
        ConnectToStation?.Invoke(isConnecting);
    }

    public void ConnectedToStation(FocusedOn focusedOn)
    {
        Focused = focusedOn;
    }
}
