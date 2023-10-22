using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static PlayerFocus;
using static ArcadeGameManager;
using static AudioManager;
using static AudioManager.EventSounds;

//Right now the game is small enoguh where we don't need a separate manager for the arcade and real world
//Changing its functionality to be smaller scale and manage things related to the arcade
//instead of the flow of the actual game
public class MachineAbilities : Singleton<MachineAbilities>
{
    [SerializeField] float glitchWorldTimerLength;

    [SerializeField] bool pauseTimerOnMachineOff;

    GameObject glitchedWorld;

    float glitchTimer;

    public bool IsMachineOn { get; private set; } = false;

    private void OnEnable()
    {
        AfterArcadeStateChange += HandleArcadeGameStateChange;
    }

    private void OnDisable()
    {
        AfterArcadeStateChange -= HandleArcadeGameStateChange;
    }

    private void Update()
    {
        if (Input.GetButtonDown("PowerMachine") && PlayerFocus.Instance.Focused == FocusedOn.Arcade)
        {
            SetMachineOn(!IsMachineOn);
            
            EventSounds soundToPlay = IsMachineOn ? ArcadeOn : ArcadeOff;

            AudioManager.Instance.TriggerAudioClip(soundToPlay, transform);
        }
        
        //This could just as easily refer to the singleton player class
        if (Input.GetButtonDown("ShakeArcade") && PlayerFocus.Instance.Focused == FocusedOn.Arcade && Player.Instance != null)
        {
            Debug.Log("Shake Arcade");
            ShakeArcade();
        }
    }

    void HandleArcadeGameStateChange(ArcadeState arcadeState)
    {
        switch (arcadeState)
        {
            case ArcadeState.StartLevel:
                StartCoroutine(FindTileMaps());
                break;
        }
    }

    void ShakeArcade()
    {
        StartCoroutine(EnterGlitchedWorld());
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

        glitchedWorld.SetActive(false);
    }

    IEnumerator EnterGlitchedWorld()
    {
        Debug.Log("Enter glitched world");

        glitchedWorld.SetActive(true);
        glitchTimer = 10;

        while (glitchTimer > 0)
        {
            Debug.Log("Paused Timer");

            if (pauseTimerOnMachineOff || IsMachineOn)
            {
                Debug.Log("Counting down timer");
                glitchTimer -= Time.deltaTime;
            }

            yield return null;
        }
        Debug.Log("Exit glitched world");
        glitchedWorld.SetActive(false);
        yield break;

    }

    void SetMachineOn(bool isOn)
    {
        IsMachineOn = isOn;
    }
}
