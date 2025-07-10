using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class MenuTransitionController : MonoBehaviour
{
    [SerializeField] Animator _trasitionAnimator;
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

    private void OnEnable()
    {
        TransitionEvents.CassetteSelected += StartTransition;
    }

    private void OnDisable()
    {
        TransitionEvents.CassetteSelected -= StartTransition;
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
            _cassettesMenu_Group
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

    public void ResetData()
    {
        // IMPLIMENT RESET DATA FEATURES
        Debug.Log("Resetting game data...");
    }

    public void LeaveGame()
    {
        // IMPLIMENT SAVE GAME FEATURES
        Application.Quit();
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
        Sequence _transitionSequence = DOTween.Sequence();
        _transitionSequence.SetDelay(0.15f); // Delay before the transition starts to let the animation play.
        _transitionSequence.Append
        (
            _mainCamera.transform.DOShakePosition(3f, 0.5f, 10, 70f, false, true)
        );
        _trasitionAnimator.SetTrigger("BoomBox"); // Starts the "BoomBox" transition animation.
        _transitionSequence.Play()
            .OnComplete(() =>
            {
                StartCoroutine(WaitForAnimation(_trasitionAnimator, "BoomBox"));
            });
    }

    private IEnumerator WaitForAnimation(Animator animator, string stateName)
    {
        // Wait until the animation starts playing
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
            yield return null;

        // Wait until it's done playing
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.98f)
            yield return null;

        MoveToGamePlayScreen();
    }


    private void MoveToGamePlayScreen()
    {
        // Ensure the main menu is at the bottom of the hierarchy so it can be seen
        // above the screens that are now being activated.
        _cassettesMenu_Group.transform.SetAsLastSibling();

        _menu_Holder_Group.alpha = 0;
        _menu_Holder_Group.interactable = false;
        _menu_Holder_Group.blocksRaycasts = false;

        _grill_Group.alpha = 1;
        _grill_Group.interactable = true;
        _grill_Group.blocksRaycasts = true;

        _meat_Table_Group.alpha = 1;
        _meat_Table_Group.interactable = true;
        _meat_Table_Group.blocksRaycasts = true;

        // Ensures the blue background is removed before the fade.
        _cassettes_Background_Group.alpha = 0;
        _cassettes_Background_Group.interactable = false;
        _cassettes_Background_Group.blocksRaycasts = false;

        Sequence TransitionFromBlack = DOTween.Sequence();

        TransitionFromBlack.Append
        (
            _cassettesMenu_Group.DOFade(0, 3f)
        );

        TransitionFromBlack.Play().OnComplete
        (
            () =>
            {
                _cassettesMenu_Group.alpha = 0;
                _cassettesMenu_Group.interactable = false;
                _cassettesMenu_Group.blocksRaycasts = false;

                OrderEvents.StartOrderSystem(); // Start the order system for the grill gameplay.
            }
        );
    }

    public enum MenuScreens
    {
        MainMenu_Holder,
        MainMenu,
        OptionsMenu,
        LeaderboardMenu,
        SoundOptions,
        ControlsOptions,
        CassettesMenu
    }
}

    // A message broker for talking between the Cassette_Anim_Control buttons and the MenuTransitionController.
    public static class TransitionEvents
    {
        public static event Action<Cassette_Anim_Control> CassetteSelected;

        public static void RaiseCassetteSelected(Cassette_Anim_Control selectedCassette)
        {
            CassetteSelected?.Invoke(selectedCassette);
        }
    }

