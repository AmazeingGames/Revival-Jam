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

    public enum ItemsAndAbilities { None, Shake, Power, Crowbar, Hammer, Screwdriver, Wrench }

    readonly List<ItemsAndAbilities> learnedAbilities = new();

    public List<ItemsAndAbilities> GetLearnedAbilities() => learnedAbilities.AsReadOnly().ToList();

    public void GainAbilityInformation(ItemsAndAbilities ability)
    {
        /*
        switch (ability)
        {
            case ItemsAndAbilities.Shake:
                break;
            case ItemsAndAbilities.Power:
                break;
            case ItemsAndAbilities.Crowbar:
                break;
            case ItemsAndAbilities.Hammer:
                break;
            case ItemsAndAbilities.Screwdriver:
                break;
        }
        */

        learnedAbilities.Add(ability);
        AbilityGain?.Invoke(ability);
    }
}
