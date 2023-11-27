using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasPlayerCamera : MonoBehaviour
{
    Canvas canvas;
    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponent<Canvas>();

        StartCoroutine(SetCameraAsPlayer());
    }

    IEnumerator SetCameraAsPlayer()
    {
        while (canvas.worldCamera == null)
        {
            yield return null;

            if (Player3D.Instance == null)
                continue;

            canvas.worldCamera = Player3D.Instance.PlayerCamera;

            yield break;
        }
    }
}
