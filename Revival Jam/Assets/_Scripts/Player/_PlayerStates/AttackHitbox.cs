using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    [SerializeField] Transform raycastStart;
    [SerializeField] float raycastLength;
    [SerializeField] LayerMask enemyLayer;

    public static event Action<GameObject, bool> Hit;

    GameObject hitObject;

    readonly List<GameObject> hitObjects = new List<GameObject>();

    private void Start()
    {
        hitObject = null;
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        foreach (GameObject obj in hitObjects)
            Hit?.Invoke(obj, false);

        hitObjects.Clear();
        Debug.Log("Cleared hit objects");
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D raycast = Physics2D.Raycast(raycastStart.transform.position, GetRayDirection(), raycastLength, enemyLayer);

        if (raycast)
        {
            hitObject = raycast.collider.gameObject;
            OnHit();
        }
    }

    void OnHit()
    {
        if (hitObjects.Contains(hitObject))
            return;

        Debug.Log("deal damage");

        Hit?.Invoke(hitObject, true);
        hitObjects.Add(hitObject);
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
