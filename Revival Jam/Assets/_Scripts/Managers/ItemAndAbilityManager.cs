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
    public static event Action<Tools> GainTool;

    public enum Abilities { None, Shake, Power}
    public enum Tools { None, Crowbar, Hammer, Wrench }


    readonly List<Abilities> learnedAbilities = new();
    readonly List<Tools> possesedTools = new();

    private void OnEnable()
        => DialogueManager.DialogueEvent += HandleDialogueEvent;

    private void OnDisable()
        => DialogueManager.DialogueEvent -= HandleDialogueEvent;

    // On Dialogue End:
        // Gain ability and inform listeners
        // Gain item and inform listeners
    public void HandleDialogueEvent(object sender, DialogueEventArgs eventArgs)
    {
        var ability = eventArgs.GetAbility();
        var tool = eventArgs.GetTool();

        if (ability != Abilities.None && !learnedAbilities.Contains(ability))
        {
            learnedAbilities.Add(ability);
            GainAbility?.Invoke(ability);
        }

        if (tool != Tools.None && !possesedTools.Contains(tool))
        {
            possesedTools.Add(tool);
            GainTool?.Invoke(tool);
        }     
    }
}
