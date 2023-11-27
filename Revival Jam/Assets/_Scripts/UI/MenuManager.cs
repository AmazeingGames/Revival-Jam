using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static GameManager;

public class MenuManager : Singleton<MenuManager>
{
    [Header("Main Menu")]
    [SerializeField] Canvas mainMenu;
    [SerializeField] Camera menuCamera;

    [Header("Pause")]
    [SerializeField] Canvas pauseMenu;
    [SerializeField] GameObject controlsPanel;
    [SerializeField] GameObject settings;

    public enum MenuState { Null, MainMenu, LevelSelectMenu, GameStart, Pause, Settings, GameResume }

    public MenuState CurrentState { get; private set; }

    public static event Action<MenuState> OnMenuStateChange;

    void OnEnable()
    {
        AfterStateChange += HandleGameStateChange;
    }

    void OnDisable()
    {
        AfterStateChange -= HandleGameStateChange;
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

    //Pressing escape will either pause and unpause the game
    void OnEscape()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
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

    void OnMainMenuEnter()
    {
        mainMenu.gameObject.SetActive(true);
        
        if (!menuCamera.isActiveAndEnabled)
        {
            Debug.LogWarning("MenuCamera was not active. Setting Cam active");
            menuCamera.gameObject.SetActive(true);
        }
    }

    void OnLevelSelectMenuEnter()
    {
        mainMenu.gameObject.SetActive(false);
    }

    void OnGameStart()
    {
        mainMenu.gameObject.SetActive(false);
        menuCamera.gameObject.SetActive(false);
    }

    void OnGamePause()
    {
        controlsPanel.SetActive(true);
        pauseMenu.gameObject.SetActive(true);

        settings.SetActive(false);
    }

    void OnGameResume()
    {
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
