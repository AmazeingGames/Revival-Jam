using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item Data/Item Data 3D")]
public class ItemData3D : ScriptableObject
{
    [field: SerializeField] public Mesh Mesh { get; private set; }
    [field: SerializeField] public Material Material { get; private set; }
    [field: SerializeField] public ItemAndAbilityManager.Tools ItemType { get; private set; }

    [field: Header("Transform")]
    [field: SerializeField] public float Scale { get; private set; } = 1f;
    [field: SerializeField] public Vector3 ColliderScale { get; private set; }

}
