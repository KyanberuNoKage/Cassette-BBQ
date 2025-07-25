using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using KyanberuGames.Utilities;
using Random = UnityEngine.Random;

public class Audio_Manager : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] AudioMixer _audioMixer;
    [SerializeField] AudioMixerGroup _effectsMixerGroup;

    [Space, Header("Audio Sources")]
    [SerializeField] AudioSource _music_AudioSource;
    [SerializeField] AudioSource _soundEffects_AudioSource;
    [SerializeField] AudioSource _grillBackground_SoundEffect;

    [SerializeField] private List<AudioSource> _effects_AudioSourcePool = new List<AudioSource>();
    [SerializeField] private int _poolSize = 10;

    [Space, Header("Music")] 
    [SerializeField] AudioClip[] _music_List;

    [Space, Header("Sound Effects")]
    [SerializeField] AudioClip[] _soundEffects_List;

    [SerializeField] private float _music_Volume; 
    public void SetMusic(float volume) { _music_Volume = volume; }

    [SerializeField] private float _soundEffects_Volume; 
    public void SetSoundEffects(float volume) { _soundEffects_Volume = volume; }

    private void OnEnable()
    {
        GamesSettingsEvents.OnAudioChanged += UpdateAudioLevels;
        AudioEvents.OnMusicChanged += MoveToNewSong;
        AudioEvents.OnFadeOutMusic += FadeOutMusic;
        AudioEvents.OnPlayEffect += PlaySoundEffect;
        AudioEvents.OnPlayLoopedEffect += PlaySoundEffect; //Looped Version
        AudioEvents.OnLoopedEffectStopped += StopLoopedEffect;
        AudioEvents.OnGrillScreen += GrillBackground;
        AudioEvents.OnWhooshPlayed += PlayRandomWooshEffect;
        AudioEvents.OnStartMainMenuMusic += Start_MainMenuMusic;

        TimerEvents.OnTimerFinished += ResetAudio;
        

        SaveData_MessageBus.OnRequestMusicVolume += () => _music_Volume; // Send _musicVolume
        SaveData_MessageBus.OnRequestSoundEffectsVolume += () => _soundEffects_Volume; // Send _soundEffectsVolume
        SaveData_MessageBus.OnSetMusicVolume += SetMusicVolume_FromSaveData;
        SaveData_MessageBus.OnSetSoundEffectsVolume += SetSoundEffectVolume_FromSaveData;
    }

    private void SetMusicVolume_FromSaveData(float volume)
    {
        _music_Volume = volume;
    }

    private void SetSoundEffectVolume_FromSaveData(float volume)
    {
        _soundEffects_Volume = volume;
    }

    private void OnDisable()
    {
        GamesSettingsEvents.OnAudioChanged -= UpdateAudioLevels;
        AudioEvents.OnMusicChanged -= MoveToNewSong;
        AudioEvents.OnFadeOutMusic -= FadeOutMusic;
        AudioEvents.OnPlayEffect -= PlaySoundEffect;
        AudioEvents.OnPlayLoopedEffect -= PlaySoundEffect; //Looped Version
        AudioEvents.OnLoopedEffectStopped -= StopLoopedEffect;
        AudioEvents.OnGrillScreen -= GrillBackground;
        AudioEvents.OnWhooshPlayed -= PlayRandomWooshEffect;
        AudioEvents.OnStartMainMenuMusic -= Start_MainMenuMusic;

        TimerEvents.OnTimerFinished -= ResetAudio;

        SaveData_MessageBus.OnRequestMusicVolume -= () => _music_Volume; // Send _musicVolume
        SaveData_MessageBus.OnRequestSoundEffectsVolume -= () => _soundEffects_Volume; // Send _soundEffectsVolume
        SaveData_MessageBus.OnSetMusicVolume -= SetMusicVolume_FromSaveData;
        SaveData_MessageBus.OnSetSoundEffectsVolume -= SetSoundEffectVolume_FromSaveData;
    }

    private void Start()
    {
        if (_music_List.Length > 0)
        {
            Start_MainMenuMusic();
        }

        GameObject audioHolder = new GameObject("AudioPoolHolder");

        // Create pool of AudioSources for sound effects.
        for (int i = 0; i < _poolSize; i++)
        {
            AudioSource source = audioHolder.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = _effectsMixerGroup;
            _effects_AudioSourcePool.Add(source);
        }
    }

    private void PlayMusic(AudioClip musicClip, bool isLooped = false)
    {
        // Start with music volume at 0 to fade in.
        _audioMixer.SetFloat("Music", 0);

        _music_AudioSource.loop = isLooped;
        _music_AudioSource.clip = musicClip;
        _music_AudioSource.Play();

        _audioMixer.DOSetFloat("Music", Mathf.Log10(_music_Volume) * 20, 1f);
    }

    float mutedVolume_Level = 0.0001f; //0.0001f is -80db which is effectively silent.

    // Keeps the actual Audio Source audio levels consistent with UI etc.
    private void UpdateAudioLevels()
    {
        if (_music_Volume < mutedVolume_Level)
        {
            _music_Volume = mutedVolume_Level; // Prevents log10(0) which is undefined.
        }

        // To translate 0-1 percentage to decibels for the Audio Mixer.
        _audioMixer.SetFloat("Music", Mathf.Log10(_music_Volume) * 20);

        if (_soundEffects_Volume < mutedVolume_Level)
        {
            _soundEffects_Volume = mutedVolume_Level; // Prevents log10(0) which is undefined.
        }
        _audioMixer.SetFloat("Sound Effects", Mathf.Log10(_soundEffects_Volume) * 20);

        _audioMixer.SetFloat("Grill Sound", Mathf.Log10(_soundEffects_Volume) * 20);
    }

    private void ResetAudio()
    {
        // Fade out pooled effects
        _audioMixer.DOSetFloat("Sound Effects", Mathf.Log10(mutedVolume_Level) * 20, 1f);
        foreach (AudioSource source in _effects_AudioSourcePool)
        {
            source.Stop();
            source.clip = null;
        } // Make the audio sources in the pool active again, ready for the next run.
        _audioMixer.DOSetFloat("Sound Effects", Mathf.Log10(_soundEffects_Volume) * 20, 1f);


        _audioMixer.DOSetFloat("Grill Sound", Mathf.Log10(mutedVolume_Level) * 20, 1f).OnComplete(() =>
        {
            _grillBackground_SoundEffect.Stop();
            _grillBackground_SoundEffect.clip = null;
        }); // Make the audio group is active again, ready for the next run.
        _audioMixer.DOSetFloat("Grill Sound", Mathf.Log10(_soundEffects_Volume) * 20, 1f);

        // Fade out main SFX
        _audioMixer.DOSetFloat("Sound Effects", Mathf.Log10(mutedVolume_Level) * 20, 1f).OnComplete(() =>
        {
            _soundEffects_AudioSource.Stop();
            _soundEffects_AudioSource.clip = null;
        });
        // Make the main SFX group is active again, ready for the next run.
        _audioMixer.DOSetFloat("Sound Effects", Mathf.Log10(_soundEffects_Volume) * 20, 1f);
    }

    private void FadeOutMusic()
    {
        _audioMixer.DOSetFloat("Music", Mathf.Log10(mutedVolume_Level) * 20, 1f).OnComplete(() =>
        {
            _music_AudioSource.Stop();
            _music_AudioSource.clip = null;
        });
    }

    private void MoveToNewSong()
    {
        _audioMixer.DOSetFloat("Music", Mathf.Log10(mutedVolume_Level) * 20, 1f).OnComplete(() =>
        {
            _music_AudioSource.Stop();
            _music_AudioSource.clip = null;
            PlayMusic(_music_List[Random.Range(0, _music_List.Length)]);
            _audioMixer.DOSetFloat("Music", Mathf.Log10(_music_Volume) * 20, 1f);
        });
    }

    private void Start_MainMenuMusic()
    {
        // First track in the music list is the menu music.
        if (_music_AudioSource.isPlaying)
        {
            FadeOutMusic();
            PlayMusic(_music_List[0], true);
        }
        else
        {
            PlayMusic(_music_List[0], true);
        }  
    }

    public void UI_Button_Click()
    {
        int randomIndex = Random.Range(0, 3);

        switch (randomIndex)
        {
            case 0:
                PlaySoundEffect(SoundEffects.Switch_2);
                break;
            case 1:
                PlaySoundEffect(SoundEffects.Switch_1);
                break;
            case 2:
                PlaySoundEffect(SoundEffects.Switch_7);
                break;
            default:
                DebugEvents.AddDebugWarning("Random index out of range, defaulting to Switch_2 sound effect.");
                PlaySoundEffect(SoundEffects.Switch_2);
                break;
        }
    }

    public void UI_Button_Click_Slider()
    {
        PlaySoundEffect(SoundEffects.Click_1);
    }

    public void UI_MeatTable_Select()
    {
        switch (Random.Range(1,4))
        {
            case 1:
                PlaySoundEffect(SoundEffects.Cloth_1);
                break;
            case 2:
                PlaySoundEffect(SoundEffects.Cloth_2);
                break;
            case 3:
                PlaySoundEffect(SoundEffects.Cloth_3);
                break;
        }
    }

    private void PlaySoundEffect(SoundEffects ChosenEffect)
    {
        // If enum and array of effects are  set up to correspond correctly in order;
        // then this will select the correct clip based on their shared index.
        AudioClip effectClip = _soundEffects_List[(int)ChosenEffect];
        _soundEffects_AudioSource.PlayOneShot(effectClip);
    }

    /// <summary>
    /// Loop altering varsion of PlaySoundEffect.
    /// </summary>
    /// <param name="ChosenEffect"></param>
    /// <param name="isLooped"></param>
    private void PlaySoundEffect(SoundEffects ChosenEffect, bool isLooped)
    {
        // If enum and array of effects are  set up to correspond correctly in order;
        // then this will select the correct clip based on their shared index.
        AudioClip effectClip = _soundEffects_List[(int)ChosenEffect];

        AudioSource availableAudioSource = null;

        foreach (AudioSource source in _effects_AudioSourcePool)
        {
            if (!source.isPlaying)
            {
                availableAudioSource = source;
                break;
            }
        }

        if (availableAudioSource != null)
        {
            availableAudioSource.clip = effectClip;
            availableAudioSource.loop = isLooped;
            availableAudioSource.Play();
        }
        else
        {
            DebugEvents.AddDebugError($"No available audio sources in _effects_AudioSourcePool to play sound effect {effectClip.name}");
        }
    }

    private void StopLoopedEffect(SoundEffects effectToStop)
    {
        foreach (AudioSource source in _effects_AudioSourcePool)
        {
            if 
            (
                source.isPlaying 
                && source.loop 
                && source.clip == _soundEffects_List[(int)effectToStop]
            )
            {
                source.Stop();
                source.clip = null;
                source.loop = false;

                break; // Exit early if found.
            }
        }
    }

    private void GrillBackground(bool IsOn)
    {
        if (IsOn)
        {
            // Reset it.
            _grillBackground_SoundEffect.volume = 0;
            _grillBackground_SoundEffect.clip = null;
            _grillBackground_SoundEffect.loop = true;
            // Change it to effect with same index as Grill_Sizle_long enum.
            _grillBackground_SoundEffect.clip = _soundEffects_List[(int)SoundEffects.Grill_Sizzle_Long];
            // Start it.
            _grillBackground_SoundEffect.Play();
            _grillBackground_SoundEffect.DOFade(_soundEffects_Volume, 1f);
        }
        else
        {
            _grillBackground_SoundEffect.DOFade(0, 1f).OnComplete(() =>
            {
                _grillBackground_SoundEffect.Stop();
            });
        }
    }

    private void PlayRandomWooshEffect()
    {
        int randomIndex = Random.Range(0,2);

        switch (randomIndex)
        {
            case 0:
                PlaySoundEffect(SoundEffects.woosh);
                break;
            case 1:
                PlaySoundEffect(SoundEffects.fast_woosh);
                break;
            default:
                DebugEvents.AddDebugWarning("Random index out of range, defaulting to whoosh sound effect.");
                PlaySoundEffect(SoundEffects.woosh);
                break;
        }
    }
}


