using System;
using UnityEngine;
using UnityEngine.UI;
using Action = System.Action;

public class GameSettings_manager : MonoBehaviour
{
    [Header("Save Data")]
    [SerializeField] SaveData_Controller _saveData_Controller;

    [Header("Options Menu Settings")]
    [SerializeField] private float _music_Volume;
    [SerializeField] private float _soundEffects_Volume;
    [SerializeField] private bool _isOneHanded = false;

    [Header("Default Game Settings")]
    [SerializeField, Range(0, 1)] private float _default_Music_Volume = 0.9f;
    [SerializeField, Range(0, 1)] private float _default_SoundEffects_Volume = 0.9f;

    [Header("Audio Sources")]
    [SerializeField] Audio_Manager _audioManager;

    private void OnEnable()
    {
        GamesSettingsEvents.OnSoundLevelChanged += UpdateSoundLevels;
        SaveData_MessageBus.OnSetIsOneHanded += SetIsOneHanded_FromSaveData;
    }

    private void SetIsOneHanded_FromSaveData(bool isOneHanded)
    {
        _isOneHanded = isOneHanded;
    }

    private void OnDisable()
    {
        GamesSettingsEvents.OnSoundLevelChanged -= UpdateSoundLevels;
        SaveData_MessageBus.OnSetIsOneHanded -= SetIsOneHanded_FromSaveData;
    }

    private void Start()
    {
        if (_saveData_Controller.DoesSaveExist())
        {
            

            _saveData_Controller.LoadAndSetupGame();
            GamesSettingsEvents.SoundSetup_Start(_music_Volume, _soundEffects_Volume);
            GamesSettingsEvents.InformAudioChanged();
        }
        else
        {
            SetDefaultValues();
        }
    }
    
    public void ResetData(bool resetData)
    {
        if (resetData)
        {
            // If player is sure.
            _saveData_Controller.ResetData();
        }
        else
        {
            // Leave "Are you sure" screen.
        }
    }

    private void SetDefaultValues()
    {
        _music_Volume = _default_Music_Volume; // Default music volume
        _soundEffects_Volume = _default_SoundEffects_Volume; // Default sound effects volume

        _audioManager.SetMusic(_music_Volume);
        _audioManager.SetSoundEffects(_soundEffects_Volume);

        GamesSettingsEvents.SoundSetup_Start(_music_Volume, _soundEffects_Volume);
        GamesSettingsEvents.InformAudioChanged();
    }

    public void UpdateSoundLevels(float Volume, SoundOptionType SentFrom)
    {
        if (SentFrom == SoundOptionType.SoundEffects)
        {
            _soundEffects_Volume = Volume;
            _audioManager.SetSoundEffects(_soundEffects_Volume);
            GamesSettingsEvents.InformAudioChanged();
        }
        else if (SentFrom == SoundOptionType.Music)
        {
            _music_Volume = Volume;
            _audioManager.SetMusic(_music_Volume);
            GamesSettingsEvents.InformAudioChanged();
        }
    }

    public void ToggleOneHandedMode(Toggle oneHandToggle_UI)
    {
        _isOneHanded = oneHandToggle_UI.isOn;
        // Update the script with OneHanded Input check.
        GamesSettingsEvents.ToggleOneHanded(_isOneHanded);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}



public static class  GamesSettingsEvents
{
    public static event Action<bool> OnOneHandedToggled;

    public static void ToggleOneHanded(bool isToggledOn)
    {
        OnOneHandedToggled?.Invoke(isToggledOn);
    }

    public static event Action<float, float> OnSoundSetup_Start;

    /// <summary>
    /// For when the game starts and the audio levels are set up. Sets either default for saved levels.
    /// </summary>
    /// <param name="musicVolume"></param>
    /// <param name="soundEffectsVolume"></param>
    public static void SoundSetup_Start(float musicVolume, float soundEffectsVolume)
    {
        OnSoundSetup_Start?.Invoke(musicVolume, soundEffectsVolume);
    }

    public static event Action<float, SoundOptionType> OnSoundLevelChanged;

    /// <summary>
    /// For when player has set the audio levels in the options menu. Will be saved on exit.
    /// </summary>
    /// <param name="musicVolume"></param>
    /// <param name="soundEffectsVolume"></param>
    public static void SoundLevelChanged(float volume, SoundOptionType volumeFrom)
    {
        OnSoundLevelChanged?.Invoke(volume, volumeFrom);
    }

    public static event Action OnAudioChanged;

    public static void InformAudioChanged()
    {
        OnAudioChanged?.Invoke();
    }
}       