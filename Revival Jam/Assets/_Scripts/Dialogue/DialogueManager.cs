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
using Unity.VisualScripting;

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
    int activeMessage = 0;

    VertexJitter currentJitter;
    TextMeshProUGUI dialogueSpeech;
    TextMeshProUGUI dialogueName;
    Image dialoguePortrait;
    RectTransform dialogueBackground;
    DialogueType dialogueType;
    EventSounds textSFX;

    public static bool isDialogueRunning = false;

    bool textFinished = false;

    //True -> Enter Dialogue
    //False -> Exit Dialogue
    public static event Action<bool> EnterDialogue;

    float blipTimer;

    float textSpeed;

    string instantMessageText = string.Empty;


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

        if (dialogueType == DialogueType.Note && !IsFocusedOn(FocusedOn.Arcade))
        {
            Debug.Log("Not focused on arcade");
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (textFinished)
                NextMessage();
            else
                DisplayMessageInstant();
        }

        blipTimer -= Time.deltaTime;
    }

    //Opens the dialogue slot and prepares for the first line of a dialogue
    public void StartDialogue(Dialogue dialogue, DialogueType type = DialogueType.Note)
    {
        if (isDialogueRunning)
            return;

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
    void SetDialogueProperties()
    {
        Message displayMessage = currentMessages[activeMessage];

        Actor actor = currentActors[displayMessage.actorId];

        //Set Speaker
        if (dialogueName != null)
            dialogueName.text = actor.name;
        if (dialoguePortrait != null)
            dialoguePortrait.sprite = actor.sprite;

        //Set Jitter
        currentJitter.shouldJitter = displayMessage.shouldJitter;
        Debug.Log($"Set {currentJitter} jitter to {displayMessage.shouldJitter}");

        //Set Speed
        textSpeed = globalTextSpeed;
        if (displayMessage.overrideSpeed)
            textSpeed = displayMessage.newSpeed;
    }

    //Displays the current line one character at a time
    IEnumerator DisplayMessageSlow()
    {
        SetDialogueProperties();

        textFinished = false;

        Message displayMessage = currentMessages[activeMessage];

        for (int i = 0; i < displayMessage.message.Length; i++)
        {
            //Is it better to make the variable inside or outside the loop?
            char character = displayMessage.message[i];
            bool skipSound = false;
            bool skipWait = false;

            if (textFinished)
                yield break;

            //Create dictionary for 'start' and 'end' 'command characters' and see if the given character is a valid key
            if (character == '<')
            {
                var subString = displayMessage.message[i..];

                var commandText = GetBetween(subString, character.ToString(), ">");

                dialogueSpeech.text += commandText;

                i += commandText.Length - 1;

                skipSound = true;
                skipWait = true;
            }
            else
                dialogueSpeech.text += character;

            //Makes sure empty characters don't take up time or make noise
            if (silentCharacters.Contains(character) && skipSilentCharacters && useSilentCharacters)
            {
                skipSound = true;
                skipWait = true;
            }

            //Plays sound for each character (at a fixed rate)
            if (blipTimer <= 0 && !skipSound)
            {
                TriggerAudioClip(textSFX, transform);
                blipTimer = blipDelay;
            }

            //Displays characters one at a time
            float wait = textSpeed;
            if (skipWait)
                wait = 0;
            yield return new WaitForSeconds(wait);
        }
        textFinished = true;
    }

    //Returns the content of a string that appears between two values
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

    //Bug Fix Idea: Issue where instant display doesn't properly display instant messages
    //Create a 'load message' string variable, it holds the message to display using the continued messages, clear it when there isn't a continuous message, add to it when there is
    
    //Displays the message all at once instead of one character at a time
    void DisplayMessageInstant(bool playSFX = true)
    {
        StopCoroutine(DisplayMessageSlow());

        SetDialogueProperties();

        Message displayMessage = currentMessages[activeMessage];

        //Maybe use a stringbuilder for performance
        bool continueMessage = currentMessages[activeMessage].continuePreviousMessage;
        if (continueMessage && activeMessage != 0)
        {
            instantMessageText += currentMessages[activeMessage - 1].message;
        }
        if (!continueMessage)
            instantMessageText = string.Empty;

        dialogueSpeech.text = string.Empty;
        dialogueSpeech.text += instantMessageText + displayMessage.message;

        textFinished = true;

        if (playSFX)
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
        {
            Debug.Log("Did not continue message");
            dialogueSpeech.text = string.Empty;
        }

        StartCoroutine(DisplayMessageSlow());
    }

    //Finishes the dialogue and informs listeners dialogue has ended
    void ExitDialogue(bool grantAbility = true)
    {
        EnterDialogue?.Invoke(false);
        
        if (grantAbility && (ItemAndAbilityManager.Instance != null && currentDialogue.NewInformation != ItemAndAbilityManager.ItemsAndAbilities.None))
            ItemAndAbilityManager.Instance.GainAbilityInformation(currentDialogue.NewInformation);

        if (!textFinished)
            DisplayMessageInstant(false);

        noteDialogueBackground.gameObject.SetActive(false);
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
