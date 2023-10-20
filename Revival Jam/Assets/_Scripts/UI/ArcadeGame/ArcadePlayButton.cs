using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ArcadeMenuManager;

public class ArcadePlayButton : UIButtonBase
{
    public override void OnClick()
    {
        base.OnClick();

        ArcadeMenuManager.Instance.UpdateArcadeMenu(ArcadeMenuState.SelectSave);
    }
}
