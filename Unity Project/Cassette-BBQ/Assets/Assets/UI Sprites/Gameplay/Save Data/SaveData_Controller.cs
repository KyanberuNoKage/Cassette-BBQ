using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using CustomInspector;
using KyanberuGames.Utilities;
using System.Text;
using System.Runtime.InteropServices;

public class SaveData_Controller : MonoBehaviour
{

#if UNITY_WEBGL && !UNITY_EDITOR
[DllImport("__Internal")]
private static extern void ReloadPage();
#endif

    private static string path;

    // Data to save:
    [Header("Game Settings")]
    [SerializeField] float _musicVolume, _soundEffectsVolume;
    [SerializeField] bool _isOneHanded;

    [Header("Cassettes")]
    [SerializeField] List<string> _revealedCassettes;
    [SerializeField] CassetteType _defaultCassette_Type = CassetteType.SummerTime; // SumerTime is default, in-case inspector default doesn't work.
    string _defaultCassette_Name;

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
        _defaultCassette_Name = _defaultCassette_Type.ToString();
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

#if UNITY_WEBGL && !UNITY_EDITOR
        PlayerPrefs.SetString("saveData", json);
        PlayerPrefs.Save();
#else
        File.WriteAllText(path, json);
        DebugEvents.AddDebugLog("Game data saved to: " + path);
#endif

        // To ensure write time is complete before quitting.
        yield return new WaitForSeconds(0.1f);

        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        #elif UNITY_WEBGL && !UNITY_EDITOR
            ReloadPage();
        #else
            Application.Quit();
        #endif
    }

    public bool DoesSaveExist()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        return PlayerPrefs.HasKey("saveData");
#else
        return File.Exists(path);
#endif
    }

    public void LoadAndSetupGame() 
    {
        StartCoroutine(StartUpGame());    
    }

    private IEnumerator StartUpGame()
    {
        Debug.Log("LoadAndStartup Coroutine started...");
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
        Debug.Log("LoadSaveData started...");
        GameData newGameData = null;

#if UNITY_WEBGL && !UNITY_EDITOR
        if (PlayerPrefs.HasKey("saveData"))
        {
            string json = PlayerPrefs.GetString("saveData");
            newGameData = JsonUtility.FromJson<GameData>(json);
        }
#else
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            newGameData = JsonUtility.FromJson<GameData>(json);
        }
#endif

        // Gets the saved data if there is any, then updates the other scripts.
        if (newGameData != null)
        {
            _musicVolume = newGameData.musicVolume;
            _soundEffectsVolume = newGameData.soundEffectsVolume;
            _isOneHanded = newGameData.isOneHanded;
            _revealedCassettes = newGameData.revealedCassettes;

            if (_revealedCassettes == null || _revealedCassettes.Count == 0)
            {
                _revealedCassettes.Add(_defaultCassette_Name);
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
        else // If no data is found:
        {
            if (newGameData == null && File.Exists(path))
            {
                Debug.LogWarning("No previous save data found, but file exists. Initializing defaults.");
            }
            if (newGameData != null && !File.Exists(path))
            {
                Debug.LogWarning("No previous save data found, but data exists. Initializing defaults.");
            }

            // --- FIRST‑TIME PLAYER, DEFAULTS ARE SET HERE ---
            Debug.Log("Setting up default game data...");
            StringBuilder defaultDataBuilder = new StringBuilder();

            defaultDataBuilder.AppendLine("NO SAVE DATA FOUND, SETTING UP DEFAULTS.");
            _musicVolume = _defaultSound_Volume;
            defaultDataBuilder.AppendLine($"MUSIC VOLUME:\nDefault: {_defaultSound_Volume}\nCurrent:{_musicVolume}");
            _soundEffectsVolume = _defaultSound_Volume;
            defaultDataBuilder.AppendLine($"SOUND EFFECTS VOLUME:\nDefault: {_defaultSound_Volume}\nCurrent:{_soundEffectsVolume}");
            _isOneHanded = _default_isOneHandedMode;
            defaultDataBuilder.AppendLine($"ONE HANDED MODE:\nDefault: {_default_isOneHandedMode}\nCurrent:{_isOneHanded}");
            _highScores = new Dictionary<int, string>();
            defaultDataBuilder.AppendLine($"HIGH SCORES:\nDefault: Empty\nCurrent: Empty");

            _revealedCassettes = new List<string>();
            _revealedCassettes.Add(_defaultCassette_Name);

            if (_revealedCassettes != null && _revealedCassettes.Count > 0)
            {
                defaultDataBuilder.AppendLine($"REVEALED CASSETTES:\nDefault: {_defaultCassette_Name}\nCurrent: {_revealedCassettes[0].ToString()}");
            }
            else
            {
                defaultDataBuilder.AppendLine($"REVEALED CASSETTES:\nDefault: {_defaultCassette_Name}\nCurrent: Empty or Null");
            }

            DebugEvents.AddDebugLog(defaultDataBuilder.ToString());
#if UNITY_EDITOR
            Debug.Log(defaultDataBuilder.ToString());
#endif
        }


        yield return null;
    }

    private IEnumerator SendData()
    {
        Debug.Log("SendData started...");
        SaveData_MessageBus.SetMusicVolume( _musicVolume );//connected
        SaveData_MessageBus.SetSoundEffectsVolume( _soundEffectsVolume );//connected
        SaveData_MessageBus.SetIsOneHanded( _isOneHanded );//connected
        SaveData_MessageBus.SetRevealedCassettes( _revealedCassettes );//connected
        SaveData_MessageBus.SetHighScoreDict( _highScores );//connected

        yield return null;
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
#if UNITY_WEBGL && !UNITY_EDITOR
        // On WebGL, clear PlayerPrefs and reload the page.
        if (PlayerPrefs.HasKey("saveData"))
        {
            PlayerPrefs.DeleteKey("saveData");
            PlayerPrefs.Save();
        }
        else
        {
            DebugEvents.AddDebugWarning("No PlayerPrefs key \"saveData\" found to reset.");
        }
    ReloadPage();
#else
        if (File.Exists(path))
        {
            File.Delete(path);
            Application.Quit();
        }
        else
        {
            DebugEvents.AddDebugError("No save file found to 'Reset'.");
        }
#endif
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
