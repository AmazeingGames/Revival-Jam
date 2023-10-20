using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class ArcadeMenuManager : Singleton<ArcadeMenuManager>
{
    [SerializeField] GameObject mainMenuScreen;
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
        Debug.Log("Main menu enter");
        mainMenuScreen.SetActive(true);
        selectSaveMenu.SetActive(false);
    }

    void SelectSaveEnter()
    {
        Debug.Log("Save Select enter");

        selectSaveMenu.SetActive(true);
        mainMenuScreen.SetActive(false);
    }

    void GameStart()
    {
        Debug.Log("Game Start");

        mainMenuScreen.SetActive(false);
        selectSaveMenu.SetActive(false);
    }
}
