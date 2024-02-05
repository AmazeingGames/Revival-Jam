using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Timeline.AnimationPlayableAsset;

public class AudioManager : Singleton<AudioManager>
{
    [field: Header("Game PriorityEventInstance")]
    [SerializeField] EventReference castleAmbience;
    [SerializeField] EventReference forestAmbience;
    [SerializeField] EventReference atticAmbience;

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

    public enum OneShotSounds { Null, PlayerJoust, PlayerDamage, ArcadeFootsteps, CircuitCablePlug, CircuitCableUnplug, ArcadeOn, ArcadeOff, CircuitPanelOpen, Player3DFootsteps, ArcadeUIHover, ArcadeUISelect, UIHover, UISelect, ConsoleDialogue, Ending, ArcadeShake, EnemyTakeDamage }

    public enum LoopingSounds { CastleAmbience, ForestAmbience, AtticAmbience, FireWall }

    public enum FootstepsParameter { Grass, Stone }

    public enum InstanceStartMode { Start, StopAllowFadeout, StopImmediate,}

    //Refers to the audio *file* correlated to the enum
    Dictionary<OneShotSounds, EventReference> OneShotToReference;

    //Refers to both the audio *file* location and the audio's priority value
    Dictionary<LoopingSounds, LoopingAudioReference> LoopingToReference;

    //Refers to the gameObject containing the 'EventInstance' script in scene
    readonly Dictionary<LoopingSounds, PriorityEventInstance> SoundTypeToEventInstance = new();


    readonly List<StudioEventEmitter> EventEmitters = new();

    void Start()
    {
        masterBus = RuntimeManager.GetBus("bus:/");

        OneShotToReference = new()
        {
            { OneShotSounds.PlayerJoust, playerJoust},
            { OneShotSounds.PlayerDamage, playerDamage},
            { OneShotSounds.ArcadeFootsteps, arcadeFootsteps},
            { OneShotSounds.CircuitCablePlug, circuitCablePlug},
            { OneShotSounds.CircuitCableUnplug, circuitCableUnplug},
            { OneShotSounds.ArcadeOn, arcadeOn },
            { OneShotSounds.ArcadeOff, arcadeOff},
            { OneShotSounds.CircuitPanelOpen, circuitPanelOpen },
            { OneShotSounds.Player3DFootsteps, player3DFootsteps },
            { OneShotSounds.ArcadeUIHover, arcadeUIHover },
            { OneShotSounds.ArcadeUISelect, arcadeUISelect },
            { OneShotSounds.UIHover, gameUIHover },
            { OneShotSounds.UISelect, gameUISelect },
            { OneShotSounds.ConsoleDialogue, consoleBlip },
            { OneShotSounds.Ending, endingSounds },
            { OneShotSounds.ArcadeShake, shakeArcade },
            { OneShotSounds.EnemyTakeDamage, enemyDamage },
        };

        LoopingToReference = new()
        {
            { LoopingSounds.FireWall, new (LoopingSounds.FireWall, fireWall) },
            { LoopingSounds.CastleAmbience, new (LoopingSounds.CastleAmbience, castleAmbience, 1) },
            { LoopingSounds.AtticAmbience,  new (LoopingSounds.AtticAmbience, atticAmbience, 1) },
            { LoopingSounds.ForestAmbience, new (LoopingSounds.ForestAmbience, forestAmbience, 1) },
        }; 
    }

