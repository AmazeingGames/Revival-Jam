using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSaveButton : UIButtonBase
{
    public override void OnClick()
    {
        base.OnClick();

        ArcadeMenuManager.Instance.UpdateArcadeMenu(ArcadeMenuManager.ArcadeMenuState.GameRunning);
        GameManager.Instance.UpdateGameState(GameManager.GameState.StartLevel, 1);
    }
}
