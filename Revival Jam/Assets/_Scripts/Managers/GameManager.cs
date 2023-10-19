using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Nice, easy to understand enum-based game manager. For larger and more complex games, look into
/// state machines. But this will serve just fine for most games.
/// </summary>
public class GameManager : StaticInstance<GameManager>
{
    public static event Action<GameState> OnBeforeStateChanged;
    public static event Action<GameState> OnAfterStateChanged;

    public GameState State { get; private set; }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UpdateGameState(GameState.StartArcadeLevel, 1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UpdateGameState(GameState.StartArcadeLevel, 2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UpdateGameState(GameState.StartArcadeLevel, 3);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            UpdateGameState(GameState.RestartLevel);
        }
    }

    public void UpdateGameState(GameState newState, int levelToLoad = -1)
    {
        OnBeforeStateChanged?.Invoke(newState);

        State = newState;

        switch (newState)
        {
            case GameState.StartGame:
                break;

            case GameState.StartArcadeLevel:
                SceneLoader.Instance.StartLevelLoad(levelToLoad);
                break;

            case GameState.Loading:
                break;

            case GameState.RestartLevel:
                ReloadLevel();
                break;

            case GameState.Win:
                break;

            case GameState.Lose:
                OnLose();
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnAfterStateChanged?.Invoke(newState);

        Debug.Log($"New state: {newState}");
    }

    void ReloadLevel()
    {
        UpdateGameState(GameState.StartArcadeLevel, SceneLoader.Instance.LevelNumber);
    }

    void OnLose()
    {
        //Add game over screen, have game over screen enter into the restart state
        ReloadLevel();
    }

    [Serializable]
    public enum GameState
    {
        StartGame,
        StartArcadeLevel,
        RestartLevel,
        Loading,
        Win,
        Lose,
    }
}