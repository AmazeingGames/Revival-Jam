using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButton : UIButtonBase
{
    [SerializeField] int levelToLoad;
    [SerializeField] List<Sprite> levelIcons;

    public override void OnClick()
    {
        base.OnClick();

        GameManager.Instance.UpdateGameState(GameManager.GameState.StartGame, levelToLoad);

    }

    public override void OnEnter()
    {

    }
}
