using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    [SerializeField] Transform raycastStart;
    [SerializeField] float raycastLength;
    [SerializeField] LayerMask enemyLayer;

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D raycast = Physics2D.Raycast(raycastStart.transform.position, GetRayDirection(), raycastLength, enemyLayer);

        if (raycast)
        {
            raycast.rigidbody.gameObject.SetActive(false);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawRay(raycastStart.transform.position, raycastLength * GetRayDirection());
    }

    Vector3 GetRayDirection()
    {
        if (Player.Instance == null)
            return Vector3.right;

        return Player.Instance.transform.localScale.x < 0 ? Vector3.left : Vector3.right;
    }
}
