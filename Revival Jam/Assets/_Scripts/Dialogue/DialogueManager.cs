using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DialogueManager : Singleton<DialogueManager>
{
    [SerializeField] float globalTextSpeed;

    [SerializeField] Image dialoguePortrait;
    [SerializeField] TextMeshProUGUI dialogueName;
    [SerializeField] TextMeshProUGUI dialogueSpeech;
    [SerializeField] RectTransform dialogueBackground;

    List<Message> currentMessages;
    List<Actor> currentActors;
    int activeMessage = 0;

    public static bool isDialogueRunning = false;

    bool textFinished = false;

    //True -> Enter Dialogue
    //False -> Exit Dialogue
    public static event Action<bool> EnterDialogue;

    private void Start()
    {
        CloseDialogueBox();
    }

    void Update()
    {
        if (isDialogueRunning && Input.GetKeyDown(KeyCode.Space))
        {
            if (textFinished)
            {
                NextMessage();
            }
            else
            {
                DisplayMessageInstant();
            }
        }
    }

    //Opens the dialogue box and prepares for the first line of a dialogue
    public void StartDialogue(List<Message> messages, List<Actor> actors)
    {
        //EnterDialogue?.Invoke(true);

        currentActors = actors;
        currentMessages = messages;
        activeMessage = 0;

        isDialogueRunning = true;
        OpenDialogueBox();
        dialogueSpeech.text = string.Empty;

        StartCoroutine(DisplayMessageSlow());

        //DisplayMessageInstant();

        Debug.Log($"Started Convo | Length {messages.Count}");
    }

    //Sets the name and portrait for the current line of dialogue
    //Not currently used to its fullest, since all dialogue comes from the same individual
    void SetActor()
    {
        Actor actor = currentActors[currentMessages[activeMessage].actorId];

        dialogueName.text = actor.name;
        dialoguePortrait.sprite = actor.sprite;
    }

    //Displays the current line one character at a time
    IEnumerator DisplayMessageSlow()
    {
        SetActor();

        textFinished = false;

        Message displayMessage = currentMessages[activeMessage];

        foreach (char character in displayMessage.message)
        {
            if (textFinished)
                yield break;

            dialogueSpeech.text += character;
            yield return new WaitForSeconds(globalTextSpeed);
        }
        textFinished = true;
    }

    //Displays the message all at once instead of one character at a time
    void DisplayMessageInstant()
    {
        StopCoroutine(DisplayMessageSlow());

        SetActor();

        Message displayMessage = currentMessages[activeMessage];

        dialogueSpeech.text = displayMessage.message;

        textFinished = true;
    }

    //Starts the next line of the current dialogue, and exits if there are none left
    void NextMessage()
    {
        activeMessage++;

        if (activeMessage >= currentMessages.Count)
        {
            ExitDialogue();
            return;
        }

        if (!currentMessages[activeMessage].continuePreviousMessage)
            dialogueSpeech.text = string.Empty;

        StartCoroutine(DisplayMessageSlow());
    }

    //Finishes the dialogue and informs listeners dialogue has ended
    void ExitDialogue()
    {
        EnterDialogue?.Invoke(false);

        CloseDialogueBox();
        isDialogueRunning = false;

        Debug.Log("Conversation finished");
    }

    void OpenDialogueBox()
    {
        dialogueBackground.gameObject.SetActive(true);
    }

    void CloseDialogueBox()
    {
        dialogueBackground.gameObject.SetActive(false);
    }
}
