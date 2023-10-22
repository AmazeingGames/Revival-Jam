using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using static EnemyAnimator;
using static EnemyAnimator.EnemyAnimation;

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
    bool isFlipRunning = false;

    EnemyAnimator enemyAnimator;
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        enemyAnimator = GetComponent<EnemyAnimator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (flipTimer < 0 && !isFlipRunning)
        {
            if (wallDetector._ShouldFlip || groundDetector._ShouldFlip)
                StartCoroutine(Flip(!groundDetector._ShouldFlip));
        }

        MoveEnemy();

        UpdateTimers();
    }

    IEnumerator Flip(bool flipImmediate)
    {
        isFlipRunning = true;

        float wait = flipImmediate ? 0 : movementPauseTimer;

        flipTimer = flipTimerLength;
        movementPauseTimer = flipMovementPauseLength;

        //Debug.Log($"flip immediate : {flipImmediate} | wait : {wait}");

        yield return new WaitForSeconds(Mathf.Abs(wait));

        Vector3 newScale = transform.localScale;

        newScale.x *= -1;

        transform.localScale = newScale;

        rigidbody.velocity = Vector3.zero;

        CheckDebug("Flipped");

        isFlipRunning = false;
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
            enemyAnimator.PlayAnimation(Idle);
            rigidbody.velocity = Vector3.zero;
            return;
        }

        enemyAnimator.PlayAnimation(EnemyAnimation.Walk);
        rigidbody.velocity = new Vector3(constantSpeed * (transform.localScale.x / Mathf.Abs(transform.localScale.x)), rigidbody.velocity.y);
    }

    void CheckDebug(string text)
    {
        if (showDebug)
            Debug.Log(text);
    }
}
