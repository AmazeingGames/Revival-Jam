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
#if DEBUG
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
#endif
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
                SceneLoader.Instance.StartLevelLoad(levelToLoad);
                break;

            case ArcadeState.RestartLevel:
                ReloadLevel();
                break;

            case ArcadeState.Lose:
                Lose();
                break;

            case ArcadeState.Win:
                SceneLoader.Instance.StartLevelLoad(SceneLoader.Instance.LevelNumber + 1);
                break;
        }

        AfterArcadeStateChange?.Invoke(newState);
    }

    //Starts the current level over from the beginning
    void ReloadLevel()
    {
        UpdateArcadeState(ArcadeState.StartLevel, SceneLoader.Instance.LevelNumber);
    }

    //Restarts the current level
    //To Do: Create Game Over Menu
    void Lose()
    {
        ReloadLevel();
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