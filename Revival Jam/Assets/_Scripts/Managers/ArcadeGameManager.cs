using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static GameManager;

public class ArcadeGameManager : Singleton<ArcadeGameManager>
{
    public static event Action<ArcadeState> AfterArcadeStateChange;
    public ArcadeState CurrentState { get; private set; }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UpdateArcadeState(ArcadeState.StartLevel, 1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UpdateArcadeState(ArcadeState.StartLevel, 2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UpdateArcadeState(ArcadeState.StartLevel, 3);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            UpdateArcadeState(ArcadeState.RestartLevel);
        }
    }

    
    public void UpdateArcadeState(ArcadeState newState, int levelToLoad = -1)
    {
        CurrentState = newState;

        switch (newState)
        {
            case ArcadeState.StartLevel:
                LoadLevel(levelToLoad);
                break;

            case ArcadeState.RestartLevel:
                ReloadLevel();
                break;

            case ArcadeState.Lose:
                Lose();
                break;
            case ArcadeState.Win:
                LoadNextLevel();
                break;

        }

        AfterArcadeStateChange?.Invoke(newState);
    }

    //Loads the given level
    void LoadLevel(int levelToLoad)
    {
        SceneLoader.Instance.StartLevelLoad(levelToLoad);
    }

    //Starts the current level over from the beginning
    void ReloadLevel()
    {
        UpdateArcadeState(ArcadeState.StartLevel, SceneLoader.Instance.LevelNumber);
    }

    void Lose()
    {
        ReloadLevel();
    }

    void LoadNextLevel()
    {
        if (SceneLoader.DoesLevelExist(SceneLoader.Instance.LevelNumber + 1))
        {
            UpdateArcadeState(ArcadeState.StartLevel, SceneLoader.Instance.LevelNumber + 1);
            Debug.Log("Loading next level");
        }
    }

    [Serializable]
    public enum ArcadeState
    {
        StartLevel,
        Lose,
        RestartLevel,
        Win,
    } 
}