using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteSwap : MonoBehaviour
{
    [SerializeField] Image primaryVisuals;
    [SerializeField] Image temporaryVisuals;

    public void SetTemporaryVisuals(Sprite sprite, float tempScale = 1)
    {
        primaryVisuals.enabled = false;
        temporaryVisuals.enabled = true;

        temporaryVisuals.transform.localScale = Vector3.one * tempScale;
        temporaryVisuals.sprite = sprite;
    }

    public void SetRegularVisuals()
    {
        primaryVisuals.enabled = true;
        temporaryVisuals.enabled = false;
    }
}
