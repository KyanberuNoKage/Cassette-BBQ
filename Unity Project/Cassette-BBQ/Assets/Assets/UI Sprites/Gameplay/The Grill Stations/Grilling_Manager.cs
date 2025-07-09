using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class Grilling_Manager : MonoBehaviour
{
    #region Events/Actions
    private void OnEnable()
    {
        GrillingEvents.OnGrillItemDestroyed += RemoveItemFromGrid;
        GrillingEvents.OnBurgerAdded += AddBurgerToGrill;
        GrillingEvents.OnSausageAdded += AddSausageToGrill;
    }

    private void OnDisable()
    {
        GrillingEvents.OnGrillItemDestroyed -= RemoveItemFromGrid;
        GrillingEvents.OnBurgerAdded -= AddBurgerToGrill;
        GrillingEvents.OnSausageAdded -= AddSausageToGrill;
    }
    #endregion

    [SerializeField] GameObject _grillPanel; public GameObject GrillPanel => _grillPanel;
    [SerializeField] GameObject _meatTablePanel; public GameObject MeatTablePanel => _meatTablePanel;

    // Player starts on grill panel.
    public bool _isGrillingActive { get; private set; } = true; 
    public void SetGrillingActive(bool isGrillStationActive) { _isGrillingActive = isGrillStationActive; }

    [SerializeField] GridLayoutGroup _grillGridGroup;

    [SerializeField] GameObject _burgerPrefab;
    [SerializeField] GameObject _sausagePrefab;

    [Space, Header("Grill Grid")]
    // All positions for the grill grid items to be placed in are shifted
    // between empty and full lists based on the grilling state.
    [SerializeField] List<Grill_Position> _GrillPositions;
    [SerializeField] int _numOfGrillPositions = 4; // Number of grill positions available.

    private void Start()
    {
        for(int i = 0; i < _numOfGrillPositions; i++)
        {
            // Create an empty with a rect transform component (for UI integration),
            // Then ensure it has a Grill_Position script component attached.
            Grill_Position newGrillPosition_Instance = 
                new GameObject
                (
                    $"Grill_Position{i}", 
                    typeof(RectTransform)
                )
                .AddComponent<Grill_Position>();

            newGrillPosition_Instance.transform.SetParent(_grillGridGroup.transform, false);
            _GrillPositions.Add(newGrillPosition_Instance);
        }
    }

    private void AddBurgerToGrill()
    {
        List<Grill_Position> _emptyGrillPositions = new List<Grill_Position>();

        foreach (Grill_Position position in _GrillPositions)
        {
            if (!position.IsFilled)
            {
                _emptyGrillPositions.Add(position);
            }
        }

        if (_emptyGrillPositions.Count > 0)
        {
            // Get random empty grill position.
            Grill_Position chosenPosition = _emptyGrillPositions[Random.Range(0, _emptyGrillPositions.Count)];

            // Create a burger instance at the chosen position.
            GameObject burgerInstance =
            Instantiate
                (
                    _burgerPrefab,
                    chosenPosition.transform,
                    false
                );

            // Give the Grill_Position class a reference to the new object.
            // (This sets the position to "Filled" in its class)
            chosenPosition.SetGrillItem(burgerInstance);
        }
    }

    private void AddSausageToGrill()
    {
        List<Grill_Position> _emptyGrillPositions = new List<Grill_Position>();

        foreach (Grill_Position position in _GrillPositions)
        {
            if (!position.IsFilled)
            {
                _emptyGrillPositions.Add(position);
            }
        }

        if (_emptyGrillPositions.Count > 0)
        {
            // Get random empty grill position.
            Grill_Position chosenPosition = _emptyGrillPositions[Random.Range(0, _emptyGrillPositions.Count)];

            // Create a burger instance at the chosen position.
            GameObject sausageInstance =
            Instantiate
                (
                    _sausagePrefab,
                    chosenPosition.transform,
                    false
                );

            // Give the Grill_Position class a reference to the new object.
            // (This sets the position to "Filled" in its class)
            chosenPosition.SetGrillItem(sausageInstance);
        }
    }

    private void RemoveItemFromGrid(GameObject itemToBeRemoved)
    {
        foreach (Grill_Position position in _GrillPositions)
        {
            if (position.ThisGrillItem == itemToBeRemoved)
            {
                // Clear the reference in the Grill_Position.
                // (Sets the position to empty if null is given)
                position.SetGrillItem(null);
                break; // Exit early if found.
            }
        }

        Destroy(itemToBeRemoved);
    }
}


public static class GrillingEvents
{
    public static event Action<GameObject> OnGrillItemDestroyed;

    public static void DestroyGrill_Obj(GameObject itemToBeRemoved)
    {
        OnGrillItemDestroyed?.Invoke(itemToBeRemoved); // Lower the grill item count.
    }

    public static event Action OnBurgerAdded;

    public static void AddRawBurger_ToGrill()
    {
        OnBurgerAdded?.Invoke(); // Add burger to grill
    }

    public static event Action OnSausageAdded;

    public static void AddRawSausage_ToGrill()
    {
        OnSausageAdded?.Invoke(); // Add sausage to grill
    }
}
