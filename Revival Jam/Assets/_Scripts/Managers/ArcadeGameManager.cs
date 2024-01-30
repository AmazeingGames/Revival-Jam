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

            //Maybe change these two to reset the level, instead of changing the arcade state
            //This would improve performance to do to the potentially intensive tasks called whenever the level starts, that wouldn't need to be performed more than once
            case ArcadeState.RestartLevel:
                UpdateArcadeState(ArcadeState.StartLevel, SceneLoader.Instance.LevelNumber);
                break;
            case ArcadeState.Lose:
                UpdateArcadeState(ArcadeState.StartLevel, SceneLoader.Instance.LevelNumber);
                break;

            case ArcadeState.Win:
                //This could instead update the ArcadeGameState
                SceneLoader.Instance.StartLevelLoad(SceneLoader.Instance.LevelNumber + 1);
                break;
        }

        AfterArcadeStateChange?.Invoke(newState);
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