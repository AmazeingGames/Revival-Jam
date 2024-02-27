using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static DialogueManager;
using static DialogueManager.DialogueEventArgs;

public class TerminalManager : MonoBehaviour
//public class TerminalManager : Singleton<TerminalManager>
{
    [Header("Instantiating")]
    [SerializeField] GameObject directoryLine;
    [SerializeField] ResponseLine responseLine;
    [SerializeField] ResponseLine emptyLine;

    [field: Header("Terminal Display Instances")]
    [field: SerializeField] public TMP_InputField TerminalInput { get; private set; }
    [SerializeField] GameObject userInputLine;
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] RectTransform commandLineContainer;

    [Header("Terminal")]
    //This should generally be the height of the object we are instantiating in the command line

    [Header("Scroll")]
    [SerializeField] AutoScroll autoScroll;

    [Header("Debug")]
    [SerializeField] bool disableInputParent;
    [SerializeField] float heightIncrease = 30f; //Used to properly scroll through the commands


    enum ScrollType { JumpTo, SmoothScroll }

    public enum CommandLineSet { Set, Add, Reset, Subtract }

    //Create a pool of response lines later
    public IList<ResponseLine> ResponseLines { get => responseLines.AsReadOnly(); }
    readonly List<ResponseLine> responseLines = new();

    //Create a pool of mimic lines later
    public IList<GameObject> MimicLines { get => mimicLines.AsReadOnly(); }
    readonly List<GameObject> mimicLines = new();
    Vector2 startingContainerSize;

    private void Start()
    {
        startingContainerSize = commandLineContainer.sizeDelta;
    }

    private void OnEnable()
        => RaiseDialogue += HandleDialogue;
    private void OnDisable()
        => RaiseDialogue -= HandleDialogue;

    //Readies player Input on Note end
    void HandleDialogue(object sender, DialogueEventArgs dialogueEventArgs)
    {
        if (dialogueEventArgs.dialogueState == DialogueState.Exiting)
            ReadyInput();
    }

    //Creates, and accommodates for, a new line of text
    public ResponseLine CreateResponseLine(bool willBeEmpty = false)
    {
        var line = willBeEmpty ? emptyLine : responseLine;
        ResponseLine responseLn = Instantiate(line, commandLineContainer);
        responseLn.transform.SetAsLastSibling();

        AdjustCommandLineSize(CommandLineSet.Add, heightIncrease);
        responseLines.Add(responseLn);

        userInputLine.transform.SetAsLastSibling(); //User Input line always needs to be at the very bottom, or else the text will jump around
        return responseLn;
    }

    //Creates a copy of the givent text in the terminal
    public void MimicInput(string userInput)
    {
        GameObject message = Instantiate(directoryLine, commandLineContainer);
        message.transform.SetSiblingIndex(commandLineContainer.transform.childCount - 1);
        message.GetComponentInChildren<TMP_Text>().text = userInput;

        mimicLines.Add(message);

        //User Input line always needs to be at the very bottom, or else the text will jump around
        userInputLine.transform.SetAsLastSibling();
    }

    //Adjusting the command line needs to be done whenver we create a new line of text
    //This is done to make sure the scrolling works properly with the scroll rect
    public void AdjustCommandLineSize(CommandLineSet setSettings, float newYSize = 0)
    {
        Debug.Log($"Set command line size : {setSettings} : {newYSize}");
        switch (setSettings)
        {
            case CommandLineSet.Set:
                commandLineContainer.sizeDelta = new Vector2(commandLineContainer.sizeDelta.x, newYSize);
                break;

            case CommandLineSet.Add:
                commandLineContainer.sizeDelta = new Vector2(commandLineContainer.sizeDelta.x, newYSize + commandLineContainer.sizeDelta.y);
                break;

            case CommandLineSet.Reset:
                commandLineContainer.sizeDelta = new Vector2(commandLineContainer.sizeDelta.x, startingContainerSize.y);
                break;

            case CommandLineSet.Subtract:
                commandLineContainer.sizeDelta = new Vector2(commandLineContainer.sizeDelta.x, Mathf.Abs(newYSize - commandLineContainer.sizeDelta.y));
                break;
        }
    }

    //Positions the input at the bottom and readies the user to write a new command
    public void ReadyInput(bool scrollToBottom = true)
    {
        if (TerminalInput.transform.parent != null)
            TerminalInput.transform.parent.gameObject.SetActive(true);

        TerminalInput.gameObject.SetActive(true);

        userInputLine.transform.SetAsLastSibling();
        TerminalInput.ActivateInputField();
        TerminalInput.Select();

        if (scrollToBottom)
            autoScroll.ScrollTimes(1);
    }

    //Stops the user from writing any responses
    public void DisableInput()
    {
        if (disableInputParent && TerminalInput.transform.parent != null)
            TerminalInput.transform.parent.gameObject.SetActive(false);
        TerminalInput.gameObject.SetActive(false);
    }
}
