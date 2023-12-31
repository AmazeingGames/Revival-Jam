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
        State = newState;

        switch (newState)
        {
            case GameState.StartGame:
                ReadyGameScenes();
                UnreadyMenuScenes();
                break;

            case GameState.Loading:
                break;

            case GameState.EndGame:
                TriggerGameEndScreen();
                UnloadGameScenes();
                AudioManager.TriggerAudioClip(AudioManager.EventSounds.Ending, ArcadeSoundEmitter.Transform);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        AfterStateChange?.Invoke(newState);

        //Debug.Log($"New state: {newState}");
    }

    void TriggerGameEndScreen()
    {
        MenuManager.Instance.UpdateState(MenuManager.MenuState.GameEnd);
    }

    void UnloadGameScenes()
    {
        SceneLoader.Instance.UnloadScene("RealWorld", true);
        SceneLoader.Instance.UnloadScene("Hotbar", true);
        SceneLoader.Instance.UnloadScene("Circuits", true);
        SceneLoader.Instance.UnloadScene("_ArcadeGame", true);
    }

    void ReadyGameScenes()
    {
        SceneLoader.Instance.LoadScene("RealWorld");
        SceneLoader.Instance.LoadScene("Hotbar");
        SceneLoader.Instance.LoadScene("Circuits");
        
        StartCoroutine(WiringManager.SetWiringCabinet(false));
        
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

    void OnLevelLoad(int levelnumber)
    {
        SceneLoader.Instance.LoadLevel(levelnumber);
    }

    [Serializable]
    public enum GameState
    {
        Null,
        StartGame,
        Loading,
        EndGame
    }
}