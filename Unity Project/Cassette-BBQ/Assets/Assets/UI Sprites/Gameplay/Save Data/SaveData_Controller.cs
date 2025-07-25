using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using UnityEditor;

public class SaveData_Controller : MonoBehaviour
{
    private static string path;

    // Data to save:
    [Header("Game Settings")]
    [SerializeField] float _musicVolume, _soundEffectsVolume;
    [SerializeField] bool _isOneHanded;

    [Header("Cassettes")]
    [SerializeField] List<string> _revealedCassettes;
    [SerializeField] private Cassette_Anim_Control _defaultCassette;

    [Header("Scores")]/**     Score, Date/Time        **/
    [SerializeField] Dictionary<int, string> _highScores;

    [Space, Header("Default Values")]
    [SerializeField] float _defaultSound_Volume = 0.8f;
    [SerializeField] bool _default_isOneHandedMode = false;


    private void Awake()
    {
        path = Path.Combine(Application.persistentDataPath, "saveData.json");
    }

    private void OnEnable()
    {
        SaveDataEvents.OnTryLoadGameData += LoadAndSetupGame;
        ReceiptStateHolder.OnDeleteData += ResetData;
        GamesSettingsEvents.OnGameQuit += SaveAndQuit;
    }

    private void OnDisable()
    {
        SaveDataEvents.OnTryLoadGameData -= LoadAndSetupGame;
        ReceiptStateHolder.OnDeleteData -= ResetData;
        GamesSettingsEvents.OnGameQuit -= SaveAndQuit;
    }  

    private void SaveAndQuit()
    {
        StartCoroutine(SaveGame());
    }

