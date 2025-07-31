using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class Data_Reset_UI_Controller : MonoBehaviour
{
    [Header("UI Group Elements")]
    [SerializeField] GameObject _receiptObj;
    RectTransform _receiptRectTransform;
    [SerializeField] CanvasGroup _receipt_CanvasGroup;

    #region UI Positions
    Vector2 startPosition = new Vector2(0, 1000);
    Vector2 midPosition = new Vector2(0, 145); 
    Vector2 endPosition = new Vector2(0, -100);
    #endregion

    [Space, Header("UI Text Elements")]
    [SerializeField] TextMeshProUGUI _highestScore_TMP;
    [SerializeField] TextMeshProUGUI _totalScore_TMP;

    [Space, Header("UI Intractable Elements")]
    [SerializeField] GameObject yesButton;
    [SerializeField] GameObject noButton;

    [Space, Header("Sprites")]
    [SerializeField] Sprite _resetData_Reciept;
    [SerializeField] Sprite _printData_Reciept;
    [SerializeField] Image _recieptImage;

    [Space, Header("Audio")]
    [SerializeField] AudioSource _printer_AudioSource;
    [SerializeField] AudioClip _printing_AudioClip;
    [SerializeField] AudioClip _tearing_AudioClip;

    private void Awake()
    {
        _receiptRectTransform = _receiptObj.GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        DataReset_UI_Events.OnChangedToPrint += ChangeToPrint_Reciept;
        DataReset_UI_Events.OnRemovePrintReceipt += RemovePrintReceipt;
    }

    private void OnDisable()
    {
        DataReset_UI_Events.OnChangedToPrint -= ChangeToPrint_Reciept;
        DataReset_UI_Events.OnRemovePrintReceipt -= RemovePrintReceipt;
    }

    public void OpenReset_UI()
    {
        int highScore = ScoreEvents.OnRequestTopScore?.Invoke() ?? 0;
        float averageTotalScore = ScoreEvents.OnRequestAverageOfScore?.Invoke() ?? 0;

        _highestScore_TMP.text = highScore.ToString();
        _totalScore_TMP.text = $"£{averageTotalScore.ToString("F2")}";

        _recieptImage.sprite = _resetData_Reciept;

        _receipt_CanvasGroup.alpha = 0f;
        _receipt_CanvasGroup.blocksRaycasts = true;

        _receipt_CanvasGroup.DOFade(1f, 1.5f).OnComplete(() =>
        {
            _receipt_CanvasGroup.alpha = 1f;
            _receipt_CanvasGroup.interactable = false;
            StartCoroutine(Receipt_Animation());
        });
    }

    private IEnumerator Receipt_Animation()
    {
        yield return null;

        Sequence moveSequence = DOTween.Sequence();
        // Ensure its position is reset.
        moveSequence.Append(_receiptRectTransform.DOAnchorPosY(startPosition.y, 0f));
        //Move it sown slowly.
        moveSequence.Append(_receiptRectTransform.DOAnchorPosY(midPosition.y, duration: 5f));
        moveSequence.JoinCallback(() => 
        {
            _printer_AudioSource.clip = _printing_AudioClip;
            _printer_AudioSource.Play();
        });
        // It gets stuck, then shakes.
        moveSequence.AppendInterval(1.5f);
        moveSequence.JoinCallback(() =>
        {
            // In case the sound is still playing when the ticket stops.
            _printer_AudioSource.DOFade(0, 0.3f).OnComplete(() => 
            { 
                _printer_AudioSource.Stop();
                _printer_AudioSource.clip = null;
            });
        });
        // Gets ripped from the machine.
        moveSequence.Append(_receiptRectTransform.DOShakeAnchorPos
            (
                duration: 0.5f,
                strength: new Vector2(5, 0),
                vibrato: 15,
                randomness: 2,
                snapping: false,
                fadeOut: false,
                ShakeRandomnessMode.Full
            ));

        moveSequence.Append(_receiptRectTransform.DOAnchorPosY(endPosition.y, 0.5f));
        moveSequence.JoinCallback(() =>
        {
            _printer_AudioSource.DOFade(1f, 0f);
            _printer_AudioSource.PlayOneShot(_tearing_AudioClip);
        });

        moveSequence.AppendInterval(0.3f);
        // Move the receipt to the center of the screen.
        moveSequence.Append(_receiptRectTransform.DOAnchorPosY(0f, 1f));

        moveSequence.Play().OnComplete(() => { _receipt_CanvasGroup.interactable = true; }); ;
    }

    private void ChangeToPrint_Reciept()
    {
        Sequence FlipReciept = DOTween.Sequence();

        FlipReciept.AppendCallback(() =>
        {
            _highestScore_TMP.DOFade(0f, 0.25f);
            _totalScore_TMP.DOFade(0f, 0.25f);
            yesButton.GetComponent<RectTransform>().DOScaleX(0f, 0.25f);
            noButton.GetComponent<RectTransform>().DOScaleX(0f, 0.25f);
        });

        FlipReciept.Join(_receiptRectTransform.DOScaleX(0, 1f));
        FlipReciept.AppendCallback(() =>
        {
            _recieptImage.sprite = _printData_Reciept;
        });

        FlipReciept.AppendCallback(() =>
        {
            yesButton.GetComponent<RectTransform>().DOScaleX(1f, 0.25f);
            noButton.GetComponent<RectTransform>().DOScaleX(1f, 0.25f);
        });

        FlipReciept.Join(_receiptRectTransform.DOScaleX(1, 1.5f));

        FlipReciept.Play();
    }

    private void RemovePrintReceipt()
    {
        Sequence moveReceipt = DOTween.Sequence();

        moveReceipt.Append(_receiptRectTransform.DOAnchorPosY(200f, 0.5f));

        moveReceipt.AppendInterval(0.05f);
        
        moveReceipt.JoinCallback(() =>
        {
            AudioEvents.PlayEffect(SoundEffects.fast_woosh);
        });

        moveReceipt.Append(_receiptRectTransform.DOAnchorPosY(-1500, 0.3f));

        moveReceipt.JoinCallback(() =>
        {
            _receipt_CanvasGroup.DOFade(0f, 1f).OnComplete(() =>
            {
                _receipt_CanvasGroup.alpha = 0f;
                _receipt_CanvasGroup.interactable = false;
                _receipt_CanvasGroup.blocksRaycasts = false;

                // Reset faded assets for next showing of Data_Reset Receipt.
                _highestScore_TMP.DOFade(1f, 0);
                _totalScore_TMP.DOFade(1f, 0);
            });
        });

        moveReceipt.Play().OnComplete(() => 
        { 
            _recieptImage.sprite = _resetData_Reciept;
        });
    }
}

public static class DataReset_UI_Events
{
    public static event Action OnDataResetClosed;

    public static void CloseDataReset()
    {
        OnDataResetClosed?.Invoke();
    }

    public static event Action OnChangedToPrint;

    public static void ChangeToPrintReceipt()
    {
        OnChangedToPrint?.Invoke();
    }

    public static event Action OnRemovePrintReceipt;

    public static void RemovePrintReceipt()
    {
        OnRemovePrintReceipt?.Invoke();
    }
}
