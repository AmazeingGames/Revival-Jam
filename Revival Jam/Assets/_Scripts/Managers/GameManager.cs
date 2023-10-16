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

    [SerializeField] string LevelNameConvention;

    public static event Action<AsyncOperation, bool> OnLoadStart;


    string sceneToUnload = null;

    public GameState State { get; private set; }

    public int LevelNumberCurrent { get; private set; }

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
    }

    public void UpdateGameState(GameState newState, int levelToLoad = -1)
    {
        OnBeforeStateChanged?.Invoke(newState);

        State = newState;

        switch (newState)
        {
            case GameState.StartLevel:
                StartLevelLoad(levelToLoad);
                break;
            case GameState.Loading:
                    break;
            case GameState.Win:
                break;
            case GameState.Lose:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnAfterStateChanged?.Invoke(newState);

        Debug.Log($"New state: {newState}");
    }

    void StartLevelLoad(int levelToLoad)
    {
        Debug.Log("level Start");

        if (levelToLoad == -1)
            throw new NotImplementedException();

        LevelNumberCurrent = levelToLoad;

        LoadLevel(levelToLoad);
    }

    bool LoadLevel(int level) => LoadScene($"{LevelNameConvention}{level}", true);

    bool LoadScene(string sceneName, bool isLevel = false)
    {
        //loadingCanvas.gameObject.SetActive(true);

        if (!DoesSceneExist(sceneName))
        {
            //loadingCanvas.gameObject.SetActive(false);

            Debug.Log("Scene does not exist");
            return false;
        }

        UnloadScene(sceneToUnload);

        sceneToUnload = sceneName;

        UpdateGameState(GameState.Loading);

        Debug.Log($"Loading Scene: {sceneName}");

        var load = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        OnLoadStart?.Invoke(load, isLevel);

        return true;
    }

    public static bool DoesSceneExist(string sceneName)
    {
        int buildIndex = SceneUtility.GetBuildIndexByScenePath(sceneName);

        if (buildIndex == -1)
        {
            return false;
        }
        return true;
    }

    bool UnloadScene(string sceneName)
    {
        if (!DoesSceneExist(sceneName))
            return false;

        if (sceneToUnload == null)
            return false;

        Debug.Log($"Unloaded scene : {sceneName}");

        SceneManager.UnloadSceneAsync(sceneName);

        return true;
    }
}

[Serializable]
public enum GameState
{
    StartLevel,
    Loading,
    Win,
    Lose,
}