using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ScoreBoard_Manager : MonoBehaviour
{
    private SortedDictionary<int, string> _highScores = new SortedDictionary<int, string>
        (
            Comparer<int>.Create((a, b) => b.CompareTo(a))
        );
    private const int _maxEntries = 4;

    [Header("Managers")]
    [SerializeField] Score_Manager _scoreManager;
    [Space, Header("UI Elements")]
    [SerializeField] TextMeshProUGUI _highScoreText_first;
    [SerializeField] TextMeshProUGUI _highScoreText_second;
    [SerializeField] TextMeshProUGUI _highScoreText_third;
    [SerializeField] TextMeshProUGUI _highScoreText_fourth;
    [SerializeField] TextMeshProUGUI _dateTimeText_First;
    [SerializeField] TextMeshProUGUI _dateTimeText_Second;
    [SerializeField] TextMeshProUGUI _dateTimeText_Third;
    [SerializeField] TextMeshProUGUI _dateTimeText_Fourth;

    private List<TextMeshProUGUI> _scores_TableTMP = new List<TextMeshProUGUI>();
    private List<TextMeshProUGUI> _dateTimes_TableTMP = new List<TextMeshProUGUI>();


    private void OnEnable()
    {
        ScoreEvents.OnAddHighScore += AddHighScore;
        ScoreEvents.OnUpdateScoreBoard += UpdateUI;

        SaveData_MessageBus.OnRequestHighScores += ProvideScores;
        SaveData_MessageBus.OnSetHighScoreDict += SetScoreDictionary_FromSaveData;
    }

    private void SetScoreDictionary_FromSaveData(Dictionary<int, string> highScores)
    {
        _highScores.Clear();

        // Put each pair from saved dictionary into the sorted dictionary.
        foreach (var entry in highScores.OrderByDescending(e => e.Key))
        {
            _highScores.Add(entry.Key, entry.Value);
        }
    }

    private void OnDisable()
    {
        ScoreEvents.OnAddHighScore -= AddHighScore;
        ScoreEvents.OnUpdateScoreBoard -= UpdateUI;

        SaveData_MessageBus.OnRequestHighScores -= ProvideScores;
        SaveData_MessageBus.OnSetHighScoreDict -= SetScoreDictionary_FromSaveData;
    }

    private void Start()
    {
        _scores_TableTMP = new List<TextMeshProUGUI>
        {
            _highScoreText_first,
            _highScoreText_second,
            _highScoreText_third,
            _highScoreText_fourth
        };

        _dateTimes_TableTMP = new List<TextMeshProUGUI>
        {
            _dateTimeText_First,
            _dateTimeText_Second,
            _dateTimeText_Third,
            _dateTimeText_Fourth
        };
    }

    private void AddHighScore()
    {
        string timestamp = $"{DateTime.UtcNow:dd/MM/yy}\n{DateTime.UtcNow:HH:mm} UTC";

        // Add or overwrite
        _highScores[_scoreManager._currentScore] = timestamp;

        // Trim to top N
        while (_highScores.Count > _maxEntries)
            _highScores.Remove(_highScores.Keys.Last());     // last == lowest because DESC
    }

    private Dictionary<int, string> ProvideScores()
    {
        return _highScores
                .OrderByDescending(entry => entry.Key)
                .Take(4)
                .ToDictionary(entry => entry.Key, entry => entry.Value);
    }

    private void UpdateUI()
    {
        for(int i = 0; i < _scores_TableTMP.Count; i++)
        {
            if (_highScores.Count > i)
            {
                var entry = _highScores.ElementAt(i);
                _scores_TableTMP[i].text = entry.Key.ToString();
                _dateTimes_TableTMP[i].text = entry.Value;
            }
            else
            {
                _scores_TableTMP[i].text = "0000000";
                _dateTimes_TableTMP[i].text = "N/A";
            }
        }
    }
}
