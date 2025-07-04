using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;


public class Grilling_Manager : MonoBehaviour
{
    #region Events/Actions
    private void OnEnable()
    {
        GrillingEvents.OnBurgerDestroyed += ChangeGrillItemCount;
    }

    private void OnDisable()
    {
        GrillingEvents.OnBurgerDestroyed -= ChangeGrillItemCount;
    }
    #endregion

    [SerializeField] GameObject _grillPanel;
    [SerializeField] GameObject _meatTablePanel;

    private bool _isGrillingActive = true; // Player starts on grill panel.

    [SerializeField] GridLayoutGroup _grillGridGroup;

    [SerializeField] GameObject _burgerPrefab;
    //[SerializeField] GameObject _saussagePrefab;

    [SerializeField] int _maxItemsOnGrill = 4;
    [SerializeField] int _currentItemsOnGrill = 0;


    public void SwitchStation()
    {
        if (_isGrillingActive)
        {
            Sequence moveSequence = DOTween.Sequence();

            moveSequence.Append
                (
                    _grillPanel.GetComponent<RectTransform>().DOAnchorPosX
                    (
                        2025f,
                        0.15f
                    )
                );
            moveSequence.Join
                (
                    _meatTablePanel.GetComponent<RectTransform>().DOAnchorPosX
                    (
                        150f,
                        0.15f
                    )
                );
            moveSequence.Append
                (
                    _meatTablePanel.GetComponent<RectTransform>().DOAnchorPosX
                    (
                        0f,
                        0.15f
                    )
                );

            moveSequence.Play();

            _isGrillingActive = false;
            return;
        }
        else
        {
            Sequence moveSequence = DOTween.Sequence();

            moveSequence.Append
                (
                    _grillPanel.GetComponent<RectTransform>().DOAnchorPosX
                    (
                        -110f,
                        0.15f
                    )
                );

            moveSequence.Join
                (
                    _meatTablePanel.GetComponent<RectTransform>().DOAnchorPosX
                    (
                        -1893f,
                        0.15f
                    )
                );

            moveSequence.Append
                (
                    _grillPanel.GetComponent<RectTransform>().DOAnchorPosX
                    (
                        0f,
                        0.15f
                    )
                );

            moveSequence.Play();

            _isGrillingActive = true;
            return;
        }
    }

    private void AddBurgerToGrill()
    {
        if (_currentItemsOnGrill < _maxItemsOnGrill)
        {
            ChangeGrillItemCount();
            Instantiate(_burgerPrefab, _grillGridGroup.transform, false);
        }
        else
        {
            Debug.Log
                (
                    $"No more room on grill..." +
                    $"\nMaximum items on grill: {_maxItemsOnGrill}" +
                    $"\nCurrent items on grill: {_currentItemsOnGrill}"
                );
        }
    }


    private void ChangeGrillItemCount(bool isIncrease = true)
    {
        if (isIncrease)
        {
            _maxItemsOnGrill += 1;
        }
        else
        {
            _maxItemsOnGrill -= 1;
        }
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) && _isGrillingActive)
        {
            SwitchStation();
        }
        else if (Input.GetKeyDown(KeyCode.A) && !_isGrillingActive)
        {
            Debug.Log("Can't move further left!");
        }

        if (Input.GetKeyDown(KeyCode.D) && !_isGrillingActive)
        {
            SwitchStation();
        }
        else if (Input.GetKeyDown(KeyCode.D) && _isGrillingActive)
        {
            Debug.Log("Can't move further right!");
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            AddBurgerToGrill();
        }
    }
}


public static class GrillingEvents
{
    #region Burgers
    public static event Action<bool> OnBurgerDestroyed;

    public static void BurgerDestroyed()
    {
        OnBurgerDestroyed?.Invoke(false); // Lower the grill item count.
    }
    #endregion
}
