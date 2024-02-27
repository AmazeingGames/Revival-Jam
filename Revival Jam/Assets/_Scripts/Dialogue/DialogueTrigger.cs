using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[Obsolete]
public class DialogueTrigger : MonoBehaviour
{
    //Probably Change this to accept a scriptable object instead
    [Header("Dialogue")]
    [SerializeField] DialogueBank.DialogueType dialogueKey;

    [Header("Prompt")]
    [SerializeField] GameObject Icon;

    //Create a second dialogue that plays when you read the note again
    //The second time would highlight the important dialogue the player needs to do (kind of like a hint)
    bool hasPlayedDialogue;

    bool isPlayerInRange = false;

    public void StartDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogueKey);
    }

    void Update()
    {
        if (Input.GetButtonDown("Interact") && isPlayerInRange && PlayerFocus.IsFocusedOn(PlayerFocus.FocusedOn.Arcade))
            StartDialogue();
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
    [Header("Dialogue")]
    public int actorId;
    public string message;

    [Header("Properties")]
    public bool continuePreviousMessage;
    public bool shouldJitter;
    public bool overrideSpeed = false;
    public float newSpeed;

    [Header("Interaction")]
    public bool autoPlay;
    public float hangTime = .5f;
}

[Serializable]
public class Actor
{
    public string name;
    public Sprite sprite;
}