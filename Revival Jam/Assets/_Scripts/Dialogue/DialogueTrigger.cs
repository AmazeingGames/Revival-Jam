using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    //Probably Change this to accept a scriptable object instead
    //Or it could read from a JSON file instead
    [Header("Dialogue")]
    [SerializeField] List<Message> messages;
    [SerializeField] List<Actor> actors;

    [Header("Prompt")]
    [SerializeField] GameObject Icon;

    //Create a second dialogue that plays when you read the note again
    //The second time would highlight the important dialogue the player needs to read
    bool hasPlayedDialogue;

    bool isPlayerInRange = false;

    public void StartDialogue()
    {
        DialogueManager.Instance.StartDialogue(messages, actors);
    }

    void Update()
    {
        if (Input.GetButtonDown("Read") && isPlayerInRange)
        {
            StartDialogue();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IsPlayerCheck(collision, isEntering: true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IsPlayerCheck(collision, isEntering: false);
    }

    void IsPlayerCheck(Collider2D collision, bool isEntering)
    {
        var player = Player.Instance;

        if (player == null)
            return;

        if (collision.gameObject == player.gameObject)
            isPlayerInRange = isEntering;
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