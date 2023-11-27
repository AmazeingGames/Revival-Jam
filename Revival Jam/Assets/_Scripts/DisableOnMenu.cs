using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnMenu : MonoBehaviour
{
    public static event Action<DisableOnMenu> AddToDisable;

    void Start() => AddToDisable?.Invoke(this);

}
