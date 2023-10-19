using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static GameManager;

//Right now the game is small enoguh where we don't need a separate manager for the arcade and real world
//Changing its functionality to be smaller scale and manage things related to the arcade
//instead of the flow of the actual game
public class ArcadeGameManager : StaticInstance<ArcadeGameManager>
{
    GameObject glitchedWorld;

    private void OnEnable()
    {
        OnAfterStateChanged += HandleGameStateChange;
    }

    private void OnDisable()
    {
        OnAfterStateChanged -= HandleGameStateChange;
    }

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

    void HandleGameStateChange(GameState state)
    {
        switch (state)
        {
            case GameState.StartLevel:
                StartCoroutine(FindTileMaps());
                break;
        }
    }

    //Yes, find in coroutine is bad for performance, but it's hard to think of a better way with tile maps
    IEnumerator FindTileMaps()
    {
        while (Player.Instance == null)
          yield return null;

        GameObject glitchedWorld = null;

        while (glitchedWorld == null)
        {
            yield return null;

            glitchedWorld = GameObject.Find("GlitchedWorld");
        }

        Debug.Log("Found glitched world!");

        this.glitchedWorld = glitchedWorld;
    }
}
