using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//I now realize this sound is confusing, given that an 'emmitter' is a real thing in FMOD
public class ArcadeSoundEmitter : Singleton<ArcadeSoundEmitter>
{
    public static Transform Transform { get => GetTransform(); }

    static Transform GetTransform()
    {
        if (Instance == null)
        {
            if (Player.Instance == null)
                return null;

            Debug.LogWarning("Arcade Sound Emitter is Null | Returning Player Transform instead to avoid Null Reference Exception");
            return Player.Instance.transform;
        }

        return Instance.transform;
    }
}
