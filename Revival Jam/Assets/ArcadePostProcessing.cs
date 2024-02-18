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
        DialogueManager.RaiseDialogue += HandleEnterDialogue;
    }

    private void OnDisable()
    {
        DialogueManager.RaiseDialogue -= HandleEnterDialogue;
    }

    private void Update()
    {
        //Starts level post on level start
        if (!TerminalPostProcessing.isActiveAndEnabled)
            LevelPostProcessing.gameObject.SetActive(true);
    }

    //Responsible for deciding which post processing to use, either the terminal or arcade processing
    void HandleEnterDialogue(object sender, DialogueManager.DialogueEventArgs dialogueEventArgs)
    {
        bool isOpeningDialogue = dialogueEventArgs.dialogueState == DialogueManager.DialogueEventArgs.DialogueState.Entering;

        TerminalPostProcessing.gameObject.SetActive(isOpeningDialogue);
        LevelPostProcessing.gameObject.SetActive(!isOpeningDialogue);
    }
}
