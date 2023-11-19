using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ArcadeSoundEmitter : Singleton<ArcadeSoundEmitter>
{
    public static Transform Transform { get => GetTransform(); }

    static Transform GetTransform()
    {
        if (Instance == null)
        {
            Debug.LogWarning("Arcade Sound Emitter is Null | Returning Player Transform instead to avoid Null Reference Exception");
            return Player.Instance.transform;
        }

        return Instance.transform;
    }
}
