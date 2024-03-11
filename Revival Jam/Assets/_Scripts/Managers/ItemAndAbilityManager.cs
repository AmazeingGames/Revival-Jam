using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

// Keeps track of the player's real world items and tools
public class ItemAndAbilityManager : Singleton<ItemAndAbilityManager>
{
    public static event Action<Abilities> GainAbility;
    public static event Action<Tools> SpawnTool;
    public static event Action<Tools> GainTool;

    public enum Abilities { None, Shake, Power}
    public enum Tools { None, Crowbar, Hammer, Wrench }

    readonly List<Abilities> learnedAbilities = new();

    public List<Tools> spawnedTools = new();
    public List<Tools> gainedTools = new();

    private void OnEnable()
    {
        DialogueManager.DialogueEvent += HandleDialogueEvent;
        Item3D.GainItem3D += HandleGainItem3D;
    }

    private void OnDisable()
    {
        DialogueManager.DialogueEvent -= HandleDialogueEvent;
        Item3D.GainItem3D += HandleGainItem3D;
    }

    void DeveloperCommands()
    {
#if DEBUG
        if (Input.GetKey(KeyCode.S))
        {
            if (Input.GetKeyDown(KeyCode.B))
                StartToolSpawn(Tools.Wrench);
            else if (Input.GetKeyDown(KeyCode.N))
                StartToolSpawn(Tools.Crowbar);
            else if (Input.GetKeyDown(KeyCode.M))
                StartToolSpawn(Tools.Hammer);
        }
        else if (Input.GetKey(KeyCode.G))
        {
            if (Input.GetKeyDown(KeyCode.B))
                StartGainAbility(Abilities.Power);
            else if (Input.GetKeyDown(KeyCode.N))
                StartGainAbility(Abilities.Shake);
        }

#endif
    }
    private void Update()
        => DeveloperCommands();

    // On Dialogue End:
    // Gain ability and inform listeners
    // Gain item and inform listeners
    public void HandleDialogueEvent(object sender, DialogueEventArgs eventArgs)
    {
        var ability = eventArgs.GetAbility();
        var tool = eventArgs.GetTool();

        StartGainAbility(ability);
        StartToolSpawn(tool);
    }

    void StartGainAbility(Abilities ability)
    {
        if (ability != Abilities.None && !learnedAbilities.Contains(ability))
        {
            learnedAbilities.Add(ability);
            GainAbility?.Invoke(ability);
        }
    }

    void StartToolSpawn(Tools tool)
    {
        if (tool != Tools.None && !spawnedTools.Contains(tool))
        {
            spawnedTools.Add(tool);
            SpawnTool?.Invoke(tool);
        }
    }

    public void HandleGainItem3D(ItemData3D itemData3D)
    {
        if (gainedTools.Contains(itemData3D.ItemType))
        {
            Debug.LogWarning("Already collected item");
            return;
        }
        gainedTools.Add(itemData3D.ItemType);
        GainTool.Invoke(itemData3D.ItemType);
    }

    // Checks if there are any spawned tools that have yet to be collected
    public bool AreUncollectedTools()
        => spawnedTools.Count > gainedTools.Count;
}
