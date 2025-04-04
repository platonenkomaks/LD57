using System.Collections.Generic;
using UnityEngine;

public class AudioSettingsUI : MonoBehaviour
{
    [System.Serializable]
    public class CategorySlider
    {
        public SoundCategory category;
        public UnityEngine.UI.Slider slider;
    }

    public CategorySlider[] volumeSliders;
    public UnityEngine.UI.Toggle muteToggle;

    private readonly Dictionary<SoundCategory, float> _savedVolumes = new Dictionary<SoundCategory, float>();

    private void Start()
    {
        // Initialize sliders with current volume values
        foreach (CategorySlider cs in volumeSliders)
        {
            if (cs.slider != null)
            {
                cs.slider.value = G.AudioManager.GetCategoryVolume(cs.category);

                // Add slider value change listener
                cs.slider.onValueChanged.AddListener((value) => {
                    G.AudioManager.SetCategoryVolume(cs.category, value);
                    SaveSettings(); // Save settings when slider value changes
                });

                // Save current volume
                _savedVolumes[cs.category] = cs.slider.value;
            }
        }

        // Add mute toggle listener
        if (muteToggle != null)
        {
            muteToggle.onValueChanged.AddListener((isMuted) => {
                ToggleMute(isMuted);
            });
        }

        // Load saved settings if available
        LoadSettings();
    }

    // Mute/unmute audio
    public void ToggleMute(bool isMuted)
    {
        if (isMuted)
        {
            // Save current values and set to 0
            foreach (CategorySlider cs in volumeSliders)
            {
                if (cs.slider != null)
                {
                    _savedVolumes[cs.category] = G.AudioManager.GetCategoryVolume(cs.category);
                    Debug.Log($"Saving volume for {cs.category}: {_savedVolumes[cs.category]}");
                    G.AudioManager.SetCategoryVolume(cs.category, 0f);
                    cs.slider.SetValueWithoutNotify(0f);
                }
            }
        }
        else
        {
            // Restore saved values
            foreach (CategorySlider cs in volumeSliders)
            {
                if (cs.slider != null && _savedVolumes.ContainsKey(cs.category))
                {
                    Debug.Log($"Restoring volume for {cs.category}: {_savedVolumes[cs.category]}");
                    G.AudioManager.SetCategoryVolume(cs.category, _savedVolumes[cs.category]);
                    cs.slider.SetValueWithoutNotify(_savedVolumes[cs.category]);
                }
            }
        }

        // Save mute state
        SaveMuteState(isMuted);
    }

    // Save settings
    public void SaveSettings()
    {
        foreach (SoundCategory category in System.Enum.GetValues(typeof(SoundCategory)))
        {
            PlayerPrefs.SetFloat("Audio_" + category.ToString(), G.AudioManager.GetCategoryVolume(category));
        }

        PlayerPrefs.Save();
    }

    // Save mute state separately
    private void SaveMuteState(bool isMuted)
    {
        PlayerPrefs.SetInt("Audio_Muted", isMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    // Load settings
    public void LoadSettings()
    {
        bool hasSavedSettings = false;

        foreach (SoundCategory category in System.Enum.GetValues(typeof(SoundCategory)))
        {
            string key = "Audio_" + category.ToString();
            if (PlayerPrefs.HasKey(key))
            {
                float volume = PlayerPrefs.GetFloat(key);
                G.AudioManager.SetCategoryVolume(category, volume);
                _savedVolumes[category] = volume;
                hasSavedSettings = true;
            }
        }

        // Update sliders if there are saved settings
        if (hasSavedSettings)
        {
            foreach (CategorySlider cs in volumeSliders)
            {
                if (cs.slider != null && _savedVolumes.ContainsKey(cs.category))
                {
                    cs.slider.SetValueWithoutNotify(_savedVolumes[cs.category]);
                }
            }
        }

        // Check mute state
        if (PlayerPrefs.HasKey("Audio_Muted") && muteToggle != null)
        {
            bool isMuted = PlayerPrefs.GetInt("Audio_Muted") == 1;
            muteToggle.SetIsOnWithoutNotify(isMuted);

            if (isMuted)
            {
                ToggleMute(true);
            }
        }
    }
}