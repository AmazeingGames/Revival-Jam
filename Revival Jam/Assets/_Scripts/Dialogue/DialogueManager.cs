using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    public static bool isActive = false;

    bool textFinished = false;

    private void Start()
    {
        CloseDialogueBox();
    }

    public void StartDialogue(List<Message> messages, List<Actor> actors)
    {
        currentActors = actors;
        currentMessages = messages;
        activeMessage = 0;

        isActive = true;
        OpenDialogueBox();
        dialogueSpeech.text = string.Empty;

        StartCoroutine(DisplayMessageSlow());

        //DisplayMessageInstant();

        Debug.Log($"Started Convo | Length {messages.Count}");
    }

    void SetActor()
    {
        Actor actor = currentActors[currentMessages[activeMessage].actorId];

        dialogueName.text = actor.name;
        dialoguePortrait.sprite = actor.sprite;
    }

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

    void DisplayMessageInstant()
    {
        StopCoroutine(DisplayMessageSlow());

        SetActor();

        Message displayMessage = currentMessages[activeMessage];

        dialogueSpeech.text = displayMessage.message;

        textFinished = true;
    }

    void NextMessage()
    {
        activeMessage++;

        if (activeMessage >= currentMessages.Count)
        {
            CloseDialogueBox();
            Debug.Log("Conversation finished");
            isActive = false;
            return;
        }

        dialogueSpeech.text = string.Empty;

        StartCoroutine(DisplayMessageSlow());
    }

    void OpenDialogueBox()
    {
        dialogueBackground.gameObject.SetActive(true);
    }

    void CloseDialogueBox()
    {
        dialogueBackground.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isActive && Input.GetKeyDown(KeyCode.Space))
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
}
