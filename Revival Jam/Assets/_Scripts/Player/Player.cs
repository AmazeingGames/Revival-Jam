using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Ground Check")]
    [SerializeField] GameObject groundRaycastStart;
    [SerializeField] float groundRaycastLength;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] bool showGroundCheckDebug;

    public CircleCollider2D Collider { get; private set; }

    public bool IsGrounded { get; private set; }

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
            RaycastHit2D raycast = Physics2D.Raycast(groundRaycastStart.transform.position, Vector3.down, groundRaycastLength, groundLayer);

            IsGrounded = raycast;

            CheckDebug($"Is grounded : {(bool)IsGrounded}", showGroundCheckDebug);

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
        Gizmos.DrawRay(groundRaycastStart.transform.position, rayDirection);    
    }
}
