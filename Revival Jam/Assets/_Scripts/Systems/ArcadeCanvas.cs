using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ArcadeGameManager;

public class ArcadeCanvas : MonoBehaviour
{
    Canvas canvas;

    private void OnEnable()
    {
        AfterArcadeStateChange += HandleArcadeGameStateChange;
    }

    private void OnDisable()
    {
        AfterArcadeStateChange -= HandleArcadeGameStateChange;
    }

    void HandleArcadeGameStateChange(ArcadeState arcadeState)
    {
        switch (arcadeState)
        {
            case ArcadeState.StartLevel:
                Debug.Log("Started search for camera");
                StartCoroutine(FindCamera());
                break;
        }
    }

    private void Start()
    {
        canvas = GetComponent<Canvas>();
    }


    //Yes; 'find' in coroutine is bad for performance, but I dumb
    IEnumerator FindCamera()
    {
        while (Player.Instance == null)
        {
            Debug.Log("Player is null");
            yield return null;
        }

        GameObject arcadeCamera = null;

        while (arcadeCamera == null)
        {
            yield return null;

            arcadeCamera = GameObject.Find("Arcade Camera");
        }

        Debug.Log("Found arcade camera!");

        canvas.worldCamera = arcadeCamera.GetComponent<Camera>();
    }
}
