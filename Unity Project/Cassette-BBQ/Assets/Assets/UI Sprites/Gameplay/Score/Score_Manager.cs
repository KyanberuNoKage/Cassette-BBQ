using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class Score_Manager : MonoBehaviour
{
    private int _currentScore;

    [Header("Score Increase Variables")]
    // For Order Score Calculation
    [SerializeField, Tooltip("Base score for finishing an order, decreases by decayRate over time.")]
    int baseScore = 2000;
    [SerializeField, Tooltip("Grace period before the decay begins, to ensure score is somewhat obtainable")] 
    const float gracePeriod = 3f;
    [SerializeField, Tooltip("How harsh the penalty is over time for not completing an order.")]
    const float decayRate = 25f;
    [SerializeField, Tooltip("Minimum score that can be awarded, for if the order is completed very late.")]
    const int minScore = 100;

    [Space, Header("Score Decrease Variables")]
    [SerializeField, Tooltip("How much score is lost on wasted food.")]
    private int _foodWastePenalty = 250;
    [SerializeField,Tooltip("Number of times player has waisted food.")]
    private int _foodWasteCount = 0;


    #region UI
    [SerializeField] TextMeshProUGUI _scoreText;

    [SerializeField] float _scoreClimbSpeed = 0.001f;

    private Tween _scoreShakeTween;
    #endregion

    private void OnEnable()
    {
        ScoreEvents.OnOrder_ScoreIncreased += CalculateFoodOrder_Score;
        ScoreEvents.OnFoodWasted_ScoreDecreased += CalculateFoodWaste_Score;
    }

    private void OnDisable()
    {
        ScoreEvents.OnOrder_ScoreIncreased -= CalculateFoodOrder_Score;
        ScoreEvents.OnFoodWasted_ScoreDecreased -= CalculateFoodWaste_Score;
    }

    void Start()
    {
        SetUpScore();
    }

    private void SetUpScore()
    {
        _currentScore = 0;

        _scoreText.text = _currentScore.ToString("D4");
    }

    private void CalculateFoodOrder_Score(float timeTaken)
    { 
        float effectiveTime = Mathf.Max(0, timeTaken - gracePeriod);
        float score = baseScore - (effectiveTime * decayRate);

        Debug.Log($"Time taken: {timeTaken}\nAfter grace period/Effective Time: {timeTaken - gracePeriod} / {effectiveTime}\nEnd Score: {score}");

        IncreaseScore(Mathf.Max(minScore, Mathf.RoundToInt(score)), false);
    }

    private void CalculateFoodWaste_Score()
    {
        _foodWasteCount++;
        int scoreDecrease = _foodWastePenalty * _foodWasteCount;

        IncreaseScore(scoreDecrease, true);
    }

    private void IncreaseScore(int amount, bool isDecrease)
    {
        int previousScore = _currentScore;

        if (isDecrease)
        {
            _currentScore -= amount;
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
}
