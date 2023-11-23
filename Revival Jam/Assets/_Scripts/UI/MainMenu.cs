using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class MainMenu : Singleton<MainMenu>
{
    [SerializeField] Canvas mainMenu;
    [SerializeField] Camera menuCamera;

    public enum MenuState { MainMenu, LevelSelectMenu, GameStart, Pause }

    public MenuState CurrentState { get; private set; }

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

    public void UpdateState(MenuState newState)
    {
        switch (newState)
        {
            case MenuState.MainMenu:
                OnMainMenuEnter();
                break;

            case MenuState.LevelSelectMenu:
                OnLevelSelectMenuEnter();
                break;

            case MenuState.GameStart:
                MenuExit();
                break;

            case MenuState.Pause:
                PauseGame();
                break;
        }
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

    void MenuExit()
    {
        mainMenu.gameObject.SetActive(false);
        menuCamera.gameObject.SetActive(false);
    }

    void PauseGame()
    {
        
    }

    //This and the play button kind of do the same thing.
    void HandleGameStateChange(GameState gameState)
    {
        if (gameState == GameState.StartGame)
            MenuExit();
    }
}
