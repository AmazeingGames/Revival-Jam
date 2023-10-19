using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class UIButtonBase : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnEnter();
    }

    public virtual void OnClick()
    {
        //if (CanBeClicked())
            //AudioManager.Instance.TriggerAudioClip(UIClick, gameObject);
    }


    public virtual void OnEnter()
    {

    }

    public virtual bool CanBeClicked()
    {
        return true;
    }
}
