using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue")]
    [SerializeField] List<Message> messages;
    [SerializeField] List<Actor> actors;

    [Header("Prompt")]
    [SerializeField] GameObject Icon;

    bool hasPlayedDialogue;

    //Create a second dialogue that plays when you read the note again
    //The second time would highlight the important dialogue the player needs to read

    public void StartDialogue()
    {
        DialogueManager.Instance.StartDialogue(messages, actors);
    }
}

[Serializable]
public class Message
{
    public int actorId;
    public string message;
    public bool continuePreviousMessage;
}

[Serializable]
public class Actor
{
    public string name;
    public Sprite sprite;
}