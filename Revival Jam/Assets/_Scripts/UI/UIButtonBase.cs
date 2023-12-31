using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static AudioManager;

public class UIButtonBase : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    [Header("Button")]
    [SerializeField] protected bool isArcadeButton = false;

    EventSounds HoverSound => isArcadeButton ? EventSounds.ArcadeUIHover : EventSounds.UIHover;
    EventSounds ClickSound => isArcadeButton ? EventSounds.ArcadeUISelect : EventSounds.UISelect;

    Transform Origin => isArcadeButton ? ArcadeSoundEmitter.Transform : transform;

    public void OnPointerClick(PointerEventData eventData) => OnClick();

    public void OnPointerEnter(PointerEventData eventData) => OnEnter();

    public virtual void OnClick()
    {
        if (!CanBeClicked())
            return;

        if (isArcadeButton)
            return;

        TriggerAudioClip(ClickSound, Origin);
    }

    public virtual void OnEnter()
    {
        if (!CanBeClicked())
            return;

        if (isArcadeButton)
            return;

        TriggerAudioClip(HoverSound, Origin);
    }

    public virtual bool CanBeClicked() => true;
}
