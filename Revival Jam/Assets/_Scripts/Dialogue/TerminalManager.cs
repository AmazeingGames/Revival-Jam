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

    [Header("Terminal Display")]
    [SerializeField] TMP_InputField terminalInput;
    [SerializeField] GameObject userInputLine;
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] RectTransform commandLineContainer;

    [Header("Terminal")]
    [SerializeField] Interpreter interpreter;
    [SerializeField] float heightIncrease = 35f;

    [Header("Display Settings")]
    [SerializeField] ScrollType scrollType;
    [SerializeField] float smoothScrollSpeed;

    enum ScrollType { JumpTo, SmoothScroll }

    List<string> previousInput;

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

    private void OnGUI()
    {
        if (terminalInput.isFocused && terminalInput.text != "" && Input.GetKeyDown(KeyCode.Return))
        {
            string userInput = terminalInput.text;

            terminalInput.text = "";

            SetCommandLineSize(CommandLineSet.Add, heightIncrease);

            MimicInput(userInput);

            //Run the user's found command
            interpreter.Interpret(userInput).Execute();
        }
    }

    //Creates and accommodates for a new line of text
    public ResponseLine CreateResponseLine()
    {
        //Create line
        ResponseLine response = Instantiate(responseLine, commandLineContainer);
        response.transform.SetAsLastSibling();

        //Accommodates to fit new line
        SetCommandLineSize(CommandLineSet.Add, heightIncrease);

        responseLines.Add(response);
        return response;
    }

    //Creates a copy of the givent text in the terminal
    void MimicInput(string userInput)
    {
        GameObject message = Instantiate(directoryLine, commandLineContainer);
        message.transform.SetSiblingIndex(commandLineContainer.transform.childCount - 1);
        message.GetComponentInChildren<TMP_Text>().text = userInput;

        mimicLines.Add(message);
    }

    //Stops the user from writing any responses
    public void DisableInput()
    {
        terminalInput.gameObject.SetActive(false);
    }

    public void SetCommandLineSize(CommandLineSet setSettings, float newYSize = 0)
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
        terminalInput.gameObject.SetActive(true);

        userInputLine.transform.SetAsLastSibling();
        terminalInput.ActivateInputField();
        terminalInput.Select();

        Debug.Log("Readied Input");

        if (scrollToBottom)
        {
            ScrollToBottom();
        }
    }

    public void ScrollToBottom()
    {
        scrollRect.velocity = new Vector2(0, smoothScrollSpeed);
        Debug.Log("Scrolled to bottom!");

        /*
        switch (scrollType)
        {
            case ScrollType.JumpTo:
                scrollRect.verticalNormalizedPosition = 0;
                break;

            case ScrollType.SmoothScroll:
                scrollRect.velocity = new Vector2(0, smoothScrollSpeed);
                break;
        }
        */
    }
}
