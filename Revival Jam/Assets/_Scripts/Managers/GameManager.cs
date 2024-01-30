using System;
using System.Collections;
using System.IO;
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

    //This can be moved into its own static class instead, but it's fine here.
    public static void Save<T>(T data, string pathName)
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.dataPath + $"/{pathName}.txt", json);
    }

    public static void Load<T>(string pathName, ref T data)
    {
        var fileLocation = Application.dataPath + $"/{pathName}.txt";
        if (File.Exists(fileLocation))
        {
            string saveString = File.ReadAllText(fileLocation);
            data = JsonUtility.FromJson<T>(saveString);
        }
        else
            Debug.Log($"No {pathName} save found");
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