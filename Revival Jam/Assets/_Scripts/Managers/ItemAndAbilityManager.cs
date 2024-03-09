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
    public static event Action<ItemsAndAbilities> GainAbility;

    public enum ItemsAndAbilities { None, Shake, Power, Crowbar, Hammer, Screwdriver, Wrench }

    readonly List<ItemsAndAbilities> learnedAbilities = new();

    private void OnEnable()
        => DialogueManager.DialogueEvent += HandleDialogueEvent;

    private void OnDisable()
        => DialogueManager.DialogueEvent -= HandleDialogueEvent;

    // On Dialogue End:
        // Gain ability/item and inform listeners
    public void HandleDialogueEvent(object sender, DialogueEventArgs eventArgs)
    {
        ItemsAndAbilities ability = eventArgs.GetAbility();
        if (ability == ItemsAndAbilities.None)
            return;

        if (learnedAbilities.Contains(ability))
            return;

        learnedAbilities.Add(ability);
        GainAbility?.Invoke(ability);
    }
}
