using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueBank : Singleton<DialogueBank>
{
    public enum DialogueType { Power, Shake, Hammer, Wires, Wrench, Final };
    public Dictionary<DialogueType, Dialogue> DialogueDataByType;

    [Header("Terminal Dialogue")]
    [SerializeField] Dialogue powerDialogue;
    [SerializeField] Dialogue shakeDialogue;
    [SerializeField] Dialogue hammerDialogue;
    [SerializeField] Dialogue wiresDialogue;
    [SerializeField] Dialogue wrenchDialogue;

    [Header("Meta Dialogue")]
    [SerializeField] Dialogue finalDialogue;

    private void Start()
    {
        DialogueDataByType = new Dictionary<DialogueType, Dialogue>()
        {
            { DialogueType.Power, powerDialogue },
            { DialogueType.Shake, shakeDialogue },
            { DialogueType.Hammer, hammerDialogue },
            { DialogueType.Wires, wiresDialogue },
            { DialogueType.Wrench, wrenchDialogue },
            { DialogueType.Final, finalDialogue},
        };
    }
}
