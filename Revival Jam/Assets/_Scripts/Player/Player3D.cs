using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player3D : Singleton<Player3D>
{
    [field: SerializeField] public Camera PlayerCamera { get; private set; }
}
