using UnityEngine;

public class GameSettings_manager : MonoBehaviour
{
    [Header("Options Menu Settings")]
    [SerializeField] private float _music_Volume;
    [SerializeField] private float _soundEffects_Volume;

    [Header("Default Game Settings")]
    [SerializeField, Range(0, 1)] private float _default_Music_Volume = 0.9f;
    [SerializeField, Range(0, 1)] private float _default_SoundEffects_Volume = 0.9f;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource _music_AudioSource;
    [SerializeField] private AudioSource _soundEffects_AudioSource;

    private void OnEnable()
    {
        GamesSettingsEvents.OnSoundLevelChanged += UpdateSoundLevels;
    }

    private void OnDisable()
    {
        GamesSettingsEvents.OnSoundLevelChanged -= UpdateSoundLevels;
    }

    private void Start()
    {
        if (/**saveGame_Exists = true**/ false)
        {
            //Set up game with save game settings.
        }
        else
        {
            SetDefaultValues();
        }
    }

    private void SetDefaultValues()
    {
        _music_Volume = _default_Music_Volume; // Default music volume
        _soundEffects_Volume = _default_SoundEffects_Volume; // Default sound effects volume

        GamesSettingsEvents.SoundSetup_Start(_music_Volume, _soundEffects_Volume);
    }

    public void UpdateSoundLevels(float Volume, SoundOptionType SentFrom)
    {
        if (SentFrom == SoundOptionType.SoundEffects)
        {
            _soundEffects_Volume = Volume;
        }
        else if (SentFrom == SoundOptionType.Music)
        {
            _music_Volume = Volume;
        }
    }
}



public static class  GamesSettingsEvents
{
    public static event System.Action<float, float> OnSoundSetup_Start;

    /// <summary>
    /// For when the game starts and the audio levels are set up. Sets either default for saved levels.
    /// </summary>
    /// <param name="musicVolume"></param>
    /// <param name="soundEffectsVolume"></param>
    public static void SoundSetup_Start(float musicVolume, float soundEffectsVolume)
    {
        OnSoundSetup_Start?.Invoke(musicVolume, soundEffectsVolume);
    }

    public static event System.Action<float, SoundOptionType> OnSoundLevelChanged;

    /// <summary>
    /// For when player has set the audio levels in the options menu. Will be saved on exit.
    /// </summary>
    /// <param name="musicVolume"></param>
    /// <param name="soundEffectsVolume"></param>
    public static void SoundLevelChanged(float volume, SoundOptionType volumeFrom)
    {
        OnSoundLevelChanged?.Invoke(volume, volumeFrom);
    }
}       