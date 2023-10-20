using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class MainMenu : Singleton<MainMenu>
{
    [SerializeField] Canvas mainMenu;

    public enum MenuState { MainMenu, LevelSelectMenu, GameStart }

    public MenuState CurrentState { get; private set; }

    void OnEnable()
    {
        MainMenuBackButton.OnBack += HandleOnBack;
        AfterStateChange += HandleGameStateChange;
    }

    void OnDisable()
    {
        MainMenuBackButton.OnBack -= HandleOnBack;
        AfterStateChange -= HandleGameStateChange;
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateState(MenuState.MainMenu);
    }

    void HandleOnBack()
    {
        Debug.Log("Pressed back");

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
        }
    }

    void OnMainMenuEnter()
    {
        mainMenu.gameObject.SetActive(true);
        //levelSelect.gameObject.SetActive(false);
    }

    void OnLevelSelectMenuEnter()
    {
        mainMenu.gameObject.SetActive(false);
        //levelSelect.gameObject.SetActive(true);
    }

    void MenuExit()
    {
        //levelSelect.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(false);
    }

    void HandleGameStateChange(GameState gameState)
    {
        if (gameState == GameState.StartGame)
            MenuExit();
    }
}
