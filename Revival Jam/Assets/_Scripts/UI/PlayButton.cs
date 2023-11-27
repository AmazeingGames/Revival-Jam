using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButton : UIButtonBase
{
    public override void OnClick()
    {
        base.OnClick();

        GameManager.Instance.UpdateGameState(GameManager.GameState.StartGame);
        MenuManager.Instance.UpdateState(MenuManager.MenuState.GameStart);
    }
}