public static class AudioEvents
{
    public static event Action OnMusicChanged;

    public static void ChangeMusic()
    {
        OnMusicChanged?.Invoke();
    }

    public static event Action OnFadeOutMusic;

    public static void FadeOutMusic()
    {
        OnFadeOutMusic?.Invoke();
    }

    public static event Action<SoundEffects> OnPlayEffect;

    public static void PlayEffect(SoundEffects ChosenEffect)
    {
        OnPlayEffect?.Invoke(ChosenEffect);
    }

    public static event Action<SoundEffects, bool> OnPlayLoopedEffect;

    public static void PlayLoopedEffect(SoundEffects ChosenEffect, bool isLooped = false)
    {
        OnPlayLoopedEffect?.Invoke(ChosenEffect, isLooped);
    }

    public static event Action<SoundEffects> OnLoopedEffectStopped;

    public static void StopLoopedEffect(SoundEffects effectToStop)
    {
        OnLoopedEffectStopped?.Invoke(effectToStop);
    }

    public static event Action OnStartMainMenuMusic;

    public static void StartMainMenuMusic()
    {
        OnStartMainMenuMusic?.Invoke();
    }

    public static event Action<bool> OnGrillScreen;

    public static void SetGrillScreen(bool IsOn)
    {
        OnGrillScreen?.Invoke(IsOn);
    }

    public static event Action OnWhooshPlayed;

    public static void PlayRandomWhoosh()
    {
        OnWhooshPlayed?.Invoke();
    }
}


public enum SoundEffects
{
    Bacon_Sizzle,
    Quick_Sizzle,
    Grill_Sizzle_Long,
    Confirmation,
    Error_5,
    Error_6,
    Switch_1,
    Switch_2,
    Switch_7,
    Click_1,
    Impact_Plate,
    Cloth_1,
    Cloth_2,
    Cloth_3,
    swoosh,
    woosh,
    fast_woosh,
}
