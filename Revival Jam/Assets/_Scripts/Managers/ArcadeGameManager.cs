using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class ArcadeGameManager : Singleton<ArcadeGameManager>
{
    public ArcadeState CurrentState { get; private set; }
    GameObject glitchedWorld;

    private void Update()
    {
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
                LoadLevel(levelToLoad);
                break;

            case ArcadeState.RestartLevel:
                ReloadLevel();
                break;

            case ArcadeState.Lose:
                Lose();
                break;
        }
    }


    //Yes, find in coroutine is bad for performance, but it's hard to think of a better way with tile maps
    IEnumerator FindTileMaps()
    {
        while (Player.Instance == null)
            yield return null;

        GameObject glitchedWorld = null;

        while (glitchedWorld == null)
        {
            yield return null;

            glitchedWorld = GameObject.Find("GlitchedWorld");
        }

        Debug.Log("Found glitched world!");

        this.glitchedWorld = glitchedWorld;
    }

    void LoadLevel(int levelToLoad)
    {
        SceneLoader.Instance.StartLevelLoad(levelToLoad);

        StartCoroutine(FindTileMaps());
    }

    void ReloadLevel()
    {
        UpdateArcadeState(ArcadeState.StartLevel, SceneLoader.Instance.LevelNumber);
    }

    //Add game over screen, have game over screen enter into the restart state
    void Lose()
    {
        ReloadLevel();
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