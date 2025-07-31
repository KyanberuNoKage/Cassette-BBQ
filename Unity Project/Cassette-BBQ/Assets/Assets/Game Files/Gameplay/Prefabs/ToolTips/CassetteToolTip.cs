using CustomInspector;
using DG.Tweening;
using KyanberuGames.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class CassetteToolTip : MonoBehaviour
{
    [SerializeField] CanvasGroup _thisToolTip_Group;
    [SerializeField] RectTransform _thisToolTip_RectTransform;
    [SerializeField] TextMeshProUGUI _thisToolTip_TextBox;
    [SerializeField] Image _thisToolTip_Image;

    // Singular animation sequence to prevent overlapping animations.
    Sequence _tweenSequence;

    [Space, Header("ToolTip Scale Settings")]
    [SerializeField, Tooltip("The scale the sprite will settle to after fading in."), Range(0.001f, 2)]
    float _defaultScale = 1f;
    [SerializeField, Tooltip("The size the sprite will grow to before returning to default scale."), Range(0.001f, 2f)] 
    float _maxScale = 1.1f;
    [SerializeField, Tooltip("Time taken to fade/scale in (until max size and alpha = 1)."), Range(0.001f, 3f)]
    float _fadeInTime = 0.4f;
    [SerializeField, Tooltip("Time taken between max scale and default scale for fade in and out."), Range(0.001f, 1f)]
    float _bounceTime = 0.1f;



    private void Awake()
    {
        // Start with the ToolTip obj hidden without using tween. (Don't want to have anims playing that i can't see)
        _thisToolTip_Group.alpha = 0f;

        _tweenSequence = DOTween.Sequence();
    }

    public void Hide()
    {
        FadeAndScale(isShowing: false);
    }

    public void Show(Vector2 toolTipPosition, string toolTipText)
    {
        _thisToolTip_RectTransform.anchoredPosition = toolTipPosition;
        _thisToolTip_TextBox.text = toolTipText;

        FadeAndScale(isShowing: true);
    }

    public void Show(Vector2 toolTipPosition, Sprite toolTipTextSprite)
    {
        _thisToolTip_RectTransform.anchoredPosition = toolTipPosition;
        _thisToolTip_Image.sprite = toolTipTextSprite;
        _thisToolTip_TextBox.text = ""; // No text if using sprite.

        FadeAndScale(isShowing: true);
    }


    private Sequence CreateNewSequence()
    {
        // If theres a sequence stored already, kill and clear it.
        if (_tweenSequence != null && _tweenSequence.IsActive())
        {
            _tweenSequence.Kill();
        }

        // New sequence for the new anim.
        _tweenSequence = DOTween.Sequence().SetAutoKill(true).Pause();

        return _tweenSequence;
    }

    private void FadeAndScale(bool isShowing)
    {
        DebugEvents.AddDebugLog("ToolTip DOTween animation starting...");

        var newSequence = CreateNewSequence();

        if (isShowing)
        {
            // Fade in and scale to 1.1, then to 1, to create a 'bounce'.
            newSequence.Append(_thisToolTip_Group.DOFade(1f, _fadeInTime))
               .Join(_thisToolTip_RectTransform.DOScale(_maxScale, _fadeInTime))
               .Append(_thisToolTip_RectTransform.DOScale(_defaultScale, _bounceTime));
        }
        else
        {
            // Fade out and scale to 0. (also with a lil' bounce before it fades out)
            newSequence.Append(_thisToolTip_RectTransform.DOScale(_maxScale, _bounceTime));
            newSequence.Append(_thisToolTip_RectTransform.DOScale(0f, _fadeInTime));
            newSequence.Join(_thisToolTip_Group.DOFade(0f, _fadeInTime));
           newSequence.AppendCallback(() =>
               {
                   // Reset position and text after fade out.
                   _thisToolTip_RectTransform.anchoredPosition = Vector2.zero;
                   _thisToolTip_TextBox.text = "";
               });
        }

        newSequence.Play().OnComplete(() => DebugEvents.AddDebugLog("ToolTip animation complete."));
    }
}
