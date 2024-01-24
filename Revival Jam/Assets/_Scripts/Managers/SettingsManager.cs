using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : Singleton<SettingsManager>
{
    public float GameVolume { get => gameVolume; private set => gameVolume = value; }
    public float MouseSensitivity { get => mouseSensitivity; private set => mouseSensitivity = value; }

    float gameVolume = 1.0f;
    float mouseSensitivity = 1.0f;

    public void UpdateVolume(float newValue) => UpdateValue(ref gameVolume, newValue);
    public void UpdateSensitivity(float newValue) => UpdateValue(ref mouseSensitivity, newValue);

    void UpdateValue<T>(ref T value,T newValue) => value = newValue;
}
