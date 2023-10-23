using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] List<Message> messages;
    [SerializeField] List<Actor> actors;

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
}

[Serializable]
public class Actor
{
    public string name;
    public Sprite sprite;
}