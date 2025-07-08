using System.Collections;
using TMPro;
using UnityEngine;

public class Score_Manager : MonoBehaviour
{
    private int _currentScore;

    [SerializeField] float _scoreClimbSpeed = 0.001f;

    #region UI
    [SerializeField] TextMeshProUGUI _scoreText;
    #endregion

    private void OnEnable()
    {
        ScoreEvents.OnScoreIncreased += CalculateFoodOrder_Score;
    }

    private void OnDisable()
    {
        ScoreEvents.OnScoreIncreased -= CalculateFoodOrder_Score;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetUpScore();
    }

    private void SetUpScore()
    {
        _currentScore = 0;

        _scoreText.text = _currentScore.ToString("D7");
    }

    int baseScore = 1000;
    const float gracePeriod = 3f;     // seconds before score decay starts.
    const float decayRate = 25f;      // how harsh the penalty is per extra second.
    const int minScore = 100;         // minimum score.

    private void CalculateFoodOrder_Score(float timeTaken)
    { 
        float effectiveTime = Mathf.Max(0, timeTaken - gracePeriod);
        float score = baseScore - (effectiveTime * decayRate);

        Debug.Log($"Time taken: {timeTaken}\nAfter grace period/Effective Time: {timeTaken - gracePeriod} / {effectiveTime}\nEnd Score: {score}");

        IncreaseScore(Mathf.Max(minScore, Mathf.RoundToInt(score)));
    }

    private void IncreaseScore(int amount)
    {
        int previousScore = _currentScore;
        int newScore = _currentScore + amount;

        StartCoroutine(NumberClimb(previousScore, newScore));

        _currentScore = newScore;
        _scoreText.text = _currentScore.ToString("D7");
    }

    private IEnumerator NumberClimb(int previousScore, int newScore)
    {
        float duration = Mathf.Clamp((newScore - previousScore) * 0.0005f, 0.2f, 1.5f); // Shorter for small gaps, longer for big
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            int displayScore = Mathf.RoundToInt(Mathf.Lerp(previousScore, newScore, t));
            _scoreText.text = displayScore.ToString("D7");
            yield return null;
        }

        // Snap to final score
        _scoreText.text = newScore.ToString("D7");
    }
}

public static class ScoreEvents
{
    public static event System.Action<float> OnScoreIncreased;

    public static void IncreaseScore(float timeTaken_ToCompleteOrder)
    {
       OnScoreIncreased?.Invoke(timeTaken_ToCompleteOrder);
    }
}
