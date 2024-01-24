using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackButton : UIButtonBase
{
    public void Start()
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnClick()
    {
        base.OnClick();

        MenuManager.Instance.UpdateState(MenuManager.MenuState.Settings);
    }
}
