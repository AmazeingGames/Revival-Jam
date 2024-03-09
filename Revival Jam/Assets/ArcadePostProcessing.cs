using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ArcadePostProcessing : MonoBehaviour
{
    [SerializeField] Volume TerminalPostProcessing;
    [SerializeField] Volume LevelPostProcessing;


    private void OnEnable()
    {
        DialogueManager.DialogueEvent += HandleEnterDialogue;
    }

    private void OnDisable()
    {
        DialogueManager.DialogueEvent -= HandleEnterDialogue;
    }

    private void Update()
    {
        //Starts level post on level start
        if (!TerminalPostProcessing.isActiveAndEnabled)
            LevelPostProcessing.gameObject.SetActive(true);
    }

    //true -> open dialogue
    //false -> close dialogue
    //Responsible for deciding which post processing to use, since we want to use different processes for the terminal and the level
    void HandleEnterDialogue(bool dialogueOpen)
    {
        TerminalPostProcessing.gameObject.SetActive(dialogueOpen);
        LevelPostProcessing.gameObject.SetActive(!dialogueOpen);
    }
}
