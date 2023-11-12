using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcadeQuad : Singleton<ArcadeQuad>
{
    [SerializeField] GameObject cabinet;

    public static IEnumerator SetCabinet(bool isActive)
    {
        while (Instance == null)
        {
            Debug.Log("looping");
            yield return null;
        }

        Debug.Log($"Set wiring cabinet active {isActive} - wiring cabine is active {Instance.cabinet.activeSelf}");

        Instance.cabinet.SetActive(isActive);
    }
}
