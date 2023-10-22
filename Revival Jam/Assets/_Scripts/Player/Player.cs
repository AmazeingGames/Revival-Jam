using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static ReceptacleObject;
using static Wire;
using static AudioManager;
using static AudioManager.EventSounds;

//Could put this in a field separate completely from the player, that the player is able to reference for their controls.
//That way information could be consistent with the game, regardless of if the arcade game is running or not
public class Player : Singleton<Player>
{
    [Header("Ground Check")]
    [SerializeField] GameObject leftGroundRaycastStart;
    [SerializeField] GameObject rightGroundRaycastStart;

    [SerializeField] float groundRaycastLength;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] bool showGroundCheckDebug;

    public float LastGroundedTime { get; private set; }
    public CircleCollider2D Collider { get; private set; }

    public bool IsGrounded { get; private set; }
    public string LastGroundLayer { get; private set; } = "";

    public float JoustTimer { get; private set; }
    bool incrementJoust = false;

    // Start is called before the first frame update
    void Start()
    {
        Collider = GetComponent<CircleCollider2D>();

        StartCoroutine(GroundCheck());
    }

    private void Update()
    {
        UpdateGrounededTimer();
        UpdateJoustTimer();
    }

    void UpdateGrounededTimer()
    {
        if (IsGrounded)
        {
            LastGroundedTime = 0;
            return;
        }

        LastGroundedTime += Time.deltaTime;
    }

    IEnumerator GroundCheck()
    {
        while (true)
        {
            RaycastHit2D leftRaycast = GroundRaycast(leftGroundRaycastStart);
            RaycastHit2D rightRaycast = GroundRaycast(rightGroundRaycastStart);

            IsGrounded = leftRaycast || rightRaycast;

            CheckDebug($"Is grounded : {IsGrounded}", showGroundCheckDebug);

            yield return null;
        }
    }

    void CheckDebug(string msg, bool shouldDebug)
    {
        if (shouldDebug)
            Debug.Log(msg);
    }

    void OnDrawGizmos()
    {
        Vector3 rayDirection = new(0, -groundRaycastLength, 0);
        Gizmos.DrawRay(leftGroundRaycastStart.transform.position, rayDirection);
        Gizmos.DrawRay(rightGroundRaycastStart.transform.position, rayDirection);
        //Vector3 rayDirection = new(0, 0, 0);
    }

    RaycastHit2D GroundRaycast(GameObject rayCastStart)
    {
        RaycastHit2D racyastHit = Physics2D.Raycast(rayCastStart.transform.position, Vector3.down, groundRaycastLength, groundLayer);

        Debug.Log($"Raycast hit : {(bool)racyastHit}");
        if (racyastHit)
        {

            int layerNumber = racyastHit.transform.gameObject.layer;

            string layerName = LayerMask.LayerToName(layerNumber);

            LastGroundLayer = layerName;//LayerMask.NameToLayer(layerName);
        }

        return racyastHit;
    }

    public EventSounds GetWalkSound()
    {
        return LastGroundLayer switch
        {
            "Arcade Stone" => PlayerStoneFootsteps,
            "Arcade Grass" => PlayerGrassFootsteps,
            _              => EventSounds.Null,
        };
    }

    public void ShouldIncrementJoustTimer(bool increment)
    {
        incrementJoust = increment;
    }

    public void ResetJoustTimer()
    {
        JoustTimer = 0;
    }

    void UpdateJoustTimer()
    {
        if (incrementJoust)
            JoustTimer += Time.deltaTime;
        //else
            //set 0 (?)
    }
}
