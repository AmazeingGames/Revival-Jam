using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Wire : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] float sensitivity;

    bool shouldFollowMouse = false;

    Vector3 startingMousePosition;
    private Vector2? lastMousePoint = null;

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Clicked");

        shouldFollowMouse = true;
        
        lastMousePoint = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        shouldFollowMouse = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
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

}
