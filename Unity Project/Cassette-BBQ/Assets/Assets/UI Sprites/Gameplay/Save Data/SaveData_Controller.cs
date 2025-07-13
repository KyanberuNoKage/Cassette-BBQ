using UnityEngine;
using System.Collections.Generic;
using System;

public class SaveData_Controller : MonoBehaviour
{
    // Data to save:
    [Header("Game Settings")]
    [SerializeField] float _musicVolume, _soundEffectsVolume;
    [SerializeField] bool _isOneHanded;

    [Header("Cassettes")]
    [SerializeField] List<Cassette_Anim_Control> RevealedCassettes;

    [Header("Scores")]/**     Score, Date/Time        **/
    [SerializeField] Dictionary<int, string> _highScores;

    private void RequestData()
    {
        // Sends requests to the message bus to get data from different scripts.

        _musicVolume = SaveData_MessageBus.OnRequestMusicVolume?.Invoke() ?? 0f;
        _soundEffectsVolume = SaveData_MessageBus.OnRequestSoundEffectsVolume?.Invoke() ?? 0f;

        _isOneHanded = SaveData_MessageBus.OnRequestIsOneHanded?.Invoke() ?? false;

        RevealedCassettes = SaveData_MessageBus.OnRequestRevealedCassettes?.Invoke() ?? new List<Cassette_Anim_Control>();
        
        _highScores = SaveData_MessageBus.OnRequestHighScores?.Invoke() ?? new Dictionary<int, string>();
    }


}

public static class SaveData_MessageBus
{
    public static Func<float> OnRequestMusicVolume;
    public static Func<float> OnRequestSoundEffectsVolume;
    public static Func<List<Cassette_Anim_Control>> OnRequestRevealedCassettes;
    public static Func<Dictionary<int, string>> OnRequestHighScores;
    public static Func<bool> OnRequestIsOneHanded;

    public static Action<float> OnSetMusicVolume;
    public static Action<float> OnSetSoundEffectsVolume;
    public static Action<List<Cassette_Anim_Control>> OnSetRevealedCassettes;
    public static Action<Dictionary<int, string>> OnSetHighScores;

}
