using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static ReceptacleObject;
using static Wire;

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

    public CircleCollider2D Collider { get; private set; }

    public bool IsGrounded { get; private set; }
    public string LastGroundLayer { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        Collider = GetComponent<CircleCollider2D>();

        StartCoroutine(GroundCheck());
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

        if (racyastHit)
        {
            int layerNumber = racyastHit.transform.gameObject.layer;
            
            string layerName = LayerMask.LayerToName(layerNumber);

            LastGroundLayer = layerName ;//LayerMask.NameToLayer(layerName);
        }

        return racyastHit;
    }
}
