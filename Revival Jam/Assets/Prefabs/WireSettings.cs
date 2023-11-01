using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Wire Settings")]

public class WireSettings : ScriptableObject
{
    [field: SerializeField] public string ReceptacleTag { get; private set; }

    [field: Header("Movement Settings")]
    [field: SerializeField] public float Sensitivity { get; private set;  }

    [field: SerializeField] public bool NormalizeVector { get; private set; }
    [field: SerializeField] public bool GetRawAxis { get; private set; }
    [field: SerializeField] public bool Divide { get; private set; }
    [field: SerializeField] public bool SetPosition { get; private set; }

    [field: Header("Mouse Settings")]
    [field: SerializeField] public bool LockMouse { get; private set; } = true;
    [field: SerializeField] public bool HideMouse { get; private set; } = true;



}
