using System;
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

    public static event Action<bool, ItemData> GrabTool;

    private void OnEnable()
    {
        Interface.UseItem += HandleUse;
    }

    private void OnDisable()
    {
        Interface.UseItem -= HandleUse;
    }

    private void Start()
    {
        InitializeData();
    }

    private void Update()
    {
        FollowMouse();

        //Not necessary to be in update, but helps for testing purposes
#if DEBUG
        InitializeData();
#endif
    }

    void HandleUse(ItemData itemData)
    {
        if (itemData == null) 
            return;

        if (itemData != ItemData)
            return;

        OnUse();
    }

    //'Destroys' the item after being used
    void OnUse() => gameObject.SetActive(false);

    //Sets position to mouse + offset
    void FollowMouse()
    {
        if (followMouse)
            transform.position = Input.mousePosition + (Vector3)ItemData.MouseFollowOffset;
    }

    //Follows virtual cursor
    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("Mouse Click");

        followMouse = true;

        OnGrabTool();
    }

    //Stops following mouse and resets to slot position
    public void OnPointerUp(PointerEventData eventData)
    {
        //Debug.Log("Mouse Up");

        if (followMouse)
            ResetPosition();

        followMouse = false;

        OnGrabTool();
    }

    //Not sure about this in a snytaxtical sense, but it seems good enough
    void OnGrabTool() => GrabTool?.Invoke(followMouse, ItemData);

    //Sets position back to its original slot position
    void ResetPosition() => transform.localPosition = zeroZeroPosition;

    //Matches the given data and prepares it for use
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
