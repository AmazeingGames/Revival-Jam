using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Obsolete]
public class StartActive : MonoBehaviour
{
    [SerializeField] bool startActive;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(startActive);
    }
}
