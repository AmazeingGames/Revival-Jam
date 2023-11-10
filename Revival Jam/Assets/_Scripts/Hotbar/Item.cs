using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [field: SerializeField] public ItemData ItemData { get; private set; }

    [SerializeField] Image image;

    [SerializeField] Vector2 zeroZeroPosition;

    bool followMouse;

    private void Start()
    {
        InitializeData();
    }

    private void Update()
    {
        FollowMouse();

        //Make sure to remove this later
#if DEBUG
        InitializeData();
#endif
    }

    void FollowMouse()
    {
        if (followMouse)
            transform.position = Input.mousePosition + (Vector3)ItemData.MouseFollowOffset;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Mouse Click");

        followMouse = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Mouse Up");

        if (followMouse)
            ResetPosition();

        followMouse = false;
    }

    void ResetPosition()
    {
        transform.localPosition = zeroZeroPosition;
    }

    public void InitializeData(Transform parent = null, ItemData itemToMatch = null)
    {
        if (itemToMatch == null && ItemData == null)
            return;

        if (itemToMatch != null)
            ItemData = itemToMatch;

        if (parent != null)
        {
            transform.SetParent(parent);
            ResetPosition();
        }

        image.sprite = ItemData.Sprite;
        
        var newScale = ItemData.Scale;
        transform.localScale = new Vector3(newScale, newScale);
    }
}
