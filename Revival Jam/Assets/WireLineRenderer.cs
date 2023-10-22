using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class WireLineRenderer : MonoBehaviour
{
    [SerializeField] Transform wire;
    [SerializeField] Color color;
    LineRenderer lineRenderer;

    protected void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, wire.position);
    }
}
