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
    public static event Action<GameState> BeforeStateChange;
    public static event Action<GameState> AfterStateChange;

    public GameState State { get; private set; }

    public void UpdateGameState(GameState newState)
    {
        BeforeStateChange?.Invoke(newState);

        State = newState;

        switch (newState)
        {
            case GameState.StartGame:
                ReadyGameScenes();
                break;

            case GameState.Loading:
                break;

            case GameState.Win:
                break;   

            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        AfterStateChange?.Invoke(newState);

        Debug.Log($"New state: {newState}");
    }

    void ReadyGameScenes()
    {
        SceneLoader.Instance.LoadScene("RealWorld");
        SceneLoader.Instance.LoadScene("Circuits");

        StartCoroutine(LoadArcadeScene());
    }

    IEnumerator LoadArcadeScene()
    {
        yield return new WaitForSeconds(.1f);
        SceneLoader.Instance.LoadScene("_ArcadeGame");
    }

    [Serializable]
    public enum GameState
    {
        StartGame,
        Loading,
        Win,
    }
}