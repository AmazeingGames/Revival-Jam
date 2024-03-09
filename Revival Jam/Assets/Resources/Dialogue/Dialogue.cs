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

    [field: Header("Information")]
    [field: SerializeField] public ItemAndAbilityManager.Abilities NewAbility = ItemAndAbilityManager.Abilities.None;
    [field: SerializeField] public ItemAndAbilityManager.Tools NewTool = ItemAndAbilityManager.Tools.None;

}
