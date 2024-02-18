using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TerminalManager : Singleton<TerminalManager>
{
    [Header("Instantiating")]
    [SerializeField] GameObject directoryLine;
    [SerializeField] ResponseLine responseLine;
    [SerializeField] GameObject userInputLinePrefab;

    [Header("Terminal Display")]
    [SerializeField] TMP_InputField terminalInput;
    [SerializeField] GameObject userInputLine;
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] RectTransform commandLineContainer;

    [Header("Terminal")]
    //This should always be the height of the object we are instantiating in the command line
    [SerializeField] float heightIncrease = 35f;

    [Header("Settings")]
    [SerializeField] ScrollType scrollType;
    [SerializeField] float smoothScrollSpeed;
    [SerializeField] bool disableInputParent;

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
    {   
        DialogueManager.EnterDialogue += HandleFinishDialogue;
    }

    private void OnDisable()
    {
        DialogueManager.EnterDialogue -= HandleFinishDialogue;
    }

    private void Update()
    {
        Switch();
    }

    void Switch()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            scrollRect.gameObject.SetActive(!scrollRect.gameObject.activeSelf);
        }
    }

    void HandleFinishDialogue(bool isEntering)
    {
        //Ready Input on Note end
        if (!isEntering)
            ReadyInput();
    }

    private void OnGUI()
    {
        if (terminalInput.isFocused && terminalInput.text != "" && Input.GetKeyDown(KeyCode.Return))
        {
            string userInput = terminalInput.text;

            terminalInput.text = "";

            SetCommandLineSize(CommandLineSet.Add, heightIncrease);

            MimicInput(userInput);

            //Run the user's found command
            Interpreter.Instance.Interpret(userInput).Execute();
        }
    }

    //Creates, and accommodates for, a new line of text
    public ResponseLine CreateResponseLine()
    {
        ResponseLine responseLn = Instantiate(responseLine, commandLineContainer);
        responseLn.transform.SetAsLastSibling();

        SetCommandLineSize(CommandLineSet.Add, heightIncrease);

        responseLines.Add(responseLn);

        //User Input line always needs to be at the very bottom, or else the text will jump around
        userInputLine.transform.SetAsLastSibling();
        return responseLn;
    }

    //Creates a copy of the givent text in the terminal
    void MimicInput(string userInput)
    {
        GameObject message = Instantiate(directoryLine, commandLineContainer);
        message.transform.SetSiblingIndex(commandLineContainer.transform.childCount - 1);
        message.GetComponentInChildren<TMP_Text>().text = userInput;

        mimicLines.Add(message);

        //User Input line always needs to be at the very bottom, or else the text will jump around
        userInputLine.transform.SetAsLastSibling();
    }

    public void SetCommandLineSize(CommandLineSet setSettings, float newYSize = 0)
    {
        //Debug.Log($"Set command line size : {setSettings} : {newYSize}");
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
        if (terminalInput.transform.parent != null)
            terminalInput.transform.parent.gameObject.SetActive(true);

        terminalInput.gameObject.SetActive(true);

        userInputLine.transform.SetAsLastSibling();
        terminalInput.ActivateInputField();
        terminalInput.Select();

        if (scrollToBottom)
        {
            ScrollToBottom();
        }
    }

    
    //Stops the user from writing any responses
    public void DisableInput()
    {
        if (disableInputParent && terminalInput.transform.parent != null)
            terminalInput.transform.parent.gameObject.SetActive(false);
        terminalInput.gameObject.SetActive(false);
    }

    public void ScrollToBottom()
    {
        scrollRect.velocity = new Vector2(0, smoothScrollSpeed);
    }
}
