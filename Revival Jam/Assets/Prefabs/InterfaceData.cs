using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Interface Data")]
public class InterfaceData : ScriptableObject
{
    [field: SerializeField] public ItemAndAbilityManager.ItemsAndAbilities InterfaceTool { get; private set; }
    [field: SerializeField] public PlayerFocus.FocusedOn InterfaceType { get; private set; }
}
