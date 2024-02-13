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


    private void OnGUI()
    {
        if (terminalInput.isFocused && terminalInput.text != "" && Input.GetKeyDown(KeyCode.Return))
        {
            string userInput = terminalInput.text;

            terminalInput.text = "";

            Vector2 commandLineContainerSize = commandLineContainer.sizeDelta;

            commandLineContainer.sizeDelta = new Vector2(commandLineContainerSize.x, commandLineContainerSize.y + 35f);

            GameObject message = Instantiate(directoryLine, commandLineContainer);

            message.transform.SetSiblingIndex(commandLineContainer.transform.childCount - 1);

            message.GetComponentInChildren<TMP_Text>().text = userInput;

            userInputLine.transform.SetAsLastSibling();

            terminalInput.ActivateInputField();
            terminalInput.Select();
        }
    }
}
