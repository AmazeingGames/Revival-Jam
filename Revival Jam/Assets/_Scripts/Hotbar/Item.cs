using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] ItemData itemData;

    [SerializeField] Image image;

    bool followMouse;
    Transform slotParent;

    private void Start()
    {
        InitializeData();
    }

    private void Update()
    {
        FollowMouse();

        InitializeData();

    }

    void FollowMouse()
    {
        if (followMouse)
            transform.position = Input.mousePosition + (Vector3)itemData.MouseFollowOffset;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Mouse Click");

        followMouse = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Mouse Up");

        followMouse = false;
    }

    public void InitializeData(ItemData itemToMatch = null)
    {
        if (itemToMatch == null && itemData == null)
            return;

        if (itemToMatch != null)
            itemData = itemToMatch;

        image.sprite = itemData.Sprite;
        
        var newScale = itemData.Scale;
        transform.localScale = new Vector3(newScale, newScale);
    }
}
