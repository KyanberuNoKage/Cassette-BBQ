using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class OilTimer : MonoBehaviour
{
    [SerializeField, Header("Timer UI")] Image _timerImage;
    [Space]
    [SerializeField, Header("Timer Data")] int timerDuration;
    [SerializeField] int currentTime = 0;

    private void OnEnable()
    {
        OrderEvents.OnStartOrderSystem += StartTimer;
    }

    private void OnDisable()
    {
        OrderEvents.OnStartOrderSystem -= StartTimer;
        StopAllCoroutines();
    }

    private void StartTimer()
    {
        StartCoroutine(TimerCoroutine());
    }

    private IEnumerator TimerCoroutine()
    {
        while(currentTime < timerDuration)
        {
            yield return new WaitForSeconds(1f);
            currentTime++;
            UpdateTimerUI();
        }

        // Inform others that the game timer has finished.
        TimerEvents.TimerFinished();
        ScoreEvents.AddHighScore();
    }

    private void UpdateTimerUI()
    {
        _timerImage.fillAmount = (float)currentTime / timerDuration;
    }
}

public static class TimerEvents
{
    public static event Action OnTimerFinished;

    public static void TimerFinished()
    {
        OnTimerFinished?.Invoke();
    }
}
