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

    [SerializeField] int _debugScoreToIncrease;

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

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            // For testing purposes, increase score by 1000 when space is pressed
            IncreaseScore(_debugScoreToIncrease);
        }
    }
}
