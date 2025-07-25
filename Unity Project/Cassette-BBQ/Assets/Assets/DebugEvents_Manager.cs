// Ignore Spelling: Kyanberu

using CustomInspector;
using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace KyanberuGames.Utilities
{
    public class DebugEvents_Manager : MonoBehaviour
    {
        #region Unity Editor Inspector
        #pragma warning disable CS0414 // Suppress: Field assigned but never used (its 'used' by CustomInspector Button)
        [SerializeField, Button(nameof(PrintPersistentDebugLogPath), tooltip = "Prints the persistent path to the debug_log Markdown file to the console and the log.")]
        [HideField] bool _printPersistentDataPath = false;
        #pragma warning restore CS0414

        private void PrintPersistentDebugLogPath()
        {
            Debug.Log($"Persistent Debug-Log Path: \n{_logPath}");
            DebugEvents.AddDebugLog($"Current Persistent Debug-Log Path\n({_logPath})\nwas printed to the unity debug console.");
        }
        #endregion

        #region Log Set-Up
        string _logPath;

        private void Awake()
        {
            _logPath = Path.Combine(Application.persistentDataPath, "/debug_log.md");

            StringBuilder _logHeader_Builder = new StringBuilder();

            _logHeader_Builder.AppendLine("- Debug Log Initialized -");
            _logHeader_Builder.AppendLine($"Time: [{DateTime.UtcNow.ToString("HH:mm:ss")} - UTC]");
            _logHeader_Builder.AppendLine($"Date: [{DateTime.UtcNow.ToString("yyyy-MM-dd")}]");
            _logHeader_Builder.AppendLine($"Unity Version: {Application.unityVersion}");
            _logHeader_Builder.AppendLine($"Version: {Application.version}");
            _logHeader_Builder.AppendLine($"---");
            _logHeader_Builder.AppendLine($"OS: {SystemInfo.operatingSystem}");
            _logHeader_Builder.AppendLine($"Platform: {Application.platform}");
            _logHeader_Builder.AppendLine($"CPU: {SystemInfo.processorType} ({SystemInfo.processorCount} cores)");
            _logHeader_Builder.AppendLine($"RAM: {SystemInfo.systemMemorySize} MB");
            _logHeader_Builder.AppendLine($"GPU: {SystemInfo.graphicsDeviceName} ({SystemInfo.graphicsMemorySize} MB)");
            _logHeader_Builder.AppendLine($"---");
            _logHeader_Builder.AppendLine($"Screen Resolution: {Screen.currentResolution.width}x{Screen.currentResolution.height} @ {Screen.currentResolution.refreshRateRatio.value:F4}Hz");
            _logHeader_Builder.AppendLine($"Device Model: {SystemInfo.deviceModel}");

            // Write initial log to file.
            System.IO.File.WriteAllText(_logPath, _logHeader_Builder.ToString());

            AddToLog("The game has started!");
        }

        private void OnEnable()
        {
            DebugEvents.OnDebugLogAdded += AddToLog;
            DebugEvents.OnDebugErrorAdded += AddErrorToLog;
            DebugEvents.OnDebugWarningAdded += AddWarningLog;

            Application.logMessageReceived += HandleUnityLog;
        }

        private void OnDisable()
        {
            DebugEvents.OnDebugLogAdded -= AddToLog;
            DebugEvents.OnDebugErrorAdded -= AddErrorToLog;
            DebugEvents.OnDebugWarningAdded -= AddWarningLog;

            Application.logMessageReceived -= HandleUnityLog;
        }
        #endregion

        #region Log Methods
        /// <summary>
        /// Add a log entry to the debug log file.
        /// </summary>
        /// <param name="log"></param>
        private void AddToLog(string log)
        {
            File.AppendAllText
                (
                    _logPath,
                    $"\n---\n\n[{System.DateTime.UtcNow}] - LOG\n{log}"
                );
        }

        /// <summary>
        /// Add a warning entry to the debug log file.
        /// </summary>
        /// <param name="warning"></param>
        private void AddWarningLog(string warning)
        {
            File.AppendAllText
                (
                    _logPath,
                    $"\n---\n\n[{System.DateTime.UtcNow}] - WARNING\n{warning}"
                );
        }

        /// <summary>
        /// Add an error entry to the debug log file.
        /// </summary>
        /// <param name="error"></param>
        private void AddErrorToLog(string error)
        {
            File.AppendAllText
                (
                    _logPath, 
                    $"\n---\n\n[{System.DateTime.UtcNow}] - ERROR!\n{error}"
                );
        }

        private void OnApplicationQuit()
        {
            File.AppendAllText
                (
                    _logPath, 
                    $"\n---\n\n[{System.DateTime.UtcNow}] - LOG\nThe game has ended!"
                );
        }

        void HandleUnityLog(string condition, string stackTrace, LogType type)
        {
            AddToLog($"[{type}] {condition}\n{stackTrace}");
        }
        #endregion
    }

    #region Log Messenger
    /// <summary>
    /// Static class for managing debug events and calling them from other classes.
    /// </summary>
    public static class DebugEvents
    {
        #region Events
        public static event Action<string> OnDebugLogAdded;

        public static event Action<string> OnDebugWarningAdded;

        public static event Action<string> OnDebugErrorAdded;
        #endregion

        #region Event Calls
        public static void AddDebugLog(string log)
        {
            OnDebugLogAdded?.Invoke(log);
        }

        public static void AddDebugWarning(string warning)
        {
            OnDebugWarningAdded?.Invoke(warning);
        }

        public static void AddDebugError(string error)
        {
            OnDebugErrorAdded?.Invoke(error);
        }
        #endregion
    }
    #endregion
}