    private void Update()
    {
        if (SettingsManager.Instance != null)
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

    //Make this have a default value of the caller's transform
    public static void TriggerAudioClip(OneShotSounds sound, Transform origin) => TriggerAudioClip(sound, origin.position);

    public static void TriggerAudioClip(OneShotSounds sound, Vector3 origin)
    {
        Debug.Log($"Triggered Audio Clip: {sound}");

        if (sound == OneShotSounds.Null)
            return;

        if (Instance == null)
            return;

        Instance.TriggerAudioClip(Instance.OneShotToReference[sound], origin);
    }

    void TriggerAudioClip(EventReference sound, Vector3 origin) => RuntimeManager.PlayOneShot(sound, origin);

    //Pass in the sound you want to play and the object to emit the sound
    //Supplies the audio event referencce and also keeps a list of emitter objects to be later disabled 
    public StudioEventEmitter InitializeEventEmitter(LoopingSounds key, GameObject emitterObject)
    {
        if (!LoopingToReference.TryGetValue(key, out var reference))
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

        emitter.EventReference = reference.eventReference;
        EventEmitters.Add(emitter);
        return emitter;
    }

    //Given a looping sound, creates an instance of that sound that can be played on repeat
    PriorityEventInstance CreateEventInstance(LoopingSounds key)
    {
        LoopingToReference.TryGetValue(key, out LoopingAudioReference reference);

        PriorityEventInstance priorityEventInstance = new(reference);

        SoundTypeToEventInstance.Add(key, priorityEventInstance);

        return priorityEventInstance;
    }

    //Responsible for making sure the event exists and calling the real 'startEvent' method
    public static void StartEventInstance(LoopingSounds key, InstanceStartMode startMode)
    {
        //This should instead instantiate the manager
        if (Instance == null)
        {
            Debug.LogWarning("AudioManager is null!");
            return;
        }

        //If no event instance exists, we create a new instance and recur
        if (!Instance.SoundTypeToEventInstance.TryGetValue(key, out PriorityEventInstance priorityEventInstance))
        {
            PriorityEventInstance newEventInstance = Instance.CreateEventInstance(key);

            Debug.Log($"Created new EventInstance (key) {key} | (instance) {newEventInstance.EventKey}");

            StartEventInstance(key, startMode);
            return;
        }

        StartEventInstance(priorityEventInstance, startMode);
    }

    //Responsbile for starting and stopping the given event instance
    //On starting an eventInstance, stops all other running event instances that match the given instance's priority
    static void StartEventInstance(PriorityEventInstance priorityEventInstance, InstanceStartMode startMode)
    {
        var instance = priorityEventInstance.instance;
        switch (startMode)
        {
            case InstanceStartMode.Start:
                instance.start();

                StopMatchingPriority(priorityEventInstance);
                break;

            case InstanceStartMode.StopAllowFadeout:
                instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                break;

            case InstanceStartMode.StopImmediate:
                instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                break;
        }
    }

    //Job is to loop through all created events that match a target's priority and stop all found tracks
    static void StopMatchingPriority(PriorityEventInstance priorityEventInstance)
    {
        int priority = priorityEventInstance.loopingAudioReference.priority;
        if (priority == -1)
            return;

        //Alternatively, I could store different priorities in different lists and simply loop through the lists
        //This would save on performance, but cost more memory
        List<PriorityEventInstance> instances = Instance.SoundTypeToEventInstance.Values.ToList();

        //Could use stringbuilder to improve performance
        string stoppedEventsString = string.Empty;
        string priorityString = $"of priority {priority}";

        foreach (var currentEventInstance in instances)
        {
            if (priority == currentEventInstance.loopingAudioReference.priority && priorityEventInstance.EventKey != currentEventInstance.EventKey)
            {
                StartEventInstance(currentEventInstance, InstanceStartMode.StopAllowFadeout);
                stoppedEventsString += $"{currentEventInstance.EventKey}, ";
            }
        }

        if (stoppedEventsString == string.Empty)
            Debug.Log($"Started {priorityEventInstance.EventKey} | Stopped no events {priorityString}");
        else
            Debug.Log($"Started {priorityEventInstance.EventKey} | Stopped all matching events {priorityString} : {stoppedEventsString[..^2]}");
    }

    void CleanUp()
    {
        Debug.Log("CLEANUP");

        List<PriorityEventInstance> priorityEvents = SoundTypeToEventInstance.Values.ToList();
        foreach (PriorityEventInstance priorityEvent in priorityEvents)
        {
            priorityEvent.instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            priorityEvent.instance.release();
        }

        foreach (var eventEmitter in EventEmitters)
            eventEmitter.Stop();
    }

    //Stores a reference to a stored audio reference and a priority value
    //Purpose is to facilitate playing looping tracks while stopping tracks that have a conflicting priority value

    //Priority is a confusing name -- think of it more like a 'sound type'. No sound is more important than any other, except the one we want to currently play. All it does is stop sounds of a matching 'type'.
    readonly struct LoopingAudioReference
    {
        public readonly LoopingSounds eventKey;
        public readonly EventReference eventReference;
        public readonly int priority;

        //Sets 'no conflicts' as the default priority value
        public LoopingAudioReference(LoopingSounds eventKey, EventReference eventReference, int priority = -1)
        {
            this.eventKey = eventKey;
            this.eventReference = eventReference;
            this.priority = priority;
        }
    }

    //Contains the sound, sound's priority, and sound reference
    //Used to start and stop sound instances
    readonly struct PriorityEventInstance
    {
        public readonly LoopingAudioReference loopingAudioReference;
        public readonly EventInstance instance;

        public EventReference EventReference => loopingAudioReference.eventReference;
        public LoopingSounds EventKey => loopingAudioReference.eventKey;
        public PriorityEventInstance(LoopingAudioReference audioReference)
        {
            this.loopingAudioReference = audioReference;
            this.instance = RuntimeManager.CreateInstance(audioReference.eventReference);
        }
    }
}