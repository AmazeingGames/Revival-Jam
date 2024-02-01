using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Obsolete]
public class ResumeButton : UIButtonBase
{
    public override void OnClick()
    {
        base.OnClick();

        MenuManager.Instance.UpdateState(MenuManager.MenuState.GameResume);
    }

    public override void OnEnter()
    {
        base.OnEnter();
    }
}
