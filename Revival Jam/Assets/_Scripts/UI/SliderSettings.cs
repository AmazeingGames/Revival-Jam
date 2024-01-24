using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderSettings : MonoBehaviour
{
    [SerializeField] SliderType sliderType;

    [Header("Components")]
    [SerializeField] Slider slider;

    public enum SliderType { Null, Sensitivity, Volume }

    Action<float> updateSetting;

    public void Start()
    {
        SetSetting();

        slider.onValueChanged.AddListener((v) =>
        {
            updateSetting?.Invoke(v);
        });

        Debug.Log("Initialized Setting");
        updateSetting?.Invoke(slider.value);
    }

    //Sets the delegate to update the proper setting when called
    void SetSetting()
    {
        SettingsManager settings = SettingsManager.Instance;

        updateSetting = sliderType switch
        {
            SliderType.Sensitivity => settings.UpdateSensitivity,
            SliderType.Volume => settings.UpdateVolume,
            _ => null
        };
    }
}
