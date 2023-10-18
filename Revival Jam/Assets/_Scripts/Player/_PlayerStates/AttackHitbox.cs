using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    [SerializeField] Transform raycastStart;
    [SerializeField] float raycastLength;
    [SerializeField] LayerMask enemyLayer;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D raycast = Physics2D.Raycast(raycastStart.transform.position, Vector3.right, raycastLength, enemyLayer);



        if (raycast)
        {
            raycast.rigidbody.gameObject.SetActive(false);
        }
    }

    void OnDrawGizmos()
    {
        Vector3 rayDirection = new (-raycastLength, 0, 0);
        Gizmos.DrawRay(raycastStart.transform.position, rayDirection);
    }
}
