using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    [SerializeField] string LevelNameConvention;

    public static event Action<AsyncOperation, bool> OnLoadStart;

    public int LevelNumber { get; private set; }
    
    string sceneToUnload = null;

    public void StartLevelLoad(int levelToLoad)
    {
        Debug.Log("level Start");

        if (levelToLoad == -1)
            throw new NotImplementedException();

        LevelNumber = levelToLoad;

        LoadLevel(levelToLoad);
    }

    public bool LoadLevel(int level) => LoadScene($"{LevelNameConvention}{level}", true);

    public bool LoadScene(string sceneName, bool isLevel = false)
    {
        //loadingCanvas.gameObject.SetActive(true);

        if (!DoesSceneExist(sceneName))
        {
            //loadingCanvas.gameObject.SetActive(false);

            Debug.Log("Scene does not exist");
            return false;
        }

        UnloadScene(sceneToUnload);

        if (isLevel)
            sceneToUnload = sceneName;

        GameManager.Instance.UpdateGameState(GameState.Loading);

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

