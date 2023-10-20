using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

//To fake functionality, only enable this component when it's being used
public class VirtualScreen : GraphicRaycaster
{
    [SerializeField] PlayerFocus.FocusedOn stationType;

    public static event Action<VirtualScreen, PlayerFocus.FocusedOn> FindStation;

    public Camera screenCamera; // Reference to the camera responsible for rendering the virtual screen's rendertexture

    public GraphicRaycaster screenCaster; // Reference to the GraphicRaycaster of the canvas displayed on the virtual screen

    public Transform lastMouseClick;

    protected override void Start()
    {
        base.Start();

        FindStation?.Invoke(this, stationType);
    }

    // Called by Unity when a Raycaster should raycast because it extends BaseRaycaster.
    public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
    {
        Ray ray = eventCamera.ScreenPointToRay(eventData.position); // Mouse

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Figure out where the pointer would be in the second camera based on texture position or RenderTexture.
            Vector3 virtualPos = new(hit.textureCoord.x, hit.textureCoord.y);
            virtualPos.x *= screenCamera.targetTexture.width;
            virtualPos.y *= screenCamera.targetTexture.height;

            eventData.position = virtualPos;

            screenCaster.Raycast(eventData, resultAppendList);
        }
    }

}