using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorTest : Singleton<CursorTest>
{
    float zConst;
    private void Start()
    {
        zConst = transform.position.z;
    }
    private void Update()
    {
        var pos = transform.position;
        pos.z = zConst;
        transform.position = pos;
    }
}
