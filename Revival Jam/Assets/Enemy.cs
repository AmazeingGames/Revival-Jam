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

    [Header("Movement")]
    [SerializeField] float constantSpeed;
    [SerializeField] float flipMovementPauseLength;

    [Header("Debug")]
    [SerializeField] bool showDebug;

    Rigidbody2D rigidbody;

    float flipTimer;
    float movementPauseTimer;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

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
        }

        MoveEnemy();

        UpdateTimers();
    }

    void Flip()
    {
        Vector3 newScale = transform.localScale;

        newScale.x *= -1;

        transform.localScale = newScale;

        rigidbody.velocity = Vector3.zero;

        flipTimer = flipTimerLength;
        movementPauseTimer = flipMovementPauseLength;

        CheckDebug("Flipped");
    }

    void UpdateTimers()
    {
        flipTimer -= Time.deltaTime;
        movementPauseTimer -= Time.deltaTime;

        if (flipTimer < 0)
            flipTimer = -1;

        if (movementPauseTimer < 0)
            movementPauseTimer = -1;
    }

    void MoveEnemy()
    {
        if (movementPauseTimer > 0)
        {
            rigidbody.velocity = Vector3.zero;
            return;
        }

        rigidbody.velocity = new Vector3(constantSpeed * (transform.localScale.x / Mathf.Abs(transform.localScale.x)), rigidbody.velocity.y);
    }

    void CheckDebug(string text)
    {
        if (showDebug)
            Debug.Log(text);
    }
}
