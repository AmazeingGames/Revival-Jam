using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//This script seems pretty complicated for what it does
//Maybe we can simplify it? Could be handled by the menu manager class, but maybe its better as its own class
public class ButtonPromptInVolume : MonoBehaviour
{
    public GameObject gameObjectToActivate;
    public TMPro.TMP_Text promptText;
    public Collider triggerVolume;
    public string TextToSay;
    public KeyCode KeyCodeToCheckFor;
    public bool killSelf;
    //public bool nextPrompt;

    private bool waitingForInput = true;

    private void Update()
    {
        if (waitingForInput)
        {
            //if (triggerVolume.bounds.Contains(transform.position))
            //{
            //    promptText.GetComponentInParent<GameObject>().SetActiveCursor(true);
            //    promptText.text = TextToSay;
            //    CheckForInput();
            //}
            //else
            //{
            //    promptText.text = string.Empty;
            //}
            CheckForInput();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Player entered trigger!");
        gameObjectToActivate.SetActive(true);
        promptText.text = TextToSay;

    }

    private void CheckForInput()
    {
        if (Input.GetKeyDown(KeyCodeToCheckFor))
        {
            waitingForInput = false;
            //if (!nextPrompt)
            gameObjectToActivate.SetActive(false);
            if (killSelf)
                this.gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        gameObjectToActivate.SetActive(false);
        if (killSelf)
            this.gameObject.SetActive(false);

    }
}
