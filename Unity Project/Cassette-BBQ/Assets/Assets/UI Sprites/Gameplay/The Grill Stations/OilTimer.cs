using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class OilTimer : MonoBehaviour
{
    [SerializeField, Header("Timer UI")] Image _timerImage;
    [Space]
    [Header("Timer Data")] 
    [SerializeField] int timerDuration; public void SetTimerDuration(int value) { timerDuration = value; }
    [SerializeField] int currentTime = 0;

    private void OnEnable()
    {
        OrderEvents.OnStartGame += StartTimer;
        CassetteEvents.OnCassetteSelected += SetCassetteValues;
    }

    private void OnDisable()
    {
        OrderEvents.OnStartGame -= StartTimer;
        CassetteEvents.OnCassetteSelected -= SetCassetteValues;
        StopAllCoroutines();
    }

    private void StartTimer()
    {
        ResetTimer();// reset before starting a new timer.

        StartCoroutine(TimerCoroutine());
    }

    private void ResetTimer()
    {
        StopAllCoroutines();
        currentTime = 0;
        UpdateTimerUI();
    }

    private void SetCassetteValues(CassetteGameValues newValues)
    {
        SetTimerDuration(newValues.TimerDuration);
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
