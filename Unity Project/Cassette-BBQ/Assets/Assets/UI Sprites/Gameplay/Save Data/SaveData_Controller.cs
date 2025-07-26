using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using UnityEditor;
using CustomInspector;
using KyanberuGames.Utilities;

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

    // DEFAULT VALUES
    float _defaultSound_Volume = 0.9f;
    bool _default_isOneHandedMode = false;


#if UNITY_EDITOR
    #region Print Persistent Data Path
#pragma warning disable CS0414 // Suppress: Field assigned but never used (its 'used' by CustomInspector Button)
    [SerializeField, Button(nameof(PrintPersistentDataPath), tooltip = "Prints Application.persistentDataPath to the console")]
    [HideField] bool _printPersistentDataPath = false;
    #pragma warning restore CS0414

    private void PrintPersistentDataPath()
    {
        Debug.Log($"Current Persistent Data Path:\n{Application.persistentDataPath}");
        DebugEvents.AddDebugLog($"Current Persistent Data Path:\n{Application.persistentDataPath}");
    }
    #endregion
#endif

    private void Awake()
    {
        path = Path.Combine(Application.persistentDataPath, "saveData.json");
    }

    private void Start()
    {
        SaveDataEvents.TryLoadGameData_Event();
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

        DebugEvents.AddDebugLog("Game data saved to: " + path);

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
        yield return LoadSaveData(); // Load it.

        yield return SendData(); // Send it. :emoji_sunglasses:

        GamesSettingsEvents.SoundSetup_Start(_musicVolume, _soundEffectsVolume);
        GamesSettingsEvents.InformAudioChanged();

        yield return null;

        if (DoesSaveExist())
        {
            // Save data exists and is now set up, skip the tutorial.
            TutorialEvents.SkipTutorial();
        }
    }

    private IEnumerator LoadSaveData()
    {
        GameData newGameData = TryLoadGameData();

        // Gets the saved data if there is any, then updates the other scripts.
        if (newGameData != null)
        {
            _musicVolume = newGameData.musicVolume;
            _soundEffectsVolume = newGameData.soundEffectsVolume;
            _isOneHanded = newGameData.isOneHanded;
            _revealedCassettes = newGameData.revealedCassettes ?? new List<string>();

            if (_revealedCassettes.Count == 0)
            {
                _revealedCassettes.Add(_defaultCassette.thisCassettesName);
            }

            if (newGameData.highScores != null)
            {
                _highScores = newGameData.highScores
                    .OrderByDescending(p => p.score)
                    .ToDictionary(p => p.score, p => p.dateTime);
            }
            else
            {
                _highScores = new Dictionary<int, string>();
            }
        }
        else
        {
            // --- FIRST‑TIME PLAYER, DEFAULTS ARE SET HERE ---
            _musicVolume = _defaultSound_Volume;
            _soundEffectsVolume = _defaultSound_Volume;
            _isOneHanded = _default_isOneHandedMode;
            _revealedCassettes = new List<string>();
            _revealedCassettes.Add(_defaultCassette.thisCassettesName);
            _highScores = new Dictionary<int, string>();
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
            DebugEvents.AddDebugWarning("No previous save game");
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

            // --------------------------------------------------------------------------------------------------------
            // SIMPLY DO NOT CREATE NEW BASE DATA, JUST DELETE THE FILE AND LET THE GAME SETUP THE DEFAULTS ON STARTUP.
            // --------------------------------------------------------------------------------------------------------
            #region Old Code
            /**
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
            #endregion

#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        else
        {
            DebugEvents.AddDebugError("No save file found to 'Reset'.");
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
