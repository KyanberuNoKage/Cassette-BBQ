using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using KyanberuGames.Utilities;

public class Score_Manager : MonoBehaviour
{
    [SerializeField] public int _currentScore { get;  private set; }

    [Header("Score Increase Variables")]
    // For Order Score Calculation
    [SerializeField, Tooltip("Base score for finishing an order, decreases by decayRate over time.")]
    int baseScore = 2000;
    [SerializeField, Tooltip("Grace period before the decay begins, to ensure score is somewhat obtainable")] 
    float gracePeriod = 3f;
    [SerializeField, Tooltip("How harsh the penalty is over time for not completing an order.")]
    float decayRate = 25f;
    [SerializeField, Tooltip("Minimum score that can be awarded, for if the order is completed very late.")]
    const int minScore = 100;

    [Space, Header("Score Decrease Variables")]
    [SerializeField, Tooltip("How much score is lost on wasted food.")]
    private int _foodWastePenalty = 250;

    [Space, Header("Score Info")]
    [SerializeField, Tooltip("Number of times player has waisted food.")] 
                     int _numberOfWastedFoodItems = 0;
    [SerializeField] int _numberOfCompletedOrders = 0;
    [SerializeField] float _averageTimeTaken_PerOrder = 0;
    [SerializeField] List<float> _listOf_timeTaken_PerOrder = new List<float>();

    private bool _isDOubleOrNothingActive = false;

    // UNLOCK BOOLS
    bool isRushHour_Unlocked = false;
    bool isSlowShift_Unlocked = false;
    bool isDoubleOrNothing_Unlocked = false;

    #region UI
    [SerializeField] TextMeshProUGUI _scoreText;

    private Tween _scoreShakeTween;
    #endregion

    private void OnEnable()
    {
        ScoreEvents.OnOrder_ScoreIncreased += CalculateFoodOrder_Score;
        ScoreEvents.OnFoodWasted_ScoreDecreased += CalculateFoodWaste_Score;
        ScoreEvents.OnRequestScoreData += ReturnScoreData;
        OrderEvents.OnStartGame += ResetScore;
        // Removes visual score for when next round starts without resetting the score data that needs to be used.
        TimerEvents.OnTimerFinished += ResetScore_VisualOnly; 

        CassetteEvents.OnCassetteSelected += SetCassetteValues;
    }

    private void SetCassetteValues(CassetteGameValues newValues)
    {
        baseScore = newValues.OrderScore_BaseValue;
        decayRate = newValues.OrderScore_DecayRate;
        _foodWastePenalty = newValues.WastedFoodPenalty;
        _isDOubleOrNothingActive = newValues.IsDoubleOrNothing;
    }

    private void OnDisable()
    {
        ScoreEvents.OnOrder_ScoreIncreased -= CalculateFoodOrder_Score;
        ScoreEvents.OnFoodWasted_ScoreDecreased -= CalculateFoodWaste_Score;
        ScoreEvents.OnRequestScoreData -= ReturnScoreData;
        OrderEvents.OnStartGame -= ResetScore;

        TimerEvents.OnTimerFinished -= ResetScore_VisualOnly;

        CassetteEvents.OnCassetteSelected -= SetCassetteValues;
    }

    void Start()
    {
        SetUpScore();
    }

    private void CheckCassetteUnlocks()
    {
        // Whenever the score changes, check if cassette should be unlocked.
        if (!isRushHour_Unlocked && _currentScore >= 19000)
        {
            CassetteEvents.UnlockCassette(CassetteType.RushHour);
            isRushHour_Unlocked = true;
        }

        if (!isSlowShift_Unlocked && _currentScore >= 23000)
        {
            CassetteEvents.UnlockCassette(CassetteType.SlowShift);
            isSlowShift_Unlocked = true;
        }

        if (!isDoubleOrNothing_Unlocked && _currentScore >= 30000)
        {
            CassetteEvents.UnlockCassette(CassetteType.DoubleOrNothing);
            isDoubleOrNothing_Unlocked = true;
        }
    }

    private void SetUpScore()
    {
        _currentScore = 0;

        _scoreText.text = _currentScore.ToString("D4");
    }

    private void ResetScore()
    {
        _currentScore = 0;
        _scoreText.text = _currentScore.ToString("D4");
        _numberOfCompletedOrders = 0;
        _numberOfWastedFoodItems = 0;
        _averageTimeTaken_PerOrder = 0;
        _listOf_timeTaken_PerOrder.Clear();

        // Ensures round-only unlock checks are reset.
        isRushHour_Unlocked = false;
        isSlowShift_Unlocked = false;
        isDoubleOrNothing_Unlocked = false;
    }

    private void ResetScore_VisualOnly()
    {
        _scoreText.text = "0000";
    }

    private void CalculateFoodOrder_Score(float timeTaken)
    { 
        _listOf_timeTaken_PerOrder.Add(timeTaken);
        _numberOfCompletedOrders++;

        float effectiveTime = Mathf.Max(0, timeTaken - gracePeriod);
        float score = baseScore - (effectiveTime * decayRate);

        DebugEvents.AddDebugLog($"Food Order Score Calculation:\nTime taken: {timeTaken}\nAfter grace period/Effective Time: {timeTaken - gracePeriod} / {effectiveTime}\nEnd Score: {score}");

        int finalScore = Mathf.Max(minScore, Mathf.RoundToInt(score));

        IncreaseScore(finalScore, isDecrease: false);
    }

