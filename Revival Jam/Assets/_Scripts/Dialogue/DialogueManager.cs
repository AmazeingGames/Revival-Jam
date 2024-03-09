using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using static ArcadeGameManager;
using static AudioManager;
using static PlayerFocus;
using TMPro.Examples;
using static DialogueEventArgs.EventType;

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
    VertexJitter noteJitter;

    [Header("Meta Dialogue")]
    [SerializeField] RectTransform metaDialogueBackground;
    [SerializeField] TextMeshProUGUI metaDialogueSpeech;
    VertexJitter metaJitter;

    [Header("Text Properties")]
    [SerializeField] bool skipSilentCharacters = false;
    [SerializeField] bool useSilentCharacters = false;

    [Header("Special Characters")]
    [SerializeField] List<char> silentCharacters;

    Dialogue currentDialogue;
    List<Message> currentMessages;
    List<Actor> currentActors;
    int currentIndex = 0;
    Message CurrentMessage => currentMessages[currentIndex];

    VertexJitter currentJitter;
    TextMeshProUGUI dialogueSpeech;
    TextMeshProUGUI dialogueName;
    Image dialoguePortrait;
    RectTransform dialogueBackground;
    DialogueType dialogueType;
    OneShotSounds textSFX;

    public static bool isDialogueRunning = false;

    bool textFinished = false;

    public static event EventHandler<DialogueEventArgs> DialogueEvent;

    float blipTimer;

    float textSpeed;

    string instantMessageText = string.Empty;

    Coroutine autoPlayMessage;
    private void Start()
    {
        if (metaDialogueSpeech != null)
            metaJitter = metaDialogueSpeech.GetComponent<VertexJitter>();
        if (noteDialogueSpeech != null)
            noteJitter = noteDialogueSpeech.GetComponent<VertexJitter>();

        noteDialogueBackground.gameObject.SetActive(false);
    }

    void Update()
    {
        ContinueDialogue();
    }

    //Checks when to update the dialogue to display the full message or to display the next message
    void ContinueDialogue()
    {
        if (!isDialogueRunning)
            return;

        // Only continue terminal dialogue while the player is at the machine
        if (dialogueType == DialogueType.Note && !IsFocusedOn(FocusedOn.Arcade))
            return;

        // Some messages we want to play without player interference
        if (CurrentMessage.autoPlay && textFinished)
            autoPlayMessage ??= StartCoroutine(AutoplayNextMessage(CurrentMessage.hangTime));

        // Other messages we let the player skip
        else if (Input.GetKeyDown(KeyCode.Space) && !CurrentMessage.autoPlay)
        {
            if (textFinished)
                NextMessage();
            else
                DisplayMessageInstant();
        }

        // Time until next beep sfx
        blipTimer -= Time.deltaTime;
    }

    IEnumerator AutoplayNextMessage(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);

        autoPlayMessage = null;
        NextMessage();
    }

    public void StartDialogue(Dialogue dialogue, DialogueType type = DialogueType.Note)
    {
        if (isDialogueRunning)
            return;

        DialogueEvent?.Invoke(this, new(eventType: DialogueStart, grantInformation: false, dialogue));

        currentDialogue = dialogue;
        currentActors = dialogue.Actors;
        currentMessages = dialogue.Messages;
        currentIndex = 0;

        isDialogueRunning = true;
        OpenDialogueBox(type);
        SetDialougeSFX(type);

        dialogueSpeech.text = string.Empty;
        StartCoroutine(DisplayMessageSlow());
    }

    // Eventually we want different dialogue to have different sfx
    void SetDialougeSFX(DialogueType dialogueType)
    {
        textSFX = dialogueType switch
        {
            _ => OneShotSounds.ConsoleDialogue
        };
    }

    // Because each dialogue, and sometimes each line of dialogue, has different properties, we set them each time
    void SetDialogueProperties()
    {
        Message displayMessage = currentMessages[currentIndex];

        Actor actor = currentActors[displayMessage.actorId];

        //Set Speaker
        if (dialogueName != null)
            dialogueName.text = actor.name;
        if (dialoguePortrait != null)
            dialoguePortrait.sprite = actor.sprite;

        //Set Jitter
        if (currentJitter != null)
            currentJitter.shouldJitter = displayMessage.shouldJitter;
        Debug.Log($"Set {currentJitter} jitter to {displayMessage.shouldJitter}");

        //Set Speed
        textSpeed = globalTextSpeed;
        if (displayMessage.overrideSpeed)
            textSpeed = displayMessage.newSpeed;
    }

    // Displays the current line one character at a time
    IEnumerator DisplayMessageSlow()
    {
        SetDialogueProperties();

        textFinished = false;

        Message displayMessage = currentMessages[currentIndex];

        for (int i = 0; i < displayMessage.message.Length; i++)
        {
            char current = displayMessage.message[i];
            bool skipSound = false;
            bool skipWait = false;

            if (textFinished)
                yield break;

            // Used to instantly display rich text at instant speed
            if (current == '<')
            {
                string commandStart = displayMessage.message[i..];
                string command = GetBetween(commandStart, current.ToString(), ">");

                dialogueSpeech.text += command;

                i += command.Length - 1;

                skipSound = true;
                skipWait = true;
            }
            else
                dialogueSpeech.text += current;

            // Makes sure empty characters don't take up time or make noise
            if (silentCharacters.Contains(current) && skipSilentCharacters && useSilentCharacters)
            {
                skipSound = true;
                skipWait = true;
            }

            // Plays sound for each non-silent character at a fixed rate
            if (blipTimer <= 0 && !skipSound)
            {
                TriggerAudioClip(textSFX, transform);
                blipTimer = blipDelay;
            }

            // Displays silent characters instantly
            // Delay between non-silent characters
            float wait = skipWait ? 0 : textSpeed;
            yield return new WaitForSeconds(wait);
        }
        textFinished = true;
    }

    public static string GetBetween(string stringSource, string startValue, string endValue)
    {
        if (stringSource.Contains(startValue) && stringSource.Contains(endValue))
        {
            int Start = stringSource.IndexOf(startValue, 0) + startValue.Length;
            int End = stringSource.IndexOf(endValue, Start);

            return $"{startValue}{stringSource[Start..End]}{endValue}";
        }
        return "";
    }

    // Displays current message all at once
    // Displays any previous message that we need to continue
    void DisplayMessageInstant(bool playSFX = true)
    {
        StopCoroutine(DisplayMessageSlow());
        SetDialogueProperties();

        Message currentMessage = currentMessages[currentIndex];

        bool continueMessage = currentMessages[currentIndex].continuePreviousMessage;

        if (continueMessage && currentIndex != 0)
            instantMessageText += currentMessages[currentIndex - 1].message;

        else if (!continueMessage)
            instantMessageText = string.Empty;

        dialogueSpeech.text = string.Empty;
        dialogueSpeech.text += instantMessageText + currentMessage.message;

        textFinished = true;

        if (playSFX)
            TriggerAudioClip(textSFX, transform);
    }

    // Starts the next line of the current dialogue
    // Ends dialogue if this is the last message
    void NextMessage()
    {
        currentIndex++;

        // No more messages
        if (currentIndex >= currentMessages.Count)
        {
            ExitDialogue();
            return;
        }

        // Clears speech
        if (!currentMessages[currentIndex].continuePreviousMessage)
            dialogueSpeech.text = string.Empty;

        StartCoroutine(DisplayMessageSlow());
    }

    // Informs listeners that dialogue has ended and closes dialogue box
    void ExitDialogue(bool grantAbility = true)
    {
        DialogueEvent?.Invoke(this, new(eventType: DialogueExit, grantAbility, currentDialogue));

        if (!textFinished)
            DisplayMessageInstant(false);

        noteDialogueBackground.gameObject.SetActive(false);
        isDialogueRunning = false;

        Debug.Log("Conversation finished");
    }

    // Different dialogue types have different canvases
    // Sets which canvas we're working on
    void OpenDialogueBox(DialogueType dialogueType)
    {
        switch (dialogueType)
        {
            case DialogueType.Note:
                dialogueBackground = noteDialogueBackground;
                dialogueSpeech = noteDialogueSpeech;
                dialogueName = noteDialogueName;
                dialoguePortrait = noteDialoguePortrait;
                currentJitter = noteJitter;
                break;

            case DialogueType.Player:
                break;

            case DialogueType.Meta:
                dialogueBackground = metaDialogueBackground;
                dialogueSpeech = metaDialogueSpeech;
                dialoguePortrait = null;
                dialogueName = null;
                currentJitter = metaJitter;
                break;
        }

        this.dialogueType = dialogueType; 
        dialogueBackground.gameObject.SetActive(true);
    }

    private void OnEnable()
        => AfterArcadeStateChange += HandleArcadeGameStateChange;

    private void OnDisable()
        => AfterArcadeStateChange -= HandleArcadeGameStateChange;

    void HandleArcadeGameStateChange(ArcadeState arcadeState)
    {
        switch (arcadeState)
        {
            case ArcadeState.StartLevel:
            case ArcadeState.Win:
                Debug.Log("Started search for camera");
                StartCoroutine(FindCamera());
                break;

            case ArcadeState.Lose:
            case ArcadeState.RestartLevel:
                if (isDialogueRunning)
                    ExitDialogue(grantAbility: false);
                break;
        }
    }

    //Yes; 'find' in coroutine is bad for performance, but I dumb
    IEnumerator FindCamera()
    {
        while (Player.Instance == null)
        {
            Debug.Log("Player is null");
            yield return null;
        }

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

public class DialogueEventArgs : EventArgs
{
    public enum EventType { DialogueStart, DialogueExit }

    public readonly EventType eventType;
    
    readonly bool shouldGrantInformation;
    readonly ItemAndAbilityManager.Abilities abilityToGrant;
    readonly ItemAndAbilityManager.Tools toolToGrant;

    public DialogueEventArgs(EventType eventType, bool grantInformation, Dialogue dialogue)
    {
        this.eventType = eventType;
        this.shouldGrantInformation = grantInformation;
        
        abilityToGrant = dialogue.NewAbility;
        toolToGrant = dialogue.NewTool;
    }

    public ItemAndAbilityManager.Abilities GetAbility()
        => shouldGrantInformation ? abilityToGrant : ItemAndAbilityManager.Abilities.None;
    public ItemAndAbilityManager.Tools GetTool()
        => shouldGrantInformation ? toolToGrant : ItemAndAbilityManager.Tools.None;
}
