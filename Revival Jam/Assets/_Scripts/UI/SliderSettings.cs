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
    Func<float> loadSetting;


    public void Start()
    {
        SetLoadValue();
        SetSetting();

        slider.onValueChanged.AddListener((v) =>
        {
            updateSetting?.Invoke(v);
        });

        Debug.Log("Initialized Setting");

        slider.value = loadSetting();
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

    //Sets the slider to equal the last loaded setting
    void SetLoadValue()
    {
        SettingsManager settings = SettingsManager.Instance;

        loadSetting = sliderType switch
        {
            SliderType.Sensitivity => settings.LoadSensitivity,
            SliderType.Volume => settings.LoadVolume,
            _ => null
        };
    }
}
