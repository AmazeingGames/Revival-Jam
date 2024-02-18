using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static PlayerFocus;
using static ArcadeGameManager;
using static AudioManager;
using static AudioManager.OneShotSounds;

public class MachineAbilities : Singleton<MachineAbilities>
{
    [SerializeField] float glitchWorldTimerLength;

    [SerializeField] bool pauseTimerOnMachineOff;

    GameObject glitchedWorld;

    float glitchTimer;

    bool canShakeMachine = false;
    bool canPowerMachine = false;
    bool canEnterTerminal = false;

    Dictionary<ItemAndAbilityManager.ItemsAndAbilities, bool> canUseAbilityByType;

    public bool IsMachineOn { get; private set; } = false;

    private void OnEnable()
    {
        AfterArcadeStateChange += HandleArcadeGameStateChange;
        ItemAndAbilityManager.AbilityGain += HandleAbilityGain;
    }

    private void OnDisable()
    {
        AfterArcadeStateChange -= HandleArcadeGameStateChange;
        ItemAndAbilityManager.AbilityGain -= HandleAbilityGain;
    }

    private void Start()
    {
        canUseAbilityByType = new()
        {
            { ItemAndAbilityManager.ItemsAndAbilities.EnterTerminal, canEnterTerminal },
            { ItemAndAbilityManager.ItemsAndAbilities.Power, canPowerMachine },
            { ItemAndAbilityManager.ItemsAndAbilities.Shake, canShakeMachine },
        };
    }

    private void Update()
    {
        if (PlayerFocus.Instance.Focused != FocusedOn.Arcade)
            return;

        if (Input.GetButtonDown("PowerMachine") && (canPowerMachine || !IsMachineOn))
        {
            SetMachineOn(!IsMachineOn);
            
            OneShotSounds soundToPlay = IsMachineOn ? ArcadeOn : ArcadeOff;

            TriggerAudioClip(soundToPlay, transform);
        }
        
        //Perhaps check if we have already shaked the machine in the last 10 seconds
        if (Input.GetButtonDown("ShakeArcade") && canShakeMachine && Player.Instance != null)
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

    void HandleAbilityGain(ItemAndAbilityManager.ItemsAndAbilities newAbility)
    {
        if (canUseAbilityByType.TryGetValue(newAbility, out bool _))
        {
            canUseAbilityByType[newAbility] = true;
            Debug.Log($"Learned Ability: {newAbility}");
        }
    }

    void ShakeArcade()
    {
        Debug.Log("Shake Arcade");
        StartCoroutine(EnterGlitchedWorld());

        TriggerAudioClip(OneShotSounds.ArcadeShake, ArcadeSoundEmitter.Transform);
    }

    //Yes, 'GameObject.Find' in coroutine is bad for performance, but it's hard to think of a better way with tile maps
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

            if (pauseTimerOnMachineOff || IsMachineOn)
            {
                Debug.Log("Counting down timer");
                glitchTimer -= Time.deltaTime;
            }
            else
                Debug.Log("Paused Timer");

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
