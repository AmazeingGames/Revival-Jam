using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

//Keeps track of the player's real world items and tools
public class ItemAndAbilityManager : Singleton<ItemAndAbilityManager>
{
    public static event Action<ItemsAndAbilities> AbilityGain;

    public enum ItemsAndAbilities { None, Shake, Power, Crowbar, Hammer, Screwdriver, Wrench, EnterTerminal }

    readonly List<ItemsAndAbilities> learnedAbilities = new();

    private void OnEnable()
    {
        DialogueManager.RaiseDialogue += HandleDialogue;
    }

    private void OnDisable()
    {
        DialogueManager.RaiseDialogue -= HandleDialogue;
    }

    void HandleDialogue(object sender, DialogueManager.DialogueEventArgs dialogueEventArgs)
    {
        if (!dialogueEventArgs.shouldGrantAbility)
            return;

        if (dialogueEventArgs.dialogue.NewItemOrAbility == ItemsAndAbilities.None)
            return;

        GainAbilityInformation(dialogueEventArgs.dialogue.NewItemOrAbility);
    }

    public void GainAbilityInformation(ItemsAndAbilities ability)
    {
        if (learnedAbilities.Contains(ability))
            return;

        learnedAbilities.Add(ability);

        AbilityGain?.Invoke(ability);

        Debug.Log($"Gained Information : {ability}");
    }
}
