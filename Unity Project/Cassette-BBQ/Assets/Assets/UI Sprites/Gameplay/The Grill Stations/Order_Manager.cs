using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Order_Manager : MonoBehaviour
{
    [SerializeField] private GameObject Order_Prefab;

    [SerializeField] private List<Order_Class> ListOfOrders = new List<Order_Class>();

    [SerializeField] private Transform _orderHolder;
    [SerializeField] private GridLayoutGroup _orderHolder_GridComponent;

    [SerializeField] private bool _areOrdersActive = true;

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

    private bool _canServeFood = true;

    private void Start()
    {
        StartCoroutine(Start_RandomOrders());
    }

    private void OnEnable()
    {
        OrderEvents.OnGrillItem_Finished += Try_FulfillOrder;
        OrderEvents.RemoveOrder += SmoothlyReOrder_OrderListGame;
    }

    private void OnDisable()
    {
        OrderEvents.OnGrillItem_Finished -= Try_FulfillOrder;
        OrderEvents.RemoveOrder -= SmoothlyReOrder_OrderListGame;
    }

    private void Try_FulfillOrder(bool IsBurger)
    {

        if (ListOfOrders.Count <= 0)
        {
            Debug.Log("No Orders To Fill, Deducting Points For Food Waste");
            /**
             * NEEDS TO BE IMPLIMENTED WHEN SCORE SYSTEM IS UP AND RUNNING.
             * **/
        }
        else
        {
            foreach (Order_Class order in ListOfOrders)
            {
                if (order.IsBurger == IsBurger) // If the order item type matches that of the item.
                {
                    order.AddToFilledOrder_Count(); // Fulfill an item on the order.
                    break;
                }
            }
        }
    }

    private void SmoothlyReOrder_OrderListGame(GameObject OrderToRemove_Obj)
    {
        Debug.Log($"Object To Remove: {OrderToRemove_Obj}");
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

        if (OrderToRemove != null && ListOfOrders.Contains(OrderToRemove))
        {
            ListOfOrders.Remove(OrderToRemove);
            Destroy(OrderToRemove_Obj);
        }
        _areOrdersActive = true; // Re-enable order spawning.
    }

    private IEnumerator Start_RandomOrders()
    {
        if (_areOrdersActive && ListOfOrders.Count < _maxNumOfOrders)
        {
            GameObject newOrder = Instantiate
                (
                    Order_Prefab, 
                    _orderHolder, 
                    false
                );

            Order_Class newOrder_Data = newOrder.GetComponent<Order_Class>();

            newOrder_Data.Set_OrderClass_Data
                (
                    // Randomly choose if the order is a burger or sausage.
                    UnityEngine.Random.Range(0, 2) == 0, 
                    // +1 to ensure the max number is included in the range.
                    UnityEngine.Random.Range(_minNumOfItems, _maxNumOfItems + 1)
                );

            ListOfOrders.Add(newOrder_Data);

            yield return new WaitForSeconds
                (
                    UnityEngine.Random.Range
                    (
                        _minOrder_SpawnTime,
                        _maxorder_SpawnTime + 1 // +1 to ensure the max time is included in the range.
                    )
                );

            StartCoroutine(Start_RandomOrders()); // Repeat the process.
        }
    }

}

public static class OrderEvents
{
    public static event Action<bool> OnGrillItem_Finished;

    public static void FillOrder(bool IsBurger) // Is burger (true), or sausage ().
    {
        OnGrillItem_Finished?.Invoke(IsBurger); // Tell orders an item is finished.
    }

    public static event Action<GameObject> RemoveOrder;

    public static void RemoveOrderFromList(GameObject orderToRemove)
    {
        RemoveOrder?.Invoke(orderToRemove); // Notify that an order has been removed.
    }
}
