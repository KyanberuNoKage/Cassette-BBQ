using TMPro;
using UnityEngine;
using UnityEngine.UI;
using KyanberuGames.Utilities;

public class OnSlider_UpdateText : MonoBehaviour
{
    [SerializeField] Slider _slider;

    [SerializeField] TextMeshProUGUI _numberDysplay;

    [SerializeField] string _textPrefix = "$";

    [SerializeField] string _textSuffix = ".99";

    [SerializeField] SoundOptionType _soundOptionType;

    private void OnEnable()
    {
        GamesSettingsEvents.OnSoundSetup_Start += UpdateSound_UI;
    }

    private void OnDisable()
    {
        GamesSettingsEvents.OnSoundSetup_Start -= UpdateSound_UI;
    }

    private void UpdateSound_UI(float musicVolume, float soundEffectsVolume)
    {
        if (_slider == null)
        {
            DebugEvents.AddDebugWarning("Slider not assigned.");
            return;
        }

        if (_soundOptionType == SoundOptionType.SoundEffects)
        {
            _slider.value = soundEffectsVolume;
        }
        else
        {
            _slider.value = musicVolume;
        }
    }

    public void OnSliderChanged()
    {
        UpdateText();
        GamesSettingsEvents.SoundLevelChanged(_slider.value, _soundOptionType);
    }

    public void UpdateText()
    {
        if (_slider != null && _numberDysplay != null)
        {
            int newValue = Mathf.RoundToInt(_slider.value * 100);
            _numberDysplay.text = $"{_textPrefix}{newValue.ToString()}{_textSuffix}";
        }
        else
        {
            DebugEvents.AddDebugWarning("Number display or slider is not assigned.");
        }
    }
}

public enum SoundOptionType
{
    Music,
    SoundEffects
}
