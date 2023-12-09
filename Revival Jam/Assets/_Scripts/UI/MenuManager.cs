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

    [Header("Pause")]
    [SerializeField] Canvas pauseMenu;
    [SerializeField] GameObject controlsPanel;
    [SerializeField] GameObject settings;

    [Header("Cameras")]
    [SerializeField] Camera menuCamera;

    List<DisableOnMenu> objectsToDisable = new();

    public bool IsInMenu => menuCamera.isActiveAndEnabled;

    public enum MenuState { Null, MainMenu, LevelSelectMenu, GameStart, Pause, Settings, GameResume }

    public MenuState CurrentState { get; private set; }

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
    }

    void HandleAddToDisable(DisableOnMenu sender)
    {
        if (sender != null)
        {
            objectsToDisable.Add(sender);
            //Debug.Log($"Added {sender} to disable list");
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
        CurrentState = newState;

        switch (newState)
        {
            case MenuState.MainMenu:
                OnMainMenuEnter();
                break;

            case MenuState.LevelSelectMenu:
                OnLevelSelectMenuEnter();
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

            default:
                Debug.Log("Unknown Menu State");
                break;
        }

        OnMenuStateChange.Invoke(newState);
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
        
        if (!menuCamera.isActiveAndEnabled)
        {
            Debug.LogWarning("MenuCamera was not active. Setting Cam active");
            SetMenuCamera(true);
        }
    }

    //Is there a level select screen???
    void OnLevelSelectMenuEnter()
    {
        mainMenu.gameObject.SetActive(false);
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
        settings.SetActive(false);
    }

    void OnGameResume()
    {
        SetMenuCamera(false);

        pauseMenu.gameObject.SetActive(false);
    }

    void OnSettingsEnter()
    {
        controlsPanel.SetActive(false);
        settings.SetActive(true);
    }

    //This and the play button kind of do the same thing.
    void HandleGameStateChange(GameState gameState)
    {
        if (gameState == GameState.StartGame)
            OnGameStart();
    }
}
