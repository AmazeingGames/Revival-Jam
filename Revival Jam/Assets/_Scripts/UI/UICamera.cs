using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICamera : MonoBehaviour
{
    Camera canvasCamera;
    private void Start()
    {
        canvasCamera = GetComponent<Camera>();

        canvasCamera.enabled = true;
    }

    private void OnEnable()
    {
        GameManager.OnAfterStateChanged += HandleGameStateChange;
    }

    private void OnDisable()
    {
        GameManager.OnAfterStateChanged -= HandleGameStateChange;
    }

    void HandleGameStateChange(GameManager.GameState newGameState)
    {
        switch (newGameState)
        {
            case GameManager.GameState.StartGame:
                canvasCamera.enabled = false;
                break;
        }
    }
}
