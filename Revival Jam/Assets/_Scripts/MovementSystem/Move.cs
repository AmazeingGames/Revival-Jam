using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static PlayerFocus;
using System;

public class Move : UIButtonBase
{
    [SerializeField] FocusedOn connectedStation;

    public static event Action<FocusedOn> ConnectToStation;

    public override void OnClick()
    {
        base.OnClick();

        ConnectToStation?.Invoke(connectedStation);
    }
}
