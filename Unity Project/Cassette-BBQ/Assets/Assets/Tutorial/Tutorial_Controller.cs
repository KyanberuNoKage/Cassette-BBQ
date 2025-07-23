using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial_Controller : MonoBehaviour
{
    [Header("Tutorial State Data")]
    [SerializeField] private TutorialPositions[] _positionsData;
    [SerializeField] private TutorialPageState _currentTutorialState;

    private Dictionary<TutorialPageState, TutorialPositions> _positionsByStep;

    public Vector2 GetBurgerGuyPosition(TutorialPageState step) => _positionsByStep[step].burgerGuyPosition;
    public Vector2 GetTextPosition(TutorialPageState step) => _positionsByStep[step].textPosition;
    public Sprite GetPageBackground(TutorialPageState step) => _positionsByStep[step].PageBackground;
    public BurgerGuyStates GetBurgerGuyState(TutorialPageState step) => _positionsByStep[step].burgerGuysState;
    public string GetPageMessage(TutorialPageState step) => _positionsByStep[step].thisPagesMessage;
    public bool IsScoreVisible(TutorialPageState step) => _positionsByStep[step].isScoreVisible;
    public Vector2 GetPressEnterTextPosition(TutorialPageState step) => _positionsByStep[step].pressEnterTextPosition;

    [Space, Header("Tutorial Background Images")]
    [SerializeField] Image _tutorialBackgroundPanel;
    [SerializeField] CanvasGroup _tutorialCanvasGroup;

    [Space, Header("Burger Guy")]
    [SerializeField] Image _burgerGuyImage;
    [SerializeField] Sprite[] _burgerGuySprites;

    [Space, Header("Text")]
    [SerializeField] string DebugTestString;
    [SerializeField] TextMeshProUGUI _tutorialText;
    [SerializeField] TypeText _typeText;
    TextMeshProUGUI _ScoreText; // Needed for score tutorial. Implement later.
    [SerializeField] TextMeshProUGUI _pressEnterText;

    bool _tutorialCompleted = false;

    private void Awake()
    {
        // convert the array set up in editor into a dictionary for easy access.
        _positionsByStep = _positionsData.ToDictionary(step => step.step);

        TutorialEvents.OnStartTutorial += StartTutorial;
        TutorialEvents.OnSkipTutorial += MoveFromTutorialToMenu;
    }

    private void OnDisable()
    {
        TutorialEvents.OnStartTutorial -= StartTutorial;
        TutorialEvents.OnSkipTutorial -= MoveFromTutorialToMenu;
    }

    private void StartTutorial()
    {
        // Set the initial state of the tutorial.
        _currentTutorialState = TutorialPageState.Start;

        // Set up the first tutorial page.
        SetUpPage();
    }

    private void Update()
    {
        // Check for input to change the tutorial text.
        if (Input.GetKeyDown(KeyCode.Return))
        {
            MoveTutorialPage();
        }
    }

    private void MoveTutorialPage()
    {
        // Get the enum values and make them into ints.
        TutorialPageState[] allStates = (TutorialPageState[])System.Enum.GetValues(typeof(TutorialPageState));

        int currentIndex = System.Array.IndexOf(allStates, _currentTutorialState);

        if (currentIndex < allStates.Length - 1)
        {
            _currentTutorialState = allStates[currentIndex + 1];
            SetUpPage();
        }
        else
        {
            // Reached the last tutorial page.
            MoveFromTutorialToMenu();
        }
    }

    private void MoveFromTutorialToMenu()
    {
        _tutorialCanvasGroup.alpha = 0f;
        _tutorialCanvasGroup.interactable = false;
        _tutorialCanvasGroup.blocksRaycasts = false;

        _tutorialCompleted = true;

        MenuTransitionEvents.MoveToStartMenu();
    }

    private void SetUpPage()
    {
        _tutorialBackgroundPanel.sprite = GetPageBackground(_currentTutorialState);

        _burgerGuyImage.sprite = GetBurgerGuySprite_ByState(GetBurgerGuyState(_currentTutorialState));

        _burgerGuyImage.rectTransform.anchoredPosition = GetBurgerGuyPosition(_currentTutorialState);

        _tutorialText.rectTransform.anchoredPosition = GetTextPosition(_currentTutorialState);

        _pressEnterText.rectTransform.anchoredPosition = GetPressEnterTextPosition(_currentTutorialState);

        // Type the text in the box.
        _typeText.Type(_tutorialText, GetPageMessage(_currentTutorialState));
    }

    private Sprite GetBurgerGuySprite_ByState(BurgerGuyStates state)
    {
        Sprite sprite;

        switch (state)
        {
            case BurgerGuyStates.Inform:
                sprite = _burgerGuySprites[0];
                break;
            case BurgerGuyStates.Chear:
                sprite = _burgerGuySprites[1];
                break;
            case BurgerGuyStates.Point:
                sprite = _burgerGuySprites[2];
                break;
            case BurgerGuyStates.Love:
                sprite = _burgerGuySprites[3];
                break;
            case BurgerGuyStates.Fear:
                sprite = _burgerGuySprites[4];
                break;
            case BurgerGuyStates.Cry:
                sprite = _burgerGuySprites[5];
                break;
            case BurgerGuyStates.Shy:
                sprite = _burgerGuySprites[6];
                break;
            default:
                sprite = _burgerGuySprites[0];
                Debug.LogWarning("Burger Guy State not found, defaulting to Inform state.");
                break;
        }

        return sprite;
    }
}

public enum TutorialPageState
{
    Start,
    Cassette,
    GrillEmpty,
    GrillScore,
    GrillOrderSausage,
    GrillOrderBurger,
    GrillTimer,
    GrillTimerHalfFull,
    RawMeatSelection,
    RawBurgerCooking,
    BurgerNeedFlipping,
    BurgerFinished,
    BurgerBurnt,
    finalPage
}

public enum BurgerGuyStates
{
    Inform,
    Chear,
    Point,
    Love,
    Fear,
    Cry,
    Shy
}

[System.Serializable]
public struct TutorialPositions
{
    public Sprite PageBackground;
    public TutorialPageState step;
    public BurgerGuyStates burgerGuysState;
    public Vector2 burgerGuyPosition;
    public Vector2 textPosition;
    public Vector2 pressEnterTextPosition;
    public string thisPagesMessage;
    public bool isScoreVisible;
}

public static class TutorialEvents
{
    public static Action OnStartTutorial;

    public static void StartTutorial()
    {
        OnStartTutorial?.Invoke();
    }

    public static Action OnSkipTutorial;

    public static void SkipTutorial()
    {
        OnSkipTutorial?.Invoke();
    }
}
