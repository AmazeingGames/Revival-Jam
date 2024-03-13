using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMouse : MonoBehaviour
{
    [SerializeField] SpriteSwap spriteSwap;
    public static event Action<SpriteSwap> GetReference;

    private void Start()
    {
        GetReference?.Invoke(spriteSwap);
    }
}
