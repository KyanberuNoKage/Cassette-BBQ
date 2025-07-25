using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Order_Manager : MonoBehaviour
{
    [SerializeField] private GameObject Order_Prefab;

    [SerializeField] private List<Order_Class> ListOfOrders = new List<Order_Class>();

    [SerializeField] private Transform _orderHolder;

    [SerializeField] private bool _areOrdersActive = false;

    [SerializeField, Tooltip("Max time between order spawns.")] 
    private int _maxorder_SpawnTime = 9;

    [SerializeField, Tooltip("Min time between order spawns.")]
    private int _minOrder_SpawnTime = 3;

    [SerializeField, Tooltip("Max time between order spawns.")]
    private int _maxNumOfItems = 8;

    [SerializeField, Tooltip("Min time between order spawns.")]
    private int _minNumOfItems = 1;

    [SerializeField, Tooltip("Max number of orders at once.")]
    private int _maxNumOfOrders = 10;

    #region Order Positioning
    [Space, Header("Positioning Settings")]
    [SerializeField] private Vector2 _columnStartPosition = new Vector2(40, 16);
    [SerializeField] private RectTransform _orderSpawnPoint;
    [SerializeField] private float _spacing = 68f; // Pixels between orders
    [SerializeField] private bool _isHorizontalLayout = true;
    [SerializeField] private float _reorderDuration = 0.3f; // How long it takes to animate

    [Space, Header("Cassette Unlocks")]
    [SerializeField] private int _hotDawgUnlockThreshhold;
    [SerializeField] private int _bunVoyageUnlockThreshhold;

    private void RepositionOrders()
    {
        for (int i = 0; i < ListOfOrders.Count; i++)
        {
            Vector2 targetPos;

            if (_isHorizontalLayout)
            {
                targetPos = _columnStartPosition + new Vector2(i * _spacing, 0);
            }
            else
            {
                targetPos = _columnStartPosition + new Vector2(0, -i * _spacing);
            }

            RectTransform orderRect = ListOfOrders[i].GetComponent<RectTransform>();
            orderRect.DOAnchorPos(targetPos, _reorderDuration).SetEase(Ease.OutQuad);
        }
    }

    #endregion

    private bool AreOrderTypesRandom = true;
    private bool AreOrders_Burgers = true;

    private Coroutine _ordersCoroutine;

    private int _burgerOrdersCompleted = 0;
    private int _sausageOrdersCompleted = 0;


    private void OnEnable()
    {
        OrderEvents.OnGrillItem_Finished += Try_FulfillOrder;
        OrderEvents.RemoveOrder += SmoothlyReOrder_OrderListGame;
        OrderEvents.OnStartGame += StartGame;
        TimerEvents.OnTimerFinished += EndOrders;

        CassetteEvents.OnCassetteSelected += SetCassetteValues;
    }

    private void SetCassetteValues(CassetteGameValues newValues)
    {
        AreOrderTypesRandom = newValues.AreOrderTypesRandom;
        AreOrders_Burgers = newValues.AreOrders_Burgers;
    }

    private void OnDisable()
    {
        OrderEvents.OnGrillItem_Finished -= Try_FulfillOrder;
        OrderEvents.RemoveOrder -= SmoothlyReOrder_OrderListGame;
        OrderEvents.OnStartGame -= StartGame;
        TimerEvents.OnTimerFinished -= EndOrders;

        CassetteEvents.OnCassetteSelected -= SetCassetteValues;
    }

    private void StartGame()
    {
        _areOrdersActive = true;

        _ordersCoroutine = StartCoroutine(Start_RandomOrders());
    }

    private void Try_FulfillOrder(bool IsBurger)
    {

        if (ListOfOrders.Count <= 0)
        {
            ScoreEvents.ItemWaste_DecreaseScore();
        }
        else
        {
            bool orderFound = false;

            foreach (Order_Class order in ListOfOrders)
            {
                if (order.IsBurger == IsBurger) // If the order item type matches that of the item.
                {
                    order.AddToFilledOrder_Count(); // Fulfill an item on the order.
                    AudioEvents.PlayEffect(SoundEffects.Confirmation);
                    orderFound = true;
                    break;
                }
            }

            if (!orderFound)
            {
                ScoreEvents.ItemWaste_DecreaseScore();
            }
        }
    }

    private void SmoothlyReOrder_OrderListGame(GameObject OrderToRemove_Obj)
    {
        StartCoroutine(RemoveOrderFromList(OrderToRemove_Obj));
    }

    private IEnumerator RemoveOrderFromList(GameObject OrderToRemove_Obj)
    {
        Order_Class OrderToRemove;

        if (OrderToRemove_Obj != null)
        {
            OrderToRemove = OrderToRemove_Obj.GetComponent<Order_Class>();
        }
        else
        {
            Debug.LogError("Null reference passed into RemoveOrderFromList method in Order_Manager");
            yield break;
        }

        CheckCassetteUnlock(OrderToRemove);

        if (OrderToRemove != null && ListOfOrders.Contains(OrderToRemove))
        {
            ListOfOrders.Remove(OrderToRemove);
            Destroy(OrderToRemove_Obj);
            RepositionOrders();
        }
        _areOrdersActive = true; // Re-enable order spawning.
    }

    private void CheckCassetteUnlock(Order_Class OrderToCheck)
    {
        // For cassette Unlocks
        if (OrderToCheck.IsBurger)
        {
            _burgerOrdersCompleted++;
        }
        else if (!OrderToCheck.IsBurger)
        {
            _sausageOrdersCompleted++;
        }

        if (_burgerOrdersCompleted >= _bunVoyageUnlockThreshhold)
        {
            CassetteEvents.UnlockCassette(CassetteType.BunVoyage);
        }

        if (_sausageOrdersCompleted >= _hotDawgUnlockThreshhold)
        {
            CassetteEvents.UnlockCassette(CassetteType.HotDawg);
        }
    }

    private IEnumerator Start_RandomOrders()
    {
        _burgerOrdersCompleted = 0;
        _sausageOrdersCompleted = 0;

        while (true)
        {
            if (_areOrdersActive && ListOfOrders.Count < _maxNumOfOrders)
            {
                GameObject newOrder = Instantiate
                    (
                        Order_Prefab, 
                        _orderSpawnPoint.position, 
                        Quaternion.identity, 
                        _orderHolder
                    );

                var newOrder_Data = newOrder.GetComponent<Order_Class>();

                if (AreOrderTypesRandom)
                {
                    newOrder_Data.Set_OrderClass_Data
                    (
                        Random.Range(0, 2) == 0,
                        Random.Range
                        (
                            _minNumOfItems,
                            _maxNumOfItems + 1
                        )
                    );
                }
                else // Random between burgers and sausages.
                {
                    newOrder_Data.Set_OrderClass_Data
                    (
                        AreOrders_Burgers, // If not random, are they all burgers? (false = Sausage)
                        Random.Range
                        (
                            _minNumOfItems,
                            _maxNumOfItems + 1 // +1 ensures that the range is inclusive.
                        )
                    );
                }

                ListOfOrders.Add(newOrder_Data);
                RepositionOrders();
            }

            if (ListOfOrders.Count >= _maxNumOfOrders)
            {
                _areOrdersActive = false;

                while (ListOfOrders.Count >= _maxNumOfOrders)
                {
                    yield return new WaitForSeconds(Random.Range(1, 6));
                }

                _areOrdersActive = true;
            }

            yield return new WaitForSeconds
                (
                    Random.Range
                    (
                        _minOrder_SpawnTime, _maxorder_SpawnTime + 1
                    )
                );
        }
    }

    private void EndOrders()
    {
        _burgerOrdersCompleted = 0;
        _sausageOrdersCompleted = 0;

        if (_ordersCoroutine != null) 
        {
            StopCoroutine(_ordersCoroutine); 
        }

        StopCoroutine(Start_RandomOrders());
        _areOrdersActive = false;

        List<Order_Class> tempList = new List<Order_Class>();

        foreach (Order_Class order in ListOfOrders)
        {
            Destroy(order.gameObject);
        }

        ListOfOrders.Clear();
    }

}

public static class OrderEvents
{
    public static event Action<bool> OnGrillItem_Finished;

    public static void FillOrder(bool IsBurger) // Is burger (true), or sausage (false).
    {
        OnGrillItem_Finished?.Invoke(IsBurger);
    }

    public static event Action<GameObject> RemoveOrder;

    public static void RemoveOrderFromList(GameObject orderToRemove)
    {
        RemoveOrder?.Invoke(orderToRemove);
    }

    public static event Action OnStartGame;

    public static void StartGameSystem()
    {
        OnStartGame?.Invoke();
    }
}
