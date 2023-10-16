using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Terrain Detection")]
    [SerializeField] TerrainDetector groundDetector;
    [SerializeField] TerrainDetector wallDetector;
    [SerializeField] float flipTimerLength;

    float flipTimer;

    // Update is called once per frame
    void Update()
    {
    #if DEBUG
        if (Input.GetKeyDown(KeyCode.F))
        {
            Flip();
        }
    #endif

        if ((groundDetector._ShouldFlip || wallDetector._ShouldFlip) && flipTimer < 0)
        {
            Flip();
            flipTimer = flipTimerLength;
        }

        UpdateTimer();
    }

    void Flip()
    {
        Vector3 newScale = transform.localScale;

        newScale.x *= -1;

        transform.localScale = newScale;

        Debug.Log("Flipped");
    }

    void UpdateTimer()
    {
        flipTimer -= Time.deltaTime;
        
        if (flipTimer < 0)
            flipTimer = -1;
    }
}
