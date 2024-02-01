using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        ReadySlider();

        slider.onValueChanged.AddListener((v) =>
        {
            updateSetting?.Invoke(v);
        });

        updateSetting?.Invoke(slider.value);

        SettingsManager settings = SettingsManager.Instance;
    }

    void ReadySlider()
    {
        SettingsManager settings = SettingsManager.Instance;

        switch (sliderType)
        {
            case SliderType.Sensitivity:
                updateSetting = settings.UpdateSensitivity;
                slider.value = settings.MouseSensitivity;
                break;

            case SliderType.Volume:
                updateSetting = settings.UpdateVolume;
                slider.value = settings.GameVolume;
                break;
        }
    }
}
