using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Wire : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] float sensitivity;
    [SerializeField] string receptacleTag;

    bool shouldFollowMouse = false;

    private Vector2? lastMousePoint = null;


    ReceptacleObject connectedReceptacle;

    List<Collider> overlappingColliders;

    void Start()
    {
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Clicked");

        shouldFollowMouse = true;

        lastMousePoint = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        shouldFollowMouse = false;

        CheckCurrentCollisions();
    }

    // Update is called once per frame
    void Update()
    {
        FollowMouse();
    }

    void FollowMouse()
    {
        if (!shouldFollowMouse)
            return;

        float differenceX = Input.mousePosition.x - lastMousePoint.Value.x;

        float differenceY = Input.mousePosition.y - lastMousePoint.Value.y;

        transform.position = new Vector3(transform.position.x + (differenceX / 188) * Time.deltaTime * sensitivity, transform.position.y + (differenceY / 188) * Time.deltaTime * sensitivity, transform.position.z);

        lastMousePoint = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    }

    void CheckCurrentCollisions()
    {
        List<Collider2D> overlappingColliders = new();

        for (int i = 0; i < overlappingColliders.Count; i++)
        {
            var collider = overlappingColliders[i];

            Debug.Log($"Loop : {collider.gameObject.name}");

            if (!collider.gameObject.CompareTag($"{receptacleTag}"))
                continue;

            collider.gameObject.SetActive(false);
        }
    }
}
