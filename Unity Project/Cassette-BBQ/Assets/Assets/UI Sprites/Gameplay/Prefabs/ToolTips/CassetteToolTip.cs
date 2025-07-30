using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class CassetteToolTip : MonoBehaviour
{
    [SerializeField] CanvasGroup _thisToolTip_Group;
    [SerializeField] RectTransform _thisToolTip_RectTransform;
    [SerializeField] TextMeshProUGUI _thisToolTip_TextBox;
    [SerializeField] Image _thisToolTip_Image;


    private void Awake()
    {
        Hide();
    }

    public void Hide()
    {
        _thisToolTip_Group.alpha = 0f;
        _thisToolTip_RectTransform.anchoredPosition = Vector2.zero;
        _thisToolTip_TextBox.text = "";
    }

    /// <summary>
    /// Shows the tooltip at position given, with a given offset.
    /// </summary>
    /// <param name="itemPosition"></param>
    /// <param name="offset"></param>
    public void Show(Vector2 toolTipPosition, string toolTipText)
    {
        _thisToolTip_RectTransform.anchoredPosition = toolTipPosition;
        _thisToolTip_TextBox.text = toolTipText;
        _thisToolTip_Group.alpha = 1f;
    }

    public void Show(Vector2 toolTipPosition, Sprite toolTipTextSprite)
    {
        _thisToolTip_RectTransform.anchoredPosition = toolTipPosition;
        _thisToolTip_Image.sprite = toolTipTextSprite;
        _thisToolTip_TextBox.text = "";
        _thisToolTip_Group.alpha = 1f;
    }
}