    public IEnumerator SaveGame()
    {
        RequestData();

        yield return null;

        // Create serializable object to save data.
        GameData dataToSave = new GameData();
        dataToSave.AddData
            (
                _musicVolume,
                _soundEffectsVolume,
                _isOneHanded,
                _revealedCassettes,
                _highScores
            );


        string json = JsonUtility.ToJson( dataToSave );
        File.WriteAllText(path, json);

        Debug.Log("Game data saved to: " + path);

        // To ensure write time is complete before quitting.
        yield return new WaitForSeconds(0.1f);

        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public bool DoesSaveExist()
    {
        if (File.Exists(path))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void LoadAndSetupGame() 
    {
        StartCoroutine(StartUpGame());    
    }

    private IEnumerator StartUpGame()
    {
        yield return LoadSaveData();

        yield return SendData();

        GamesSettingsEvents.SoundSetup_Start(_musicVolume, _soundEffectsVolume);
        GamesSettingsEvents.InformAudioChanged();

        yield return null;

        // Save data exists and is now set up, skip the tutorial.
        TutorialEvents.SkipTutorial();
    }

    private IEnumerator LoadSaveData()
    {
        GameData newGameData = TryLoadGameData();

        // Logs the saved data, if there is any,
        // then updates the required systems with the retrieved information.
        if (newGameData != null)
        {
            _musicVolume = newGameData.musicVolume;
            _soundEffectsVolume = newGameData.soundEffectsVolume;
            _isOneHanded = newGameData.isOneHanded;

            _revealedCassettes = newGameData.revealedCassettes;

            // Ensure default cassette is on there if no cassettes have been revealed yet.
            if (_revealedCassettes.Count == 0) { _revealedCassettes.Add(_defaultCassette.thisCassettesName); }

            // Revert List of high scores back to Dict of high scores.
            _highScores = newGameData.highScores
            .OrderByDescending(p => p.score)
            .ToDictionary(p => p.score, p => p.dateTime);
        }

        yield return null;
    }

    private IEnumerator SendData()
    {
        SaveData_MessageBus.SetMusicVolume( _musicVolume );//connected
        SaveData_MessageBus.SetSoundEffectsVolume( _soundEffectsVolume );//connected
        SaveData_MessageBus.SetIsOneHanded( _isOneHanded );//connected
        SaveData_MessageBus.SetRevealedCassettes( _revealedCassettes );//connected
        SaveData_MessageBus.SetHighScoreDict( _highScores );//connected



        yield return null;
    }

    private static GameData TryLoadGameData()
    {
        if (!File.Exists(path))
        {
            Debug.LogWarning("No previous save game");
            return null;
        }
        else
        {
            string json = File.ReadAllText(path);
            GameData data = JsonUtility.FromJson<GameData>(json);

            return data;
        }
    }

    private void RequestData()
    {
        // Sends requests to the message bus to get data from different scripts.

        _musicVolume = SaveData_MessageBus.OnRequestMusicVolume?.Invoke() ?? 0f;
        _soundEffectsVolume = SaveData_MessageBus.OnRequestSoundEffectsVolume?.Invoke() ?? 0f;

        _isOneHanded = SaveData_MessageBus.OnRequestIsOneHanded?.Invoke() ?? false;

        _revealedCassettes = SaveData_MessageBus.OnRequestRevealedCassettes?.Invoke() ?? new List<string>();
        
        _highScores = SaveData_MessageBus.OnRequestHighScores?.Invoke() ?? new Dictionary<int, string>();
    }

    public void ResetData()
    {
        if (File.Exists(path))
        {
            // Delete current save file.
            File.Delete(path);

            /** SIMPLY DON@T CREATE NEW BASE DATA, JUST DELETE THE FILE AND LET THE GAME SETUP THE DEFAULTS ON STARTUP.
            // Create new serializable data for saving.
            GameData newSaveData = new GameData();

            List<string> newCassetteList = new List<string>();
            newCassetteList.Add(_defaultCassette.thisCassettesName);

            // Give it all the default values.
            newSaveData.AddData
                (
                    _musicVolume: _defaultSound_Volume, 
                    _soundEffectsVolume: _defaultSound_Volume, 
                    _isOneHanded: _default_isOneHandedMode, 
                    _revealedCassettes: newCassetteList, 
                    _highScores: new Dictionary<int, string>()
                );

            // Endure the values in the SaveData_Controller are
            // also at their default values.
            _musicVolume = newSaveData.musicVolume;
            _soundEffectsVolume= newSaveData.soundEffectsVolume;
            _isOneHanded = newSaveData.isOneHanded;
            _revealedCassettes = newCassetteList;
            _highScores = new Dictionary<int, string>();

            // Send this data out to all areas of the game to be reset.
            StartCoroutine(SendData());

            // Save the new default info as to ensure defaults have stuck.
            string json = JsonUtility.ToJson(newSaveData);
            File.WriteAllText(path, json);
            **/

            // Quit application to ensure the game restarts with the new data, rather than saving data on exit.


            #if UNITY_EDITOR
                EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        else
        {
            Debug.LogWarning("No save file found to delete.");
        }
    }
}

[Serializable]
public class ScoreDataPair
{
    public int score;
    public string dateTime;
}


[Serializable]
public class GameData
{
    // Data to save:
    [Header("Game Settings")]
    public float musicVolume, soundEffectsVolume;
    public bool isOneHanded;

    [Header("Cassettes")]
    public List<string> revealedCassettes;

    [Header("Scores")]/**     Score, Date/Time        **/
    public List<ScoreDataPair> highScores;


    public void AddData
        (
            float _musicVolume, 
            float _soundEffectsVolume,
            bool _isOneHanded,
            List<string> _revealedCassettes,
            Dictionary<int, string> _highScores
        )
    {
        musicVolume = _musicVolume;
        soundEffectsVolume = _soundEffectsVolume;
        isOneHanded = _isOneHanded;
        revealedCassettes = _revealedCassettes;
        
        // Convert dict into List to serialize and convert to Json.
        highScores = new List<ScoreDataPair>();
        foreach(var pair in _highScores)
        {
            ScoreDataPair newPair = new ScoreDataPair();
            newPair.score = pair.Key;
            newPair.dateTime = pair.Value;

            highScores.Add(newPair);
        }
    }


    public void ClearData()
    {
        musicVolume = 0f;
        soundEffectsVolume = 0f;
        isOneHanded = false;
        revealedCassettes?.Clear();
        highScores?.Clear();
    }
}


public static class SaveDataEvents
{
    public static event Action OnTryLoadGameData;

    public static void TryLoadGameData_Event()
    {
        OnTryLoadGameData?.Invoke();
    }
}

public static class SaveData_MessageBus
{
    // Get data.
    public static Func<float> OnRequestMusicVolume;
    public static Func<float> OnRequestSoundEffectsVolume;
    public static Func<List<string>> OnRequestRevealedCassettes;
    public static Func<Dictionary<int, string>> OnRequestHighScores;
    public static Func<bool> OnRequestIsOneHanded;

    // Set Data,
    public static Action<float> OnSetMusicVolume;
    public static void SetMusicVolume(float newValue)
    {
        OnSetMusicVolume?.Invoke(newValue);
    }

    public static Action<float> OnSetSoundEffectsVolume;
    public static void SetSoundEffectsVolume(float newValue)
    {
        OnSetSoundEffectsVolume?.Invoke(newValue);
    }

    public static Action<List<string>> OnSetRevealedCassettes;
    public static void SetRevealedCassettes(List<string> newList)
    {
        OnSetRevealedCassettes?.Invoke(newList);
    }

    public static Action<Dictionary<int, string>> OnSetHighScoreDict;
    public static void SetHighScoreDict(Dictionary<int, string> newDict)
    {
        OnSetHighScoreDict?.Invoke(newDict);
    }

    public static Action<bool> OnSetIsOneHanded;
    public static void SetIsOneHanded(bool isTrue)
    {
        OnSetIsOneHanded?.Invoke(isTrue);
    }
}
