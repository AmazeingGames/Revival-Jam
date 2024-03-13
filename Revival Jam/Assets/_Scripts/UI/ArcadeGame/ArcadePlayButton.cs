using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static ArcadeMenuManager;
using static UnityEngine.UI.Image;

public class ArcadePlayButton : ArcadeButton
{
    public override void OnPress()
    {
        base.OnPress();

        ArcadeMenuManager.Instance.UpdateArcadeMenu(ArcadeMenuState.SelectSave);
    }
}
