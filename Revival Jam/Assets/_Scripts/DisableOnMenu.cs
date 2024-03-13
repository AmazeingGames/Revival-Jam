using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnMenu : MonoBehaviour
{
    [field: Header("Debug")]
    [field: SerializeField] public bool Disable { get; private set; } = true;

    public static event Action<DisableOnMenu> AddToDisable;

    void Start()
    {
        if (!Disable)
            Debug.LogWarning("Disable should always remain true. Please either fix or remove this component");

        AddToDisable?.Invoke(this);
    }

}
