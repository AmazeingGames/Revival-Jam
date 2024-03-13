using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static AudioManager;
using static UnityEngine.UI.Button;

public class UIButtonBase : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Button")]
    [SerializeField] protected bool isArcadeButton = false;

    OneShotSounds HoverSound => isArcadeButton ? OneShotSounds.ArcadeUIHover : OneShotSounds.UIHover;
    OneShotSounds ClickSound => isArcadeButton ? OneShotSounds.ArcadeUISelect : OneShotSounds.UISelect;

    Transform Origin => isArcadeButton ? ArcadeSoundEmitter.Transform : transform;

    public void OnPointerClick(PointerEventData eventData) 
        => OnClick();

    public void OnPointerEnter(PointerEventData eventData) 
        => OnEnter();

    public void OnPointerExit(PointerEventData eventData) 
        => OnExit();

    public virtual void OnClick()
    {
        if (AudioManager.Instance != null) 
            TriggerAudioClip(ClickSound, Origin);
        return;
    }

    public virtual void OnEnter()
    {
        if (!CanBeClicked())
            return;

        if (isArcadeButton)
            return;

        TriggerAudioClip(HoverSound, Origin);
    }

    public virtual void OnExit()
    {
    }

    public virtual bool CanBeClicked() => true;

   
}
