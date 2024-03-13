using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static GameManager;

public class ArcadeMenuManager : Singleton<ArcadeMenuManager>
{
    [SerializeField] GameObject ScreenContentCamera;

    [SerializeField] GameObject mainMenuScreen;
    [SerializeField] GameObject playButton;


    [SerializeField] GameObject selectSaveMenu;
    [SerializeField] GameObject startingSaveButton;

    [SerializeField] GameObject eventSystem;
    [SerializeField] GameObject mainCamera;

    public enum ArcadeMenuState { MainMenu, SelectSave, GameRunning }

    public ArcadeMenuState CurrentState { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        var isGameManagerNull = GameManager.Instance == null;

        eventSystem.SetActive(isGameManagerNull);
        mainCamera.SetActive(isGameManagerNull);

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
        ScreenContentCamera.SetActive(true);
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

        EventSystem.current.SetSelectedGameObject(null);

        mainMenuScreen.SetActive(false);
        selectSaveMenu.SetActive(false);
        ScreenContentCamera.SetActive(false);
    }
}
