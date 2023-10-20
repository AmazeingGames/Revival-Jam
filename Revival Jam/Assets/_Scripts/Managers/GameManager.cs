using System;
using System.Collections;
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
            UpdateGameState(GameState.StartLevel, 1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UpdateGameState(GameState.StartLevel, 2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UpdateGameState(GameState.StartLevel, 3);
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
                ReadyGameScenes();
                break;

            case GameState.StartLevel:
                LoadArcadeLevel(levelToLoad);
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

    void LoadArcadeLevel(int levelToLoad)
    {
        SceneLoader.Instance.StartLevelLoad(levelToLoad);

        SceneLoader.Instance.UnloadScene("_ArcadeMenu");
    }

    void ReadyGameScenes()
    {
        SceneLoader.Instance.LoadScene("RealWorld");
        SceneLoader.Instance.LoadScene("Circuits");

        StartCoroutine(LoadArcadeMenu());
    }

    IEnumerator LoadArcadeMenu()
    {
        yield return new WaitForSeconds(.1f);
        SceneLoader.Instance.LoadScene("_ArcadeMenu");
    }

    void ReloadLevel()
    {
        UpdateGameState(GameState.StartLevel, SceneLoader.Instance.LevelNumber);
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
        StartLevel,
        RestartLevel,
        Loading,
        Win,
        Lose,
    }
}