    private void CalculateFoodWaste_Score()
    {
        _numberOfWastedFoodItems++;

        if (_isDOubleOrNothingActive)
        {

            // Resets the timer, resets the score and ends the game.
            _currentScore = 0;
            TimerEvents.OrNothing();

            // Stops method early due to round ending.
            return;
        }

        int scoreDecrease = _foodWastePenalty * _numberOfWastedFoodItems;

        IncreaseScore(scoreDecrease, isDecrease: true);
    }

    private void IncreaseScore(int amount, bool isDecrease)
    {
        int previousScore = _currentScore;

        if (isDecrease)
        {
            _currentScore -= amount;
            AudioEvents.PlayEffect(SoundEffects.Error_6);
            StartScoreShake(_currentScore - previousScore);
        }
        else
        {
            _currentScore += amount;
            StartScorePulse();
        }         
            
        StartCoroutine(NumberClimb(previousScore, _currentScore, isDecrease));
    }

    private IEnumerator NumberClimb(int from, int to, bool IsDecrease)
    {
        float duration = Mathf.Clamp(Mathf.Abs(to - from) * 0.0005f, 0.2f, 1.5f);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            int displayValue = Mathf.RoundToInt(Mathf.Lerp(from, to, t));
            _scoreText.text = displayValue.ToString("D4");
            yield return null;
        }

        _scoreText.text = to.ToString("D4");

        CheckCassetteUnlocks();

        if (IsDecrease)
        {
            StopScoreShake();
        }
        else
        {
            StopScorePulse();
        }  
    }

    private Vector2 _originalScorePos;

    private void StartScoreShake(int scoreDifference)
    {
        float shakeStrength = Mathf.Clamp(Mathf.Abs(scoreDifference) * 0.05f, 1f, 10f);

        // Store the original position before shaking
        _originalScorePos = _scoreText.rectTransform.anchoredPosition;

        _scoreShakeTween?.Kill();
        _scoreShakeTween = _scoreText.rectTransform
            .DOShakeAnchorPos(
                duration: 10f,
                strength: shakeStrength,
                vibrato: 20,
                randomness: 60,
                snapping: false,
                fadeOut: false)
            .SetLoops(-1, LoopType.Restart);
    }

    private void StopScoreShake()
    {
        _scoreShakeTween?.Kill();
        _scoreShakeTween = null;
        // Restore the original anchored position
        _scoreText.rectTransform.anchoredPosition = _originalScorePos;
    }

    private void StartScorePulse()
    {
        _scoreText.rectTransform.DOKill();
        _scoreText.rectTransform.DOScale(new Vector3(1.1f, 1.1f, 1), 0.15f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    // Call this to stop the pulse and reset scale
    private void StopScorePulse()
    {
        _scoreText.rectTransform.DOKill(); // stop all tweens on the rectTransform
        _scoreText.rectTransform.localScale = Vector3.one; // reset scale
    }

    public class ScoreData
    {
        public int NumberOfWastedFoodItems;
        public int NumberOfCompletedOrders;
        public float AverageOrderCompleteTime;
        public int CurrentScore;

        public ScoreData(int wastedFoodItems, int completedOrders, float averageTime, int currentScore)
        {
            NumberOfWastedFoodItems = wastedFoodItems;
            NumberOfCompletedOrders = completedOrders;
            AverageOrderCompleteTime = averageTime;
            CurrentScore = currentScore;
        }
    }

    private ScoreData ReturnScoreData()
    {
        float average = 0;

        foreach (float time in _listOf_timeTaken_PerOrder)
        {
            average += time;
        }

        _averageTimeTaken_PerOrder = average / _listOf_timeTaken_PerOrder.Count;

        ScoreData scoreData = new ScoreData
        (
            _numberOfWastedFoodItems,
            _numberOfCompletedOrders,
            _averageTimeTaken_PerOrder,
            _currentScore
        );

        return scoreData;
    }
}

public static class ScoreEvents
{
    public static event System.Action<float> OnOrder_ScoreIncreased;

    public static void IncreaseScore_Order(float timeTaken_ToCompleteOrder)
    {
       OnOrder_ScoreIncreased?.Invoke(timeTaken_ToCompleteOrder);
    }

    public static event System.Action OnFoodWasted_ScoreDecreased;

    public static void ItemWaste_DecreaseScore()
    {
        OnFoodWasted_ScoreDecreased?.Invoke();
    }

    public static event Action OnAddHighScore;

    public static void AddHighScore()
    {
        OnAddHighScore?.Invoke();
    }

    public static event Action OnUpdateScoreBoard;

    public static void UpdateScoreBoard()
    {
        OnUpdateScoreBoard?.Invoke();
    }

    public static Func<Score_Manager.ScoreData> OnRequestScoreData;

    public static Func<int> OnRequestTopScore;
    public static Func<float> OnRequestAverageOfScore;
}
