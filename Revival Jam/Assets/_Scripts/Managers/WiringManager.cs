using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WiringManager : Singleton<WiringManager>
{
    [SerializeField] GameObject wiringCabinet;

    public static IEnumerator SetWiringCabinet(bool isActive)
    {
        while (Instance == null)
        {
            Debug.Log("looping");
            yield return null;
        }

        Debug.Log($"Set wiring cabinet active {isActive} - wiring cabine is active {Instance.wiringCabinet.activeSelf}");

        Instance.wiringCabinet.SetActive(isActive);
    }


}
