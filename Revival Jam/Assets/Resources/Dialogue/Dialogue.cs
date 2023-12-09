using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue")]
public class Dialogue : ScriptableObject
{
    [field: Header("Dialogue")]
    [field: SerializeField] public DialogueManager.DialogueType DialogueType { get; private set; } = DialogueManager.DialogueType.Note;
    [field: SerializeField] public List<Message> Messages { get; private set; }
    [field: SerializeField] public List<Actor> Actors { get; private set; }

    [field: Header("Ability")]
    [field: SerializeField] public ItemAndAbilityManager.ItemsAndAbilities NewInformation = ItemAndAbilityManager.ItemsAndAbilities.None;
}
