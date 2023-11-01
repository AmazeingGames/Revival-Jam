using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Wire Settings")]

public class WireSettings : ScriptableObject
{
    [field: SerializeField] public float Sensitivity { get; private set;  }
    [field: SerializeField] public string ReceptacleTag {get; private set; }

    [field: SerializeField] public bool NormalizeVector {get; private set; }
    [field: SerializeField] public bool GetRawAxis {get; private set; }
    [field: SerializeField] public bool Divide {get; private set; }
}
