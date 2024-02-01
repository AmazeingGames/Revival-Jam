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
        SetUpdateFunc();

        slider.onValueChanged.AddListener((v) =>
        {
            updateSetting?.Invoke(v);
        });

        string sliderOldValue = $"{sliderType} slider old value : {slider.value}";

        LoadSlider();

        string sliderNewValue = $"slider new value : {slider.value}";

        updateSetting?.Invoke(slider.value);

        SettingsManager settings = SettingsManager.Instance;

        Debug.Log($"{sliderOldValue} | {sliderNewValue}");
    }

    public void Update()
    {
        Debug.Log("running update");
    }
    //
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

    //Sets the delegate to update the proper setting when called
    void SetUpdateFunc()
    {
        
    }

    //Sets the slider to equal the last loaded setting
    void LoadSlider()
    {
        
    }
}
