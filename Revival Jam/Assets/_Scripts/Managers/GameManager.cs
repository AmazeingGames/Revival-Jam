using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Nice, easy to understand enum-based game manager. For larger and more complex games, look into
/// state machines. But this will serve just fine for most games.
/// </summary>
public class GameManager : Singleton<GameManager>
{
    [SerializeField] bool loadStartingScenes = true;

    public static event Action<GameState> BeforeStateChange;
    public static event Action<GameState> AfterStateChange;

    private void Awake()
    {
        base.Awake();

        #if DEBUG
        if (!loadStartingScenes)
            return;
        #endif

        ReadyUI();
    }

    public GameState State { get; private set; }

    public void UpdateGameState(GameState newState)
    {
        BeforeStateChange?.Invoke(newState);

        State = newState;

        switch (newState)
        {
            case GameState.StartGame:
                ReadyGameScenes();
                UnreadyMenuScenes();
                break;

            case GameState.Loading:
                break;

            case GameState.Win:
                LoadNextLevel();
                break;

            //I don't think we ever use the level start game state | Needs additional testing
            case GameState.LevelStart:
                OnLevelLoad(SceneLoader.Instance.LevelNumber + 1);
                throw new Exception("I want to see when this is called");

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

    void UnreadyMenuScenes()
    {
        SceneLoader.Instance.UnloadScene("RealWorld_BackgroundArea", true);
    }

    IEnumerator LoadArcadeScene()
    {
        yield return new WaitForSeconds(.1f);
        SceneLoader.Instance.LoadScene("_ArcadeGame");
    }

    void ReadyUI()
    {
        SceneLoader.Instance.LoadScene("RealWorld_BackgroundArea");
    }

    void LoadNextLevel()
    {
        if (SceneLoader.DoesLevelExist(SceneLoader.Instance.LevelNumber + 1)) UpdateGameState(GameState.LevelStart);
    }

    void OnLevelLoad(int levelnumber)
    {
        SceneLoader.Instance.LoadLevel(levelnumber);
    }

    [Serializable]
    public enum GameState
    {
        StartGame,
        Loading,
        Win,
        LevelStart,
    }
}