using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbienceManager : Singleton<AmbienceManager>
{
    bool isPaused;
    AudioManager.LoopingAudio currentAmbience;
    AudioManager.LoopingAudio previousAmbience;
    AudioManager.LoopingAudio pauseRememberAmbience;
    AudioManager.LoopingAudio arcadeRememberAmbience;


    private void OnEnable()
    {
        ArcadeGameManager.AfterArcadeStateChange += HandleArcadeStateChange;
        MenuManager.OnMenuStateChange += HandleMenuStateChange;
        FocusStation.ConnectToStation += HandleConnectToStation;
    }

    private void OnDisable()
    {
        ArcadeGameManager.AfterArcadeStateChange -= HandleArcadeStateChange;
        MenuManager.OnMenuStateChange -= HandleMenuStateChange;
        FocusStation.ConnectToStation -= HandleConnectToStation;
    }

    void HandleArcadeStateChange(ArcadeGameManager.ArcadeState newArcadeState)
    {
        switch (newArcadeState)
        {
            case ArcadeGameManager.ArcadeState.StartLevel:
                //Play forest ambience
                StartTrack(AudioManager.LoopingAudio.ForestAmbience);
                break;
        }
    }

    //
    void HandleMenuStateChange(MenuManager.MenuState newMenuState)
    {
        switch (newMenuState)
        {
            case MenuManager.MenuState.Pause:
                if (!isPaused)
                {
                    isPaused = true;
                    pauseRememberAmbience = currentAmbience;
                }

                if (MenuManager.Instance.PreviousState == MenuManager.MenuState.GameResume || MenuManager.Instance.PreviousState == MenuManager.MenuState.GameStart)
                {
                    //Play pause music
                }
                break;

            case MenuManager.MenuState.GameResume:
                isPaused = false;
                
                break;

            case MenuManager.MenuState.GameStart:
                //play attic ambience
                StartTrack(AudioManager.LoopingAudio.AtticAmbience);
                break;
        }
    }

    void HandleConnectToStation(FocusStation.ConnectEventArgs connectEventArgs)
    {
        switch (connectEventArgs.LinkedStation) 
        {
            case PlayerFocus.FocusedOn.FrontView:
                //Start attic ambience
                //Stop game ambience (or set volume to half)
                //Remember arcade ambience
                if (PlayerFocus.Instance.PreviouslyFocusedOn == PlayerFocus.FocusedOn.Arcade)
                    arcadeRememberAmbience = currentAmbience;
                StartTrack(AudioManager.LoopingAudio.AtticAmbience);
                break;

            case PlayerFocus.FocusedOn.Arcade:
                StartTrack(arcadeRememberAmbience);
                break;
        }
    }

    //Create a variable to have room for different track types
    void StartTrack(AudioManager.LoopingAudio track)
    {
        if (currentAmbience == track || track == AudioManager.LoopingAudio.Null)
            return;

        Debug.Log($"Started Ambience : {track}");

        previousAmbience = currentAmbience;
        currentAmbience = track;
        AudioManager.StartEventTrack(track);
    }
}