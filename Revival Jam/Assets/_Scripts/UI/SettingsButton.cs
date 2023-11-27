using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsButton : UIButtonBase
{
    public override void OnClick()
    {
        base.OnClick();

        MenuManager.Instance.UpdateState(MenuManager.MenuState.Settings);
    }

    public override void OnEnter()
    {
        base.OnEnter();
    }
}
