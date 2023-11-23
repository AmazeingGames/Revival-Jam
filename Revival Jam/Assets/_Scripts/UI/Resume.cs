using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resume : MonoBehaviour
{
    public static event Action ResumeGame;

    public void OnResume()
    {
        ResumeGame?.Invoke();
    }
}
