using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue")]
public class Dialogue : ScriptableObject
{
    [field: Header("Dialogue")]
    [field: SerializeField] public List<Message> Messages { get; private set; }
    [field: SerializeField] public List<Actor> Actors { get; private set; }
}
