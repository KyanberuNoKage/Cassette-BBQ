using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


public class Grilling_Manager : MonoBehaviour
{
    [SerializeField] GameObject _grillPanel;
    [SerializeField] GameObject _meatTablePanel;

    private bool _isGrillingActive = true; // Player starts on grill panel.

    [SerializeField] GridLayoutGroup _grillGroup;

    public void SwitchStation()
    {
        if (_isGrillingActive)
        {
            Sequence moveSequence = DOTween.Sequence();

            moveSequence.Append(_grillPanel.transform.DOMoveX(-_grillPanel.GetComponent<RectTransform>().rect.width, 0.5f));
            //_grillPanel
        }
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            SwitchStation();
        }
    }
}
