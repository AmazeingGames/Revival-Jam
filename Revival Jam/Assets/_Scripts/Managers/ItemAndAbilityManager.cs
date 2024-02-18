using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    //The player needs to receive an ability or an item at the end of reading a note
    void HandleDialogue(object sender, DialogueManager.DialogueEventArgs dialogueEventArgs)
    {
        //Sometimes the note won't want us to gain the ability, even when it finishes
        //Example being when we restart the level, closing the note
        //Because we're now creating the notes to be entirely separate from the arcade, this is starting to become a moot point
        if (!dialogueEventArgs.shouldGrantAbility)
            return;

        var ability = dialogueEventArgs.dialogue.NewItemOrAbility;

        if (ability == ItemsAndAbilities.None)
            return;

        if (learnedAbilities.Contains(ability))
            return;

        learnedAbilities.Add(ability);

        AbilityGain?.Invoke(ability);

        Debug.Log($"Gained Information : {ability}");
    }
}
