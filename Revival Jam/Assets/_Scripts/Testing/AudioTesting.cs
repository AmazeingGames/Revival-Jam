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
            StartEventTrack(LoopingAudio.CastleAmbience);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            StartEventTrack(LoopingAudio.ForestAmbience);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            StartEventTrack(LoopingAudio.AtticAmbience);
#endif


    }
}
