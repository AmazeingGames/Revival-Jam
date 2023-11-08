using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoDTriggerVolume : MonoBehaviour
{
    [SerializeField] GameObject gamgameObject;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Player passed thru me");
        //if (!ArcadeGameManager.Instance) return;
        //ArcadeGameManager.Instance.UpdateArcadeState(ArcadeGameManager.ArcadeState.Win);

        if (gamgameObject != null)
            gamgameObject.SetActive(true);

    }
}
