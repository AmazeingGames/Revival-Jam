using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MenuManager;

public class SettingsManager : Singleton<SettingsManager>
{
    SaveSettings settings = new();

    public float GameVolume { get => settings.gameVolume; private set => settings.gameVolume = value; }
    public float MouseSensitivity { get => settings.mouseSensitivity; private set => settings.mouseSensitivity = value; }


    public float LoadVolume() => GameVolume;
    public float LoadSensitivity() => MouseSensitivity;
    public void UpdateVolume(float newValue) => GameVolume = newValue;
    public void UpdateSensitivity(float newValue) => MouseSensitivity = newValue;


    private void OnEnable()
    {
        MenuManager.OnMenuStateChange += HandleMenuStateChange;
    }

    private void OnDisable()
    {
        MenuManager.OnMenuStateChange -= HandleMenuStateChange;
    }

    const string pathName = "settings";

    private void Start()
    {
        GameManager.Load(pathName, ref settings);
    }

    void HandleMenuStateChange(MenuState menuState)
    {
        if (MenuManager.Instance.PreviousState == MenuState.Settings)
            GameManager.Save(settings, pathName);
    }

    class SaveSettings
    {
        public float gameVolume;
        public float mouseSensitivity;
    }
}
