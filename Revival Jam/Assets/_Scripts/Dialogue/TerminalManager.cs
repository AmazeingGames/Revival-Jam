using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TerminalManager : Singleton<TerminalManager>
{
    [Header("Instantiating")]
    [SerializeField] GameObject directoryLine;
    [SerializeField] GameObject responseLine;

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

    private void OnGUI()
    {
        if (terminalInput.isFocused && terminalInput.text != "" && Input.GetKeyDown(KeyCode.Return))
        {
            string userInput = terminalInput.text;

            terminalInput.text = "";

            Vector2 commandLineContainerSize = commandLineContainer.sizeDelta;

            commandLineContainer.sizeDelta = new Vector2(commandLineContainerSize.x, commandLineContainerSize.y + heightIncrease);

            GameObject message = Instantiate(directoryLine, commandLineContainer);
            message.transform.SetSiblingIndex(commandLineContainer.transform.childCount - 1);
            message.GetComponentInChildren<TMP_Text>().text = userInput;

            int lines = DisplayInterpretation(interpreter.Interpret(userInput));
            ScrollToBottom(lines);

            userInputLine.transform.SetAsLastSibling();

            terminalInput.ActivateInputField();
            terminalInput.Select();
        }
    }

    int DisplayInterpretation(Interpreter.Command interpretedCommand)
    {
        for (int i = 0; i < interpretedCommand.GetResponse().Count; i++)
        {
            GameObject response = Instantiate(responseLine, commandLineContainer);

            response.transform.SetAsLastSibling();

            Vector2 listSize = commandLineContainer.sizeDelta;
            commandLineContainer.sizeDelta = new Vector2(listSize.x, listSize.y + heightIncrease);

            response.GetComponentInChildren<TMP_Text>().text = interpretedCommand.GetResponse()[i];
        }
        
        return interpretedCommand.GetResponse().Count;
    }

    void ScrollToBottom(int lines)
    {
        if (lines > 4)
        {
            scrollRect.velocity = new Vector2(0, 450);
        }
        else
        {
            if (scrollType == ScrollType.JumpTo)
                scrollRect.verticalNormalizedPosition = 0;
            else if (scrollType == ScrollType.SmoothScroll)
                scrollRect.velocity = new Vector2(0, smoothScrollSpeed);
        }
    }
}
