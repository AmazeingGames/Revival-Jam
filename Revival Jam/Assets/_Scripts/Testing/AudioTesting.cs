using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static AudioManager;
public class AudioTesting : MonoBehaviour
{
    void Update()
    {

#if DEBUG
        if (Input.GetKeyDown(KeyCode.Alpha1))
            StartEventInstance(LoopingSounds.CastleAmbience, InstanceStartMode.Start);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            StartEventInstance(LoopingSounds.ForestAmbience, InstanceStartMode.Start);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            StartEventInstance(LoopingSounds.AtticAmbience, InstanceStartMode.Start);
#endif


    }
}
