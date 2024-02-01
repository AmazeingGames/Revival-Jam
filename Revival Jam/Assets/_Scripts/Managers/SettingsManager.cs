using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MenuManager;

public class SettingsManager : Singleton<SettingsManager>
{
    static SaveSettings settings = new();

    public float GameVolume { get => settings.gameVolume; private set => settings.gameVolume = value; }
    public float MouseSensitivity { get => settings.mouseSensitivity; private set => settings.mouseSensitivity = value; }

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

    private void Awake()
    {
        base.Awake();
        GameManager.Load(ref settings);
    }

    void HandleMenuStateChange(MenuState menuState)
    {
        if (MenuManager.Instance.PreviousState == MenuState.Settings)
            GameManager.Save(settings);
    }

    [field: Serializable] class SaveSettings
    {
        public float gameVolume = .5f;
        public float mouseSensitivity = .25f;
    }
}
