using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ArcadeMenuManager;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArcadeButton : Button
{
    // Disables color change when moused over
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (MovementManager.Instance.ArrowMoveType != MovementManager.ArrowMovementType.Click)
            base.OnPointerEnter(eventData);
    }

    // Disables button event call
    public override void OnPointerClick(PointerEventData eventData)
    {
        if (MovementManager.Instance.ArrowMoveType != MovementManager.ArrowMovementType.Click)
            base.OnPointerClick(eventData);
    }

    // Disables color change when pressed
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (MovementManager.Instance.ArrowMoveType != MovementManager.ArrowMovementType.Click)
            base.OnPointerDown(eventData);
    }

    private void OnEnable()
    {
        base.OnEnable();
        onClick.AddListener(OnPress);
    }

    private void OnDisable()
    {
        base.OnDisable();
        onClick.RemoveAllListeners();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump") && PlayerFocus.Instance.Focused == PlayerFocus.FocusedOn.Arcade && MachineAbilities.Instance.IsMachineOn)
            OnPress();
    }

    public virtual void OnPress()
    {
        if (SceneLoader.Instance == null)
            return;

        if (AudioManager.Instance != null)
            AudioManager.TriggerAudioClip(AudioManager.OneShotSounds.ArcadeUISelect, ArcadeSoundEmitter.Transform);
    }


}
