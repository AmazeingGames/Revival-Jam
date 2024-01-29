using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    [SerializeField] string levelNameConvention;

    public static event Action<AsyncOperation, bool> OnLoadStart;

    public int LevelNumber { get; private set; }
    
    string sceneToUnload = null;

    public void StartLevelLoad(int levelToLoad)
    {
        Debug.Log($"level Start : Level {levelToLoad}");

        if (levelToLoad == -1)
            throw new NotImplementedException();

        LevelNumber = levelToLoad;

        LoadLevel(levelToLoad);
    }

    public bool LoadLevel(int level)
    {
        Debug.Log("CHANGE LEVEL NAME CONVENTION");
        return LoadScene($"NewArcadeLevel_{level}", true);
        //return LoadScene($"{levelNameConvention}{level}", true);
    }

    public bool LoadScene(string sceneName, bool isLevel = false)
    {
        //loadingCanvas.gameObject.SetActiveCursor(true);

        if (!DoesSceneExist(sceneName))
        {
            //loadingCanvas.gameObject.SetActiveCursor(false);

            Debug.Log("Scene does not exist");
            return false;
        }

        UnloadScene(sceneToUnload);

        if (isLevel)
            sceneToUnload = sceneName;

        if (!GameManager.Instance)
            Debug.Log("No Game Manager Instance");

        GameManager.Instance.UpdateGameState(GameState.Loading);

        //Debug.Log($"Loading Scene: {sceneName}");

        AsyncOperation load = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

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

    public static bool DoesLevelExist(int levelnumber)
    {
        if (Instance == null)
            return false;

        return DoesSceneExist($"{Instance.levelNameConvention}{levelnumber}");
    }

    public bool UnloadScene(string sceneName, bool bypass = false)
    {
        if (!DoesSceneExist(sceneName))
            return false;

        if (sceneToUnload == null && !bypass)
            return false;

        Debug.Log($"Unloaded scene : {sceneName}");

        SceneManager.UnloadSceneAsync(sceneName);

        return true;
    }
}

