using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Wire : MonoBehaviour, IPointerDownHandler, IPointerMoveHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Clicked");
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        Debug.Log("Move");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
