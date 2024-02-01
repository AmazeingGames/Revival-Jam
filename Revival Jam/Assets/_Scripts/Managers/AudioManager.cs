using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [field: Header("Arcade Game Ambience")]
    [SerializeField] EventReference castleAmbience;
    [SerializeField] EventReference castleGlitchAmbience;
    [SerializeField] EventReference forestAmbience;
    [SerializeField] EventReference forestGlitchAmbience;

    [field: Header("Arcade Game SFX")]
    [SerializeField] EventReference playerJoust;
    [SerializeField] EventReference playerDamage;
    [SerializeField] EventReference arcadeFootsteps;
    [SerializeField] EventReference fireWall;
    [SerializeField] EventReference enemyDamage;

    [field: Header("Arcade Cabinet SFX")]
    [SerializeField] EventReference circuitCablePlug;
    [SerializeField] EventReference circuitCableUnplug;
    [SerializeField] EventReference arcadeOn;
    [SerializeField] EventReference arcadeOff;
    [SerializeField] EventReference circuitPanelOpen;
    [SerializeField] EventReference arcadeUIHover;
    [SerializeField] EventReference arcadeUISelect;
    [SerializeField] EventReference shakeArcade;

    [field: Header("UI")]
    [SerializeField] EventReference gameUIHover;
    [SerializeField] EventReference gameUISelect;

    [field: Header("3D Game")]
    [SerializeField] EventReference player3DFootsteps;
    [SerializeField] EventReference endingSounds;

    [field: Header("2D Game")]
    [SerializeField] EventReference consoleBlip;

    Bus masterBus;

    public enum EventSounds { Null, CastleAmbience, CastleGlitchAmbience, ForestAmbience, ForestGlitchAmbience, PlayerJoust, PlayerDamage, ArcadeFootsteps, CircuitCablePlug, CircuitCableUnplug, ArcadeOn, ArcadeOff, CircuitPanelOpen, Player3DFootsteps, ArcadeUIHover, ArcadeUISelect, UIHover, UISelect, FireWall, ConsoleDialogue, Ending, ArcadeShake, EnemyTakeDamage }

    public enum FootstepsParameter { Grass, Stone }


    Dictionary<EventSounds, EventReference> SoundTypeToReference;

    readonly Dictionary<EventSounds, EventInstance> SoundTypeToInstance = new();
    readonly List<EventInstance> EventInstances = new();
    readonly List<StudioEventEmitter> EventEmitters = new();

    EventInstance footstepsEvent;

    void Start()
    {
        SoundTypeToReference = new()
        {
            { EventSounds.CastleAmbience, castleAmbience },
            { EventSounds.CastleGlitchAmbience, castleGlitchAmbience},
            { EventSounds.ForestAmbience, forestAmbience},
            { EventSounds.ForestGlitchAmbience, forestGlitchAmbience },
            { EventSounds.PlayerJoust, playerJoust},
            { EventSounds.PlayerDamage, playerDamage},
            { EventSounds.FireWall, fireWall },
            { EventSounds.ArcadeFootsteps, arcadeFootsteps},
            { EventSounds.CircuitCablePlug, circuitCablePlug},
            { EventSounds.CircuitCableUnplug, circuitCableUnplug},
            { EventSounds.ArcadeOn, arcadeOn },
            { EventSounds.ArcadeOff, arcadeOff},
            { EventSounds.CircuitPanelOpen, circuitPanelOpen },
            { EventSounds.Player3DFootsteps, player3DFootsteps },
            { EventSounds.ArcadeUIHover, arcadeUIHover },
            { EventSounds.ArcadeUISelect, arcadeUISelect },
            { EventSounds.UIHover, gameUIHover },
            { EventSounds.UISelect, gameUISelect },
            { EventSounds.ConsoleDialogue, consoleBlip },
            { EventSounds.Ending, endingSounds },
            { EventSounds.ArcadeShake, shakeArcade },
            { EventSounds.EnemyTakeDamage, enemyDamage },
        };

        masterBus = RuntimeManager.GetBus("bus:/");
    }

    private void Update()
    {
        masterBus.setVolume(SettingsManager.Instance.GameVolume);
    }

    public static void SetFootsteps(FootstepsParameter groundType)
    {
        RuntimeManager.StudioSystem.setParameterByName("TerrainType", (float)groundType);
    }

    void OnDestroy()
    {
        CleanUp();
    }

    public static void TriggerAudioClip(EventSounds sound, Transform origin) => TriggerAudioClip(sound, origin.position);

    public static void TriggerAudioClip(EventSounds sound, Vector3 origin)
    {
        // Debug.Log($"Triggered Audio Clip: {sound}");

        if (sound == EventSounds.Null)
            return;

        if (Instance == null)
            return;

        Instance.TriggerAudioClip(Instance.SoundTypeToReference[sound], origin);
    }

    void TriggerAudioClip(EventReference sound, Vector3 origin) => RuntimeManager.PlayOneShot(sound, origin);

    EventInstance CreateEventInstance(EventSounds key)
    {
        SoundTypeToReference.TryGetValue(key, out EventReference reference);

        EventInstance value = RuntimeManager.CreateInstance(reference);

        EventInstances.Add(value);
        SoundTypeToInstance.Add(key, value);

        return value;
    }

    public StudioEventEmitter InitializeEventEmitter(EventSounds key, GameObject emitterObject)
    {
        if (!SoundTypeToReference.TryGetValue(key, out var reference))
        {
            Debug.LogWarning($"No event reference found with the key \"{key}\"");
            return null;
        }

        if (!emitterObject.TryGetComponent<StudioEventEmitter>(out var emitter))
        {
            Debug.LogWarning("No event emitter component attached to object");
            return null;
        }

        Debug.Log($"Initialized emitter {key}");

        emitter.EventReference = reference;
        EventEmitters.Add(emitter);
        return emitter;
    }

    //Starts and Stops a given event instance
    public void StartEventInstance(EventSounds key)
    {
        //If no event instance exists, we create a new instance and recur
        if (!SoundTypeToInstance.TryGetValue(key, out EventInstance instance))
        {
            var newInstance = CreateEventInstance(key);
            
            Debug.Log($"Created new EventInstance (key) {key} | (instnace) {newInstance}");

            StartEventInstance(key);
            return;
        }
        
        instance.start();

        Debug.Log($"Started {key}");
    }

    public void StopEventInstance(EventSounds key, FMOD.Studio.STOP_MODE stopMode = FMOD.Studio.STOP_MODE.ALLOWFADEOUT)
    {
        if (!SoundTypeToInstance.TryGetValue(key, out EventInstance instance))
            return;

        instance.stop(stopMode);

        Debug.Log($"Stopped {key}");
    }

    void CleanUp()
    {
        foreach (var eventInstance in EventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }

        foreach (var eventEmitter in EventEmitters)
            eventEmitter.Stop();
    }
}