using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using static ArcadeGameManager;
using static AudioManager;

public class DialogueManager : Singleton<DialogueManager>
{
    public enum DialogueType { Note, Player, Meta }

    [SerializeField] float globalTextSpeed;
    [SerializeField] float blipDelay;

    [Header("Note Dialogue")]
    [SerializeField] Canvas noteDialogueCanvas;
    [SerializeField] RectTransform noteDialogueBackground;
    [SerializeField] Image noteDialoguePortrait;
    [SerializeField] TextMeshProUGUI noteDialogueName;
    [SerializeField] TextMeshProUGUI noteDialogueSpeech;

    [Header("Meta Dialogue")]
    [SerializeField] RectTransform metaDialogueBackground;
    [SerializeField] TextMeshProUGUI metaDialogueSpeech;


    Dialogue currentDialogue;
    List<Message> currentMessages;
    List<Actor> currentActors;
    int activeMessage = 0;

    TextMeshProUGUI dialogueSpeech;
    TextMeshProUGUI dialogueName;
    Image dialoguePortrait;
    RectTransform dialogueBackground;
    EventSounds textSFX;

    public static bool isDialogueRunning = false;

    bool textFinished = false;

    //True -> Enter Dialogue
    //False -> Exit Dialogue
    public static event Action<bool> EnterDialogue;

    float blipTimer;

    float textSpeed;

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

        blipTimer -= Time.deltaTime;
    }

    //Opens the dialogue slot and prepares for the first line of a dialogue
    public void StartDialogue(Dialogue dialogue, DialogueType type = DialogueType.Note)
    {
        EnterDialogue?.Invoke(true);

        currentDialogue = dialogue;
        currentActors = dialogue.Actors;
        currentMessages = dialogue.Messages;
        activeMessage = 0;

        isDialogueRunning = true;
        OpenDialogueBox(type);
        SetDialougeSFX(type);

        dialogueSpeech.text = string.Empty;
        StartCoroutine(DisplayMessageSlow());

        //DisplayMessageInstant();

        Debug.Log($"Started Convo -- {currentDialogue.NewInformation} | Length {dialogue.Messages.Count}");
    }

    void SetDialougeSFX(DialogueType dialogueType)
    {
        textSFX = dialogueType switch
        {
            _ => EventSounds.ConsoleDialogue
        };
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

        textSpeed = globalTextSpeed;

        if (displayMessage.overrideSpeed)
            textSpeed = displayMessage.newSpeed;

        foreach (char character in displayMessage.message)
        {
            if (textFinished)
                yield break;

            dialogueSpeech.text += character;

            if (blipTimer <= 0)
            {
                TriggerAudioClip(textSFX, transform);
                blipTimer = blipDelay;
            }

            yield return new WaitForSeconds(textSpeed);
        }
        textFinished = true;
    }

    //Bug Fix Idea: Issue where instant display doesn't properly display instant messages
    //Create a 'load message' string variable, it holds the message to display using the continues messages, clear it when there isn't a continuous message, add to it when there is
    //Displays the message all at once instead of one character at a time
    void DisplayMessageInstant()
    {
        StopCoroutine(DisplayMessageSlow());

        SetActor();

        Message displayMessage = currentMessages[activeMessage];

        dialogueSpeech.text = displayMessage.message;

        textFinished = true;

        TriggerAudioClip(textSFX, transform);
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
            noteDialogueSpeech.text = string.Empty;

        StartCoroutine(DisplayMessageSlow());
    }

    //Finishes the dialogue and informs listeners dialogue has ended
    void ExitDialogue()
    {
        EnterDialogue?.Invoke(false);
        
        //Why does this happen twice?
        //^^^What is this comment for? Did I fix this??? - 12/8/23
        if (ItemAndAbilityManager.Instance != null && currentDialogue.NewInformation != ItemAndAbilityManager.ItemsAndAbilities.None)
            ItemAndAbilityManager.Instance.GainAbilityInformation(currentDialogue.NewInformation);

        CloseDialogueBox();
        isDialogueRunning = false;

        Debug.Log("Conversation finished");
    }

    void OpenDialogueBox(DialogueType dialogueType)
    {
        switch (dialogueType)
        {
            case DialogueType.Note:
                dialogueBackground = noteDialogueBackground;
                dialogueSpeech = noteDialogueSpeech;
                dialogueName = noteDialogueName;
                dialoguePortrait = noteDialoguePortrait;
                break;

            case DialogueType.Player:
                break;

            case DialogueType.Meta:
                dialogueBackground = metaDialogueBackground;
                dialogueSpeech = metaDialogueSpeech;
                break;
        }

        dialogueBackground.gameObject.SetActive(true);
    }

    void CloseDialogueBox()
    {
        noteDialogueBackground.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        AfterArcadeStateChange += HandleArcadeGameStateChange;
    }

    private void OnDisable()
    {
        AfterArcadeStateChange -= HandleArcadeGameStateChange;
    }

    void HandleArcadeGameStateChange(ArcadeState arcadeState)
    {
        switch (arcadeState)
        {
            case ArcadeState.StartLevel:
                StartCoroutine(FindCamera());
                break;
        }
    }

    //Yes, find in coroutine is bad for performance, but it's hard to think of a better way with tile maps
    IEnumerator FindCamera()
    {
        while (Player.Instance == null)
            yield return null;

        GameObject arcadeCamera = null;

        while (arcadeCamera == null)
        {
            yield return null;

            arcadeCamera = GameObject.Find("Arcade Camera");
        }

        Debug.Log("Found arcade camera!");

        noteDialogueCanvas.worldCamera = arcadeCamera.GetComponent<Camera>();
    }
}
