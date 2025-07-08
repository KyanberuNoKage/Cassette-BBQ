using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Order_Class : MonoBehaviour
{
    #region Data
    // if not a burger, then its a sausage.
    [SerializeField, Header("Data")] 
    private bool _isBurger;

    // How many of the item are needed.
    [SerializeField] private int _orderSize = 1;

    [SerializeField] private int _currentOrderSizeFuffilled = 0;

    [Space, Header("Sprites")]
    [SerializeField] GameObject _burgerSprite_Prefab;
    [SerializeField] GameObject _sausageSprite_Prefab;
    #endregion

    #region UI
    [Space, Header("UI Elements")]
    [SerializeField] TextMeshProUGUI _OrderSize_UI;
    [SerializeField] Image _filledOrderSprite;
    #endregion

    Order_Class(bool isBurger = true, int orderSize = 1)
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
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            _currentOrderSizeFuffilled++;

            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        // update text to show current score.
        if (_currentOrderSizeFuffilled < _orderSize)
        {
            // Update the UI to show how many items are left to fulfill the order.
            _OrderSize_UI.text = $"{_orderSize - _currentOrderSizeFuffilled}";
        }
        else if(_currentOrderSizeFuffilled >= _orderSize)
        {
            Debug.LogWarning("Order Fulfilled, Destroying Order Card");
            Destroy(gameObject);
        }

        // Update Orders fill amount.
        float fillAmount = CalculateOrderFillAmount();

        _filledOrderSprite.DOFillAmount(fillAmount, 0.25f)
            .OnComplete(() => 
            {
                _filledOrderSprite.fillAmount = fillAmount;
            });
    }

    private float CalculateOrderFillAmount()
    {
        return (float)_currentOrderSizeFuffilled / _orderSize;
    }
}
