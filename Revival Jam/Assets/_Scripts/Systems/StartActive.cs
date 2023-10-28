using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartActive : MonoBehaviour
{
    [SerializeField] bool startActive;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(startActive);
    }
}
