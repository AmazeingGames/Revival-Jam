using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item Data")]
public class ItemData : ScriptableObject
{
    [field: SerializeField] public Sprite Sprite { get; private set; }
    [field: SerializeField] public ItemAndAbilityManager.Tools ItemType { get; private set; }

    [field: Header("Transform")]
    [field: SerializeField] public Vector2 MouseFollowOffset { get; private set; }
    [field: SerializeField] public float Scale { get; private set; } = 1f;
}
