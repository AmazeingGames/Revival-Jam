using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainDetector : MonoBehaviour
{
    [SerializeField] Transform detector;
    [SerializeField] bool detectTerrainPresent;
    [SerializeField] bool debugResults;
    [SerializeField] float raycastLength;
    [SerializeField] LayerMask terrainLayer;

    public bool _ShouldFlip { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        _ShouldFlip = ShouldFlip();

        //CheckDebug($"({gameObject.name}) {DebugText()} : {_ShouldFlip}");
    }

    public bool ShouldFlip()
    {
        if (detector == null)
        {
            Debug.LogWarning("Detector is null");
        }

        RaycastHit2D raycast = Physics2D.Raycast(detector.position, Vector3.down, raycastLength, terrainLayer);

        return detectTerrainPresent ? raycast : !raycast;
    }

    void OnDrawGizmos()
    {
        Vector3 rayDirection = new(0, -raycastLength, 0);
        Gizmos.DrawRay(detector.position, rayDirection);
    }

    string DebugText()
    {
        return detectTerrainPresent ? "Is there terrain present" : "Is there terrain absent";
    }

    void CheckDebug(string text)
    {
        if (debugResults)
            Debug.Log(text);
    }
}
