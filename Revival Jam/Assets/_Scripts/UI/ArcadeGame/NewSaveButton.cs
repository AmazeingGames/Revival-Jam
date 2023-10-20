using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ArcadeGameManager;

public class NewSaveButton : UIButtonBase
{
    public override void OnClick()
    {
        base.OnClick();

        ArcadeMenuManager.Instance.UpdateArcadeMenu(ArcadeMenuManager.ArcadeMenuState.GameRunning);
        ArcadeGameManager.Instance.UpdateArcadeState(ArcadeState.StartLevel, 1);
    }
}
