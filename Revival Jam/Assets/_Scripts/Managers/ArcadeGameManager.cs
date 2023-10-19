using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

//Right now the game is small enoguh where we don't need a separate manager for the arcade and real world
//Changing its functionality to be smaller scale and manage things related to the arcade
//instead of the flow of the actual game
public class ArcadeGameManager : StaticInstance<ArcadeGameManager>
{
    public static bool IsMachineOn { get; private set; } = false;

    private void Update()
    {
        if (Input.GetButtonDown("PowerMachine"))
        {
            SetMachineOn(!IsMachineOn);
        }
    }

    void SetMachineOn(bool isOn)
    {
        IsMachineOn = isOn;
    }
}
