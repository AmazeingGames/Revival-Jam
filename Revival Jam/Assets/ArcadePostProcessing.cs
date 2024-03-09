using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ArcadePostProcessing : MonoBehaviour
{
    [SerializeField] Volume TerminalPostProcessing;
    [SerializeField] Volume LevelPostProcessing;


    private void OnEnable()
        => DialogueManager.DialogueEvent += HandleDialogueEvent;

    private void OnDisable()
        => DialogueManager.DialogueEvent -= HandleDialogueEvent;

    private void Update()
    {
        // Starts level post on level start
        if (!TerminalPostProcessing.isActiveAndEnabled)
            LevelPostProcessing.gameObject.SetActive(true);
    }

    // Uses different post-processing for the terminal and the level
    void HandleDialogueEvent(object sender, DialogueEventArgs eventArgs)
    {
        bool isOpening = eventArgs.eventType == DialogueEventArgs.EventType.DialogueStart;

        TerminalPostProcessing.gameObject.SetActive(isOpening);
        LevelPostProcessing.gameObject.SetActive(!isOpening);
    }
}
