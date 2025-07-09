using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Order_Class : MonoBehaviour
{
    #region Data
    // if not a burger, then its a sausage.
    [SerializeField, Header("Data")] 
    private bool _isBurger; public bool IsBurger => _isBurger;

    // How many of the item are needed.
    [SerializeField] private int _orderSize = 1;

    [SerializeField] private int _currentOrderSizeFuffilled = 0;

    [Space, Header("Sprites")]
    [SerializeField] GameObject _burgerSprite_Prefab;
    [SerializeField] GameObject _sausageSprite_Prefab;

    private float _spawnTime;
    private float _fulfillTime;
    #endregion

    #region UI
    [Space, Header("UI Elements")]
    [SerializeField] TextMeshProUGUI _OrderSize_UI;
    [SerializeField] Image _filledOrderSprite;
    #endregion

    public void Set_OrderClass_Data(bool isBurger = true, int orderSize = 1)
    {
        _isBurger = isBurger;
        _orderSize = orderSize;
    }

    private void Start()
    {
        if (_isBurger)
        {
            Instantiate(_burgerSprite_Prefab, transform, false);
        }
        else
        {
            Instantiate(_sausageSprite_Prefab, transform, false);
        }

        if (_currentOrderSizeFuffilled != 0)
        {
            _currentOrderSizeFuffilled = 0;
        }

        if (_orderSize <= _currentOrderSizeFuffilled)
        {
            _orderSize = 1;
            Debug.LogError("Order size required must be greater than 0, setting Order size to 1");
        }

        UpdateUI();

        _spawnTime = Time.time;
    }

    private void OnValidate()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        // Update text to show how many items are left
        if (_currentOrderSizeFuffilled < _orderSize)
        {
            _OrderSize_UI.text = $"{_orderSize - _currentOrderSizeFuffilled}";
        }
        else if (_currentOrderSizeFuffilled >= _orderSize)
        {
            Debug.LogWarning("Order Fulfilled, Destroying Order Card");

            // Prevent accidental object deletion in editor
            if (Application.isPlaying)
            {
                _fulfillTime = Time.time;
                float elapsed = _fulfillTime - _spawnTime;
                ScoreEvents.IncreaseScore_Order(elapsed);
                RemoveOrder();
            }
        }

        float fillAmount = CalculateOrderFillAmount();

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            // Direct assignment in Editor mode
            _filledOrderSprite.fillAmount = fillAmount;
            UnityEditor.EditorUtility.SetDirty(_filledOrderSprite);
        }
        else
#endif
        {
            // Tweening only at runtime
            _filledOrderSprite.DOFillAmount(fillAmount, 0.25f)
                .OnComplete(() =>
                {
                    _filledOrderSprite.fillAmount = fillAmount;
                });
        }
    }

    private void RemoveOrder()
    {
        Sequence removalSequence = DOTween.Sequence();

        removalSequence.Append
            (
                gameObject.transform.DOScale(1.1f, 0.05f)
            );

        removalSequence.Append
            (
                gameObject.transform.DOScale(0f, 0.1f)
            );

        removalSequence.Play().OnComplete
            (
                () =>
                {
                    OrderEvents.RemoveOrderFromList(gameObject);
                }
            );
    }

    private float CalculateOrderFillAmount()
    {
        return (float)_currentOrderSizeFuffilled / _orderSize;
    }

    public void AddToFilledOrder_Count()
    {
        _currentOrderSizeFuffilled++;
        
        UpdateUI();
    }
}
