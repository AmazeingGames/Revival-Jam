using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenClamper : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] RectTransform rectTransform;
    [SerializeField] RectTransform canvasTransform;
    Canvas canvasCanvas;

    Vector2 negativeBounds;
    Vector2 positiveBounds;

    RectTransform parentRect;
    Camera cam;

    private void Start()
    {
        canvasCanvas = canvasTransform.GetComponent<Canvas>();

        CalculateBounds();
    }

    void CalculateBounds()
    {
        parentRect = rectTransform.parent.GetComponent<RectTransform>();

        cam = canvasCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvasCanvas.worldCamera;

        negativeBounds.x = rectTransform.pivot.x * rectTransform.rect.size.x;
        positiveBounds.x = canvasTransform.rect.size.x - (1 - rectTransform.pivot.x) * rectTransform.rect.size.x;

        negativeBounds.y = rectTransform.pivot.y * rectTransform.rect.size.y;
        positiveBounds.y = canvasTransform.rect.size.y - (1 - rectTransform.pivot.y) * rectTransform.rect.size.y;
    }

    private void LateUpdate()
    {
        Clamp();
    }

    //Source Code from: https://forum.unity.com/threads/keep-ui-objects-inside-screen.523766/
    void Clamp()
    {
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(cam, rectTransform.position);

        screenPos.x = Mathf.Clamp(screenPos.x, negativeBounds.x, positiveBounds.x);
        screenPos.y = Mathf.Clamp(screenPos.y, negativeBounds.y, positiveBounds.y);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPos, cam, out Vector2 anchoredPos);

        rectTransform.localPosition = anchoredPos;
    }

}
