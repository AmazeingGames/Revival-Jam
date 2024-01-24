using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static GameManager;
using static PlayerFocus;

public class MenuManager : Singleton<MenuManager>
{
    [Header("Main Menu")]
    [SerializeField] Canvas mainMenu;
    [SerializeField] GameObject menuButtons;
    [SerializeField] GameObject title;

    [Header("Pause")]
    [SerializeField] Canvas pauseMenu;
    [SerializeField] GameObject controlsPanel;
    [SerializeField] GameObject settings;

    [Header("Game End")]
    [SerializeField] Canvas endScreen;
    [SerializeField] float timeUntilCredits = 4f;
    [SerializeField] Dialogue finalDialogue;

    [Header("Cameras")]
    [SerializeField] Camera menuCamera;

    readonly List<DisableOnMenu> objectsToDisable = new();

    public bool IsInMenu => menuCamera.isActiveAndEnabled;

    public enum MenuState { Null, MainMenu, GameStart, Pause, Settings, GameResume, GameEnd }

    public MenuState CurrentState { get; private set; }
    public MenuState PreviousState { get; private set; } = MenuState.Null;

    public static event Action<MenuState> OnMenuStateChange;

    void OnEnable()
    {
        AfterStateChange += HandleGameStateChange;
        DisableOnMenu.AddToDisable += HandleAddToDisable;
    }

    void OnDisable()
    {
        AfterStateChange -= HandleGameStateChange;
        DisableOnMenu.AddToDisable -= HandleAddToDisable;
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateState(MenuState.MainMenu);
    }

    private void Update()
    {
        OnEscape();

//Test game end
#if DEBUG
        if (Input.GetKeyDown(KeyCode.L))
        {
            UpdateState(MenuState.GameEnd);
        }
#endif
    }

    void HandleAddToDisable(DisableOnMenu sender)
    {
        if (sender != null)
        {
            objectsToDisable.Add(sender);
            Debug.Log($"Added {sender} to disable list");
        }
    }

    //Pressing escape will either pause and unpause the game
    void OnEscape()
    {
        var keyPress = KeyCode.Escape;

#if DEBUG 
        keyPress = KeyCode.P;
#endif

        if (Input.GetKeyDown(keyPress))
        {
            switch (CurrentState)
            {
                case MenuState.Pause:
                    UpdateState(MenuState.GameResume);
                    break;

                case MenuState.GameStart:
                case MenuState.GameResume:
                    UpdateState(MenuState.Pause);
                    break;
            }
        }
    }

    public void UpdateState(MenuState newState)
    {
        PreviousState = CurrentState;
        CurrentState = newState;

        switch (newState)
        {
            case MenuState.MainMenu:
                OnMainMenuEnter();
                break;

            case MenuState.GameStart:
                OnGameStart();
                break;

            case MenuState.Pause:
                OnGamePause();
                break;

            case MenuState.Settings:
                OnSettingsEnter();
                break;

            case MenuState.GameResume:
                OnGameResume();
                break;

            case MenuState.GameEnd:
                OnGameEnd();
                break;

            default:
                Debug.Log("Unknown Menu State");
                break;
        }

        OnMenuStateChange.Invoke(newState);
    }

    //Not sure if this is necessary at the moment, but I will think about it
    //When exiting a menu, we'll need to always disable certain things.
    //Instead of handling this in every other 'entry' function, it may be better to handle it in a single area
    //The only downside this would have would be if the gamee is not set up properly the first time, though we can make a new function to prepare the game to be ready
    void AddToDisable(params GameObject[] objects)
    {

    }

    void OnGameEnd()
    {
        SetMenuCamera(true);

        endScreen.gameObject.SetActive(true);

        StartCoroutine(PlayCredits());
    }


    IEnumerator PlayCredits()
    {
        yield return new WaitForSeconds(timeUntilCredits);
        DialogueManager.Instance.StartDialogue(finalDialogue, DialogueManager.DialogueType.Meta);
    }

    void SetMenuCamera(bool setActive)
    {
        menuCamera.gameObject.SetActive(setActive);

        foreach (var disable in objectsToDisable)
        {
            if (disable != null)
                disable.gameObject.SetActive(!setActive);
        }
    }

    void OnMainMenuEnter()
    {
        mainMenu.gameObject.SetActive(true);
        endScreen.gameObject.SetActive(false);
        menuButtons.SetActive(true);
        title.SetActive(true);

        settings.SetActive(false);
        
        if (!menuCamera.isActiveAndEnabled)
        {
            Debug.LogWarning("MenuCamera was not active. Setting Cam active");
            SetMenuCamera(true);
        }
    }

    void OnGameStart()
    {
        SetMenuCamera(false);

        mainMenu.gameObject.SetActive(false);
    }

    void OnGamePause()
    {
        SetMenuCamera(true);

        controlsPanel.SetActive(true);
        pauseMenu.gameObject.SetActive(true);

        menuButtons.SetActive(false);
        settings.SetActive(false);
    }

    void OnGameResume()
    {
        SetMenuCamera(false);

        pauseMenu.gameObject.SetActive(false);
    }

    void OnSettingsEnter()
    {
        menuButtons.SetActive(false);
        controlsPanel.SetActive(false);
        title.SetActive(false);

        settings.SetActive(true);
    }

    //This and the play button kind of do the same thing.
    void HandleGameStateChange(GameState gameState)
    {
        if (gameState == GameState.StartGame)
            OnGameStart();
    }
}
