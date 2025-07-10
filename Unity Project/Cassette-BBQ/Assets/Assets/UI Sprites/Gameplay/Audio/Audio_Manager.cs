using DG.Tweening;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;

public class Audio_Manager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] AudioSource _music_AudioSource;
    [SerializeField] AudioSource _soundEffects_AudioSource;

    [Space, Header("Music")] 
    [SerializeField] AudioClip[] _music_List;

    private float _music_Volume; 
    public void SetMusic(float volume) { _music_Volume = volume; }

    private float _soundEffects_Volume; 
    public void SetSoundEffects(float volume) { _soundEffects_Volume = volume; }

    private void OnEnable()
    {
        GamesSettingsEvents.OnAudioChanged += UpdateAudioLevels;
        AudioEvents.OnMusicChanged += MoveToNewSong;
        AudioEvents.OnFadeOutMusic += FadeOutMusic;
    }

    private void OnDisable()
    {
        GamesSettingsEvents.OnAudioChanged -= UpdateAudioLevels;
        AudioEvents.OnMusicChanged -= MoveToNewSong;
        AudioEvents.OnFadeOutMusic -= FadeOutMusic;
    }

    private void Start()
    {
        if (_music_List.Length > 0)
        {
            PlayMusic(_music_List[Random.Range(0, _music_List.Length)]);
        }
    }

    private void PlayMusic(AudioClip musicClip)
    {
        _music_AudioSource.volume = 100;
        _music_AudioSource.clip = musicClip;
        _music_AudioSource.Play();
    }

    // Keeps the actual Audio Source audio levels consistent with UI etc.
    private void UpdateAudioLevels()
    {
        _music_AudioSource.volume = _music_Volume;
        _soundEffects_AudioSource.volume = _soundEffects_Volume;
    }

    private void FadeOutMusic()
    {
        _music_AudioSource.DOFade(0, 1f).OnComplete(() =>
        {
            _music_AudioSource.Stop();
            _music_AudioSource.clip = null;
        });
    }

    private void MoveToNewSong()
    {
        _music_AudioSource.DOFade(0, 1f).OnComplete(() =>
        {
            _music_AudioSource.Stop();
            _music_AudioSource.clip = null;
            PlayMusic(_music_List[Random.Range(0, _music_List.Length)]);
            _music_AudioSource.DOFade(_music_Volume, 1f);
        });
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
}
