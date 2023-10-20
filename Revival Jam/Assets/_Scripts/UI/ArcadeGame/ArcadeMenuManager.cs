using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class ArcadeMenuManager : Singleton<ArcadeMenuManager>
{
    [SerializeField] GameObject mainMenuScreenMenu;
    [SerializeField] GameObject selectSaveMenu;

    public enum ArcadeMenuState { MainMenu, SelectSave, GameRunning }

    public ArcadeMenuState CurrentState { get; private set; }

    private void OnDisable()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateArcadeMenu(ArcadeMenuState.MainMenu);
    }

    public void UpdateArcadeMenu(ArcadeMenuState newState)
    {
        switch (newState)
        {
            case ArcadeMenuState.MainMenu:
                MainMenuEnter();
                break;

            case ArcadeMenuState.SelectSave:
                SelectSaveEnter();
                break;

            case ArcadeMenuState.GameRunning:
                GameStart();
                break;
        }
    }

    void MainMenuEnter()
    {
        mainMenuScreenMenu.SetActive(true);
    }

    void SelectSaveEnter()
    {
        selectSaveMenu.SetActive(false);
    }

    void GameStart()
    {
        mainMenuScreenMenu.SetActive(false);
        selectSaveMenu.SetActive(false);
    }
}
