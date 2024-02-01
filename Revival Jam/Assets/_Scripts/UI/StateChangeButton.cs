using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static MenuManager;


public class StateChangeButton : UIButtonBase
{
    [Header("State Change")]
    [SerializeField] GameState gameState = GameState.Null;
    [SerializeField] MenuState menuState = MenuState.Null;

    public override void OnClick()
    {
        base.OnClick();

        if (gameState != GameState.Null)
            GameManager.Instance.UpdateGameState(gameState);

        if (menuState != MenuState.Null)
            MenuManager.Instance.UpdateState(menuState);
    }
}
