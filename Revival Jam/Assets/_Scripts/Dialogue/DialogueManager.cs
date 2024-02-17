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

public class DialogueManager : Singleton<DialogueManager>
{
    public enum DialogueType { Terminal, Player, Meta }

    [SerializeField] float globalTextSpeed;
    [SerializeField] float blipDelay;

    [Header("Terminal Dialogue")]
    [SerializeField] Canvas terminalDialogueCanvas;
    [SerializeField] RectTransform terminalDialogueBackground;
    readonly VertexJitter noteJitter;

    [Header("Meta Dialogue")]
    [SerializeField] RectTransform metaDialogueBackground;
    [SerializeField] TextMeshProUGUI metaDialogueSpeech;
    VertexJitter metaJitter;

    [Header("Text Properties")]
    [SerializeField] float terminalGlobalSpeed;
    [SerializeField] bool skipSilentCharacters = false;
    [SerializeField] bool useSilentCharacters = false;

    [Header("Special Characters")]
    [SerializeField] List<char> silentCharacters;

    [Header("TerminalSettins")]
    [SerializeField] bool generateNewLines;

    Dialogue currentDialogue;
    List<Message> currentMessages;
    List<Actor> currentActors;
    int currentIndex = 0;
    DialogueType currentDialogueType;
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

    public static event Action<bool> EnterDialogue;

    float blipTimer;

    float textSpeed;

    string instantMessageText = string.Empty;

    Coroutine autoPlayMessage;
    private void Start()
    {
        if (metaDialogueSpeech != null)
            metaJitter = metaDialogueSpeech.GetComponent<VertexJitter>();

        //if (noteDialogueSpeech != null)
            //noteJitter = noteDialogueSpeech.GetComponent<VertexJitter>();

        terminalDialogueBackground.gameObject.SetActive(false);

        //REMOVE THIS LATER
        terminalDialogueBackground.gameObject.SetActive(true);
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

        if (dialogueType == DialogueType.Terminal && !IsFocusedOn(FocusedOn.Arcade))
        {
            Debug.Log("Not focused on arcade");
            return;
        }

        //Always autoplay for terminal messages
        if (dialogueType == DialogueType.Terminal)
        {
            if (textFinished)
                autoPlayMessage ??= StartCoroutine(AutoplayNextMessage(0));
        }
        //Check autoplay status
        else if (CurrentMessage.autoPlay && textFinished)
            autoPlayMessage ??= StartCoroutine(AutoplayNextMessage(CurrentMessage.hangTime));
        
        if (Input.GetKeyDown(KeyCode.Space) && !CurrentMessage.autoPlay)
        {
            if (textFinished)
                NextMessage();
            else
                DisplayMessageInstant();
        }

        blipTimer -= Time.deltaTime;
    }

    IEnumerator AutoplayNextMessage(float delaySeconds)
    {
        //Debug.Log($"Started auto play. Will play next message in {delaySeconds} seconds");

        yield return new WaitForSeconds(delaySeconds);

        autoPlayMessage = null;
        NextMessage();
    }

    //Opens the dialogue slot and prepares for the first line of a dialogue
    public void StartDialogue(DialogueBank.DialogueType dialogueKey)
    {
        if (isDialogueRunning)
            return;

        Dialogue dialogue = DialogueBank.Instance.DialogueDataByType[dialogueKey];

        EnterDialogue?.Invoke(true);

        currentDialogue = dialogue;
        currentActors = dialogue.Actors;
        currentMessages = dialogue.Messages;
        currentIndex = 0;
        currentDialogueType = dialogue.DialogueType;

        isDialogueRunning = true;
        OpenDialogueBox(currentDialogueType);
        SetDialougeSFX(currentDialogueType);

        dialogueSpeech.text = string.Empty;
        StartCoroutine(DisplayMessageSlow());

        //Debug.Log($"Started Convo -- {currentDialogue.NewInformation} | Length {dialogue.Messages.Count}");
    }

    void SetDialougeSFX(DialogueType dialogueType)
    {
        textSFX = dialogueType switch
        {
            _ => OneShotSounds.ConsoleDialogue
        };
    }

    //Sets the name and portrait for the current line of dialogue
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
        //Debug.Log($"Set {currentJitter} jitter to {displayMessage.shouldJitter}");

        //Set Speed
        textSpeed = globalTextSpeed;

        if (displayMessage.overrideSpeed)
            textSpeed = displayMessage.newSpeed;

        if (dialogueType == DialogueType.Terminal)
            textSpeed = terminalGlobalSpeed;
    }

    //Displays the current line one character at a time
    IEnumerator DisplayMessageSlow()
    {
        SetDialogueProperties();

        textFinished = false;

        Message displayMessage = currentMessages[currentIndex];

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

        Message displayMessage = currentMessages[currentIndex];

        //Maybe use a stringbuilder for performance
        bool continueMessage = currentMessages[currentIndex].continuePreviousMessage;
        if (continueMessage && currentIndex != 0)
        {
            instantMessageText += currentMessages[currentIndex - 1].message;
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
        Debug.Log($"Line {currentIndex} has {dialogueSpeech.textInfo.lineCount} lines");
        int numOfLines = dialogueSpeech.textInfo.lineCount;
        for (int i = 1; i < numOfLines; i++)
        {
            TerminalManager.Instance.CreateResponseLine();
        }

        currentIndex++;

        if (currentIndex >= currentMessages.Count)
        {
            ExitDialogue();
            return;
        }

        if (currentDialogueType == DialogueType.Terminal && generateNewLines)
            dialogueSpeech = TerminalManager.Instance.CreateResponseLine().responseText;
        else if (!currentMessages[currentIndex].continuePreviousMessage && currentDialogueType != DialogueType.Terminal)
        {
            //Creates a new line in the terminal
            Debug.Log("Did not continue message");
            dialogueSpeech.text = string.Empty;
        }

        if (currentDialogueType == DialogueType.Terminal)
        {
            TerminalManager.Instance.ScrollToBottom();
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

        //terminalDialogueBackground.gameObject.SetActive(false);
        isDialogueRunning = false;

        Debug.Log("Conversation finished");
    }

    //When starting a new dialogue, we need to make sure we write to the proper location
    void OpenDialogueBox(DialogueType dialogueType)
    {
        switch (dialogueType)
        {
            case DialogueType.Terminal:
                dialogueBackground = terminalDialogueBackground;
                currentJitter = noteJitter;
                dialoguePortrait = null;
                dialogueName = null;
                dialogueSpeech = TerminalManager.Instance.CreateResponseLine().responseText;

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

        terminalDialogueCanvas.worldCamera = arcadeCamera.GetComponent<Camera>();
    }
}
