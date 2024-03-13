using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static ArcadeGameManager;
using static ArcadeMenuManager;

public class NewSaveButton : ArcadeButton
{
    public override void OnPress()
    {
        base.OnPress();

        ArcadeMenuManager.Instance.UpdateArcadeMenu(ArcadeMenuManager.ArcadeMenuState.GameRunning);
        ArcadeGameManager.Instance.UpdateArcadeState(ArcadeState.StartLevel, 1);
    }
}
