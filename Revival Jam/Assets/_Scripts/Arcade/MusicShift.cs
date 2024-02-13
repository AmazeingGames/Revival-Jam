using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicShift : MonoBehaviour
{
    [SerializeField] AudioManager.OneShotSounds ambienceTrigger;
    bool isPlayerOver = false;

    private void Update()
    {
        Interact();
    }

    void Interact()
    {
        if (!isPlayerOver)
            return;

        if (ArcadeGameManager.Instance == null)
            return;

        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger Enter");

        if (IsCollisionPlayer(collision))
            isPlayerOver = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Trigger Exit");

        if (IsCollisionPlayer(collision))
            isPlayerOver = false;
    }

    bool IsCollisionPlayer(Collider2D collision) => (Player.Instance != null && collision.gameObject == Player.Instance.gameObject);
}
