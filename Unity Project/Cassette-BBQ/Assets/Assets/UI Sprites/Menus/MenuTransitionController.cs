using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class MenuTransitionController : MonoBehaviour
{
    [SerializeField] Camera _mainCamera;

    [Space, Header("Main Menu Croups")]
    [SerializeField] CanvasGroup _menu_Holder_Group;
    [SerializeField] CanvasGroup _mainMenu_Group;
    [SerializeField] CanvasGroup _optionsMenu_Group;
    [SerializeField] CanvasGroup _leaderboardMenu_Group;
    [SerializeField] CanvasGroup _soundOptions_CanvasGroup;
    [SerializeField] CanvasGroup _controlsOptions_CanvasGroup;
    [SerializeField] CanvasGroup _cassettesMenu_Group;

    private CanvasGroup[] _mainMenuGroups;

    [Space, Header("BoomBox Animation")]
    [SerializeField] Image _trasitionImage;
    [SerializeField] float _timeBetweenFrames;
    [SerializeField] Sprite[] _boomBox_FlipAnim;
    [SerializeField] Sprite _boomboxHitFloor_Frame;
    [SerializeField] Sprite _boomBoxClick_Frame;

    [Space, Header("Cassette Transforms")]
    [SerializeField] Transform _cassette_One;
    [SerializeField] Transform _cassette_Two;
    [SerializeField] Transform _cassette_Three;
    [SerializeField] Transform _cassette_Four;
    [SerializeField] Transform _cassette_Five;
    [SerializeField] Transform _cassette_Six;

    [Space, Header("Game Menu Groups")]
    [SerializeField] CanvasGroup _cassettes_Background_Group;
    [SerializeField] CanvasGroup _grill_Group;
    [SerializeField] CanvasGroup _meat_Table_Group;
    [SerializeField] CanvasGroup _scoreCounter_Group;
    [SerializeField] CanvasGroup _gameOverScreen_Group;

    [Space, Header("Game Over Screen Data")]
    [SerializeField] TextMeshProUGUI _ordersCompleted_Count;
    [SerializeField] TextMeshProUGUI _foodWasted_Count;
    [SerializeField] TextMeshProUGUI _AverageOrderTime_Text;
    [SerializeField] TextMeshProUGUI _totalScore_Text;

    private void OnEnable()
    {
        MenuTransitionEvents.CassetteSelected += StartTransition;
        TimerEvents.OnTimerFinished += EnableEndScreen;
        MenuTransitionEvents.OnMenuStarted += TutorialToMenu;
    }

    private void OnDisable()
    {
        MenuTransitionEvents.CassetteSelected -= StartTransition;
        TimerEvents.OnTimerFinished -= EnableEndScreen;
        MenuTransitionEvents.OnMenuStarted -= TutorialToMenu;
    }

    private void Start()
    {
        _mainMenuGroups = new CanvasGroup[]
        {
            _mainMenu_Group,
            _menu_Holder_Group,
            _optionsMenu_Group,
            _leaderboardMenu_Group,
            _soundOptions_CanvasGroup,
            _controlsOptions_CanvasGroup,
            _cassettesMenu_Group,
            _gameOverScreen_Group
        };
    }

    #region Button Connectors
    public void MainMenu_Holder() => MoveMenuScreen(MenuScreens.MainMenu_Holder);
    public void MainMenu() => MoveMenuScreen(MenuScreens.MainMenu);
    public void OptionsMenu() => MoveMenuScreen(MenuScreens.OptionsMenu);
    public void LeaderBoardMenu() => MoveMenuScreen(MenuScreens.LeaderboardMenu);
    public void SoundOptions() => MoveMenuScreen(MenuScreens.SoundOptions);
    public void ControlsOptions() => MoveMenuScreen(MenuScreens.ControlsOptions);
    public void CassettesMenu() => MoveMenuScreen(MenuScreens.CassettesMenu);

    public void LeaveGame()
    {
        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    #endregion

    private void MoveMenuScreen(MenuScreens newScreen)
    {
        CanvasGroup ScreenToEnable;

        switch (newScreen)
        {
            case MenuScreens.MainMenu_Holder:
                ScreenToEnable = _menu_Holder_Group;
                break;
            case MenuScreens.MainMenu:
                ScreenToEnable = _mainMenu_Group;
                break;
            case MenuScreens.OptionsMenu:
                ScreenToEnable = _optionsMenu_Group;
                break;
            case MenuScreens.LeaderboardMenu:
                ScreenToEnable = _leaderboardMenu_Group;
                ScoreEvents.UpdateScoreBoard();
                break;
            case MenuScreens.SoundOptions:
                ScreenToEnable = _soundOptions_CanvasGroup;
                break;
            case MenuScreens.ControlsOptions:
                ScreenToEnable = _controlsOptions_CanvasGroup;
                break;
            case MenuScreens.CassettesMenu:
                ScreenToEnable = _cassettesMenu_Group;
                break;
            case MenuScreens.GameOverScreen:
                ScreenToEnable = _gameOverScreen_Group;
                break;
            default:
                Debug.LogError("Invalid screen type: " + newScreen);
                ScreenToEnable = null;
                break;
        }

        foreach (CanvasGroup group in _mainMenuGroups)
        {
            if (group == ScreenToEnable || group == _menu_Holder_Group)
            {
                group.alpha = 1;
                group.interactable = true;
                group.blocksRaycasts = true;
            }
            else
            {
                group.alpha = 0;
                group.interactable = false;
                group.blocksRaycasts = false;
            }
        }

    }

    private void StartTransition(Cassette_Anim_Control selectedCassette)
    {
        Debug.Log("Cassette selected: " + selectedCassette.name);

        MoveCassettesOffScreen();
    }

    private void MoveCassettesOffScreen()
    {
        float _offscreenX = 250f;
        float _moveDuration = 0.25f;

        Sequence _exitSequence;

        _exitSequence = DOTween.Sequence();

        // half‑delay so second row starts when first is 50% done,
        // etc.
        float halfDelay = _moveDuration * 0.5f;

        // Row 1: One & Two, at t = 0
        _exitSequence.Insert(0f,
            _cassette_One.DOMoveX(_cassette_One.position.x - _offscreenX, _moveDuration));
        _exitSequence.Insert(0f,
            _cassette_Two.DOMoveX(_cassette_Two.position.x + _offscreenX, _moveDuration));

        // Row 2: Three & Four, at t = halfDelay
        _exitSequence.Insert(halfDelay,
            _cassette_Three.DOMoveX(_cassette_Three.position.x - _offscreenX, _moveDuration));
        _exitSequence.Insert(halfDelay,
            _cassette_Four.DOMoveX(_cassette_Four.position.x + _offscreenX, _moveDuration));

        // Row 3: Five & Six, at t = 2 * halfDelay
        float thirdStartTime = 2f * halfDelay;
        _exitSequence.Insert(thirdStartTime,
            _cassette_Five.DOMoveX(_cassette_Five.position.x - _offscreenX, _moveDuration));
        _exitSequence.Insert(thirdStartTime,
            _cassette_Six.DOMoveX(_cassette_Six.position.x + _offscreenX, _moveDuration));

        // Fire your Transition() right when the last pair starts:
        _exitSequence.InsertCallback(thirdStartTime, () => StartTransition());

        // Finally play it
        _exitSequence.Play();

    }

    private void StartTransition()
    {
        StartCoroutine(PlayFlipAnimation_Boombox());
    }

    private IEnumerator PlayFlipAnimation_Boombox()
    {
        Sequence _transitionSequence = DOTween.Sequence();

        foreach(Sprite sprite in _boomBox_FlipAnim)
        {
            _trasitionImage.sprite = sprite;

            if (sprite == _boomboxHitFloor_Frame)
            {
                _mainCamera.transform.DOShakePosition(2f, 0.5f, 10, 70f, false, true);
                AudioEvents.PlayEffect(SoundEffects.Impact_Plate);
            }

            if (sprite == _boomBoxClick_Frame)
            {
                AudioEvents.PlayEffect(SoundEffects.Click_1);
            }

            yield return new WaitForSeconds(_timeBetweenFrames);
        }

        MoveToGamePlayScreen();
    }

    private void MoveToGamePlayScreen()
    {
        // Ensure the main menu is at the bottom of the hierarchy so it can be seen
        // above the screens that are now being activated.
        _cassettesMenu_Group.transform.SetAsLastSibling();

        foreach (CanvasGroup group in _mainMenuGroups)
        {
            if (group != _cassettesMenu_Group)
            {
                // Disable all main menu groups.
                group.alpha = 0;
                group.interactable = false;
                group.blocksRaycasts = false;
            }
        }

        // Ensure blue cassette background is gone before fade from black.
        _cassettes_Background_Group.alpha = 0;
        _cassettes_Background_Group.interactable = false;
        _cassettes_Background_Group.blocksRaycasts = false;

        _grill_Group.alpha = 1;
        _grill_Group.interactable = true;
        _grill_Group.blocksRaycasts = true;

        _meat_Table_Group.alpha = 1;
        _meat_Table_Group.interactable = true;
        _meat_Table_Group.blocksRaycasts = true;

        // Score is only visible, not intractable.
        _scoreCounter_Group.alpha = 1;
        _scoreCounter_Group.interactable = false;
        _scoreCounter_Group.blocksRaycasts = false;

        Sequence TransitionFromBlack = DOTween.Sequence();

        TransitionFromBlack.Append
        (
            _cassettesMenu_Group.DOFade(0, 3f)
        );

        AudioEvents.ChangeMusic(); // Change the music and it in for the gameplay.
        // Notify the audio manager to start grill background sound.
        AudioEvents.SetGrillScreen(true);

        TransitionFromBlack.Play().OnComplete
        (
            () =>
            {
                _cassettesMenu_Group.alpha = 0;
                _cassettesMenu_Group.interactable = false;
                _cassettesMenu_Group.blocksRaycasts = false;

                OrderEvents.StartGameSystem(); // Start the order system for the grill gameplay.

                ResetTransitionScreen();
            }
        );
    }

    private void ResetTransitionScreen()
    {
        // Reset the boombox image to the first frame. (So its invisible)
        _trasitionImage.sprite = _boomBox_FlipAnim[0]; 
    }

    private void TutorialToMenu()
    {
        MoveMenuScreen(MenuScreens.MainMenu);
    }

    private void EnableEndScreen()
    {
        // Disable all menu screens.
        MoveMenuScreen(MenuScreens.GameOverScreen);

        // Get the score data from the Score_Manager.
        Score_Manager.ScoreData scoreData = ScoreEvents.OnRequestScoreData?.Invoke() ?? new Score_Manager.ScoreData(0, 0, 0, 0);

        if (scoreData.CurrentScore == 0 && scoreData.AverageOrderCompleteTime == 0)
        {
            Debug.LogError("SOMETHING IS WRONG - ScoreData not returned correctly, values all 0");
        }

        _ordersCompleted_Count.text = scoreData.NumberOfCompletedOrders.ToString();
        _ordersCompleted_Count.alpha = 0f;
        _ordersCompleted_Count.gameObject.transform.localScale = Vector3.zero;

        _foodWasted_Count.text = scoreData.NumberOfWastedFoodItems.ToString();
        _foodWasted_Count.alpha = 0f;
        _foodWasted_Count.gameObject.transform.localScale = Vector3.zero;

        _AverageOrderTime_Text.text = MathF.Round(scoreData.AverageOrderCompleteTime).ToString() + "s";
        _AverageOrderTime_Text.alpha = 0f;
        _AverageOrderTime_Text.gameObject.transform.localScale = Vector3.zero;

        _totalScore_Text.text = scoreData.CurrentScore.ToString();
        _totalScore_Text.alpha = 0f;
        _totalScore_Text.gameObject.transform.localScale = Vector3.zero;

        Sequence scoreRevealSequence = DOTween.Sequence();

        float fadeScale_Time = 0.25f;
        float shakeDuration = 0.5f;
        float punchDuration = 0.4f;

        scoreRevealSequence.Append(_ordersCompleted_Count.DOFade(1, fadeScale_Time));
        scoreRevealSequence.Join(_ordersCompleted_Count.transform.DOScale(Vector3.one, fadeScale_Time));
        scoreRevealSequence.Join(_ordersCompleted_Count.transform.DOShakePosition(shakeDuration, strength: 70, randomness: 80, fadeOut: true));
        
        scoreRevealSequence.Append(_foodWasted_Count.DOFade(1, fadeScale_Time));
        scoreRevealSequence.Join(_foodWasted_Count.transform.DOScale(Vector3.one, fadeScale_Time));
        scoreRevealSequence.Join(_foodWasted_Count.transform.DOShakePosition(shakeDuration, strength: 70, randomness: 80, fadeOut: true));

        scoreRevealSequence.Append(_AverageOrderTime_Text.DOFade(1, fadeScale_Time));
        scoreRevealSequence.Join(_AverageOrderTime_Text.transform.DOScale(Vector3.one, fadeScale_Time));
        scoreRevealSequence.Join(_AverageOrderTime_Text.transform.DOShakePosition(shakeDuration, strength: 70, randomness: 80, fadeOut: true));

        scoreRevealSequence.Append(_totalScore_Text.DOFade(1, fadeScale_Time));
        scoreRevealSequence.Join(_totalScore_Text.transform.DOScale(Vector3.one, fadeScale_Time));
        scoreRevealSequence.Join(_totalScore_Text.transform.DOShakePosition(shakeDuration, strength: 70,randomness: 80, fadeOut: true));

        scoreRevealSequence.Append(_ordersCompleted_Count.transform.DOPunchScale(punch: new Vector2(1.1f, 1.1f), punchDuration, vibrato: 5, elasticity: 5));
        scoreRevealSequence.Append(_foodWasted_Count.transform.DOPunchScale(punch: new Vector2(1.1f, 1.1f), punchDuration, vibrato: 5, elasticity: 5));
        scoreRevealSequence.Append(_AverageOrderTime_Text.transform.DOPunchScale(punch: new Vector2(1.1f, 1.1f), punchDuration, vibrato: 5, elasticity: 5));
        scoreRevealSequence.Append(_totalScore_Text.transform.DOPunchScale(punch: new Vector2(1.1f, 1.1f), punchDuration, vibrato: 5, elasticity: 5));

        scoreRevealSequence.Play();

        // Disable meat_Table and score groups.
        _meat_Table_Group.alpha = 0;
        _meat_Table_Group.interactable = false;
        _meat_Table_Group.blocksRaycasts = false;

        _scoreCounter_Group.alpha = 0;
        _scoreCounter_Group.interactable = false;
        _scoreCounter_Group.blocksRaycasts = false;
    }

    public void EndToMainMenu()
    {
        RectTransform menuRect = _mainMenu_Group.GetComponent<RectTransform>();
        Sequence sequence = DOTween.Sequence();

        sequence.AppendCallback(() =>
        {
            _grill_Group.DOFade(0, 1f).onComplete += () =>
            {
                _grill_Group.interactable = false;
                _grill_Group.blocksRaycasts = false;
            };
            AudioEvents.SetGrillScreen(false);
        });

        sequence.AppendInterval(0.5f);

        sequence.AppendCallback(() =>
        {
            menuRect.SetAsLastSibling();
            menuRect.DOAnchorPos(new Vector2(0, 1098.66f), 1f).SetEase(Ease.InCubic);
        });

        sequence.AppendCallback(() =>
        {
            menuRect.DOAnchorPos(new Vector2(0, -80f), 1f).SetEase(Ease.OutCubic);
        });

        sequence.AppendCallback(() =>
        {
            menuRect.DOAnchorPos(new Vector2(0, 0f), 1f).SetEase(Ease.OutCubic);
        });

        sequence.Play().OnComplete(() => { AudioEvents.StartMainMenuMusic(); });

        MoveMenuScreen(MenuScreens.MainMenu);
    }

    public enum MenuScreens
    {
        MainMenu_Holder,
        MainMenu,
        OptionsMenu,
        LeaderboardMenu,
        SoundOptions,
        ControlsOptions,
        CassettesMenu,
        GameOverScreen,
    }
}

// A message broker for talking between the Cassette_Anim_Control buttons and the MenuTransitionController.
public static class MenuTransitionEvents
{
    public static event Action<Cassette_Anim_Control> CassetteSelected;

    public static void RaiseCassetteSelected(Cassette_Anim_Control selectedCassette)
    {
            CassetteSelected?.Invoke(selectedCassette);
    }

    public static event Action OnMenuStarted;

    public static void MoveToStartMenu()
    {
        OnMenuStarted?.Invoke();
    }
}

