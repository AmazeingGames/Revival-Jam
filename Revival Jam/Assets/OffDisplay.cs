using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffDisplay : MonoBehaviour
{
    [SerializeField] MeshRenderer model;

    private void Update()
    {
        model.enabled = !ArcadeGameManager.IsMachineOn;
    }
}
