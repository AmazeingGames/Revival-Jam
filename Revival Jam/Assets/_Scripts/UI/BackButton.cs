using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MenuManager;

[Obsolete]
public class BackButton : UIButtonBase
{
    public override void OnClick()
    {
        base.OnClick();

        MenuManager.Instance.UpdateState(MenuManager.Instance.PreviousState);
    }
}
