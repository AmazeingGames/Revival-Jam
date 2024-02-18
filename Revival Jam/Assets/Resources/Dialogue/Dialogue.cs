using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue")]
public class Dialogue : ScriptableObject
{

    [field: Header("Dialogue")]
    [field: SerializeField] public DialogueManager.DialogueType DialogueType { get; private set; } = DialogueManager.DialogueType.Terminal;
    [field: SerializeField] public List<Message> Messages { get; private set; }
    [field: SerializeField] public List<Actor> Actors { get; private set; }

    [field: Header("Ability")]
    [field: SerializeField] public ItemAndAbilityManager.ItemsAndAbilities NewItemOrAbility = ItemAndAbilityManager.ItemsAndAbilities.None;

    void Init(string message, DialogueManager.DialogueType dialogueType = DialogueManager.DialogueType.Terminal)
    {
        DialogueType = dialogueType;
        Messages = new List<Message>
        {
            new()
        };

        Messages[0].message = message;
    }

    public static Dialogue CreateDialogue(string message, DialogueManager.DialogueType dialogueType = DialogueManager.DialogueType.Terminal)
    {
        var data = CreateInstance<Dialogue>();

        data.Init(message, dialogueType);
        return data;
    }
}
