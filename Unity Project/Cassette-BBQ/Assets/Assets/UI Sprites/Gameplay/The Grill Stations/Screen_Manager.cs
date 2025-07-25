using DG.Tweening;
using UnityEngine;

public class Screen_Manager : MonoBehaviour
{
    [SerializeField] Grilling_Manager _grillManager;

    [SerializeField] private bool _oneHandedMode = false;

    bool _canChangeScreens = false;

    private void OnEnable()
    {
        SaveData_MessageBus.OnRequestIsOneHanded += () => _oneHandedMode;
        MenuTransitionEvents.OnEnsureGrillingScreen += EnsureOnGrillScreen; // Ensures the player starts on the grill screen.
        GamesSettingsEvents.OnOneHandedToggled += SetOneHanded;

        // Ensure the grill/meat screens cannot change until the round starts.
        OrderEvents.OnStartGame += () => _canChangeScreens = true;
        // Ensures the grill/meat screens cannot change after the round ends.
        TimerEvents.OnTimerFinished += () => _canChangeScreens = false;
    }

    private void OnDisable()
    {
        SaveData_MessageBus.OnRequestIsOneHanded -= () => _oneHandedMode;
        MenuTransitionEvents.OnEnsureGrillingScreen -= EnsureOnGrillScreen;
        GamesSettingsEvents.OnOneHandedToggled -= SetOneHanded;
    }

    private void Update()
    {
        HandleInput();
    }

    private void SetOneHanded(bool isToggledOn)
    {
        if (isToggledOn)
        {
            _oneHandedMode = true;
        }
        else
        {
            _oneHandedMode = false;
        }
    }

    private void HandleInput()
    {
        if (!_canChangeScreens) { return; }

        if (_oneHandedMode)
        {
            if (Input.GetMouseButtonDown(1)) // Right-click to toggle
            {
                SwitchStation(toGrill: !_grillManager._isGrillingActive);
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.A) && _grillManager._isGrillingActive)
            {
                SwitchStation(toGrill: false); // Go from Grill to Meat Table
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                BumpScreen(toLeft: true);
            }

            if (Input.GetKeyDown(KeyCode.D) && !_grillManager._isGrillingActive)
            {
                SwitchStation(toGrill: true); // Go from Meat Table to Grill
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                BumpScreen(toLeft: false);
            }
        }
    }

    public void SwitchStation(bool toGrill)
    {
        if (toGrill == _grillManager._isGrillingActive) return; // Already on desired station

        Sequence moveSequence = DOTween.Sequence();

        if (toGrill)
        {
            moveSequence.Append(_grillManager.GrillPanel.GetComponent<RectTransform>().DOAnchorPosX(-110f, 0.15f));
            moveSequence.Join(_grillManager.MeatTablePanel.GetComponent<RectTransform>().DOAnchorPosX(-1893f, 0.15f));
            moveSequence.Append(_grillManager.GrillPanel.GetComponent<RectTransform>().DOAnchorPosX(0f, 0.15f));
        }
        else
        {
            moveSequence.Append(_grillManager.GrillPanel.GetComponent<RectTransform>().DOAnchorPosX(2025f, 0.15f));
            moveSequence.Join(_grillManager.MeatTablePanel.GetComponent<RectTransform>().DOAnchorPosX(150f, 0.15f));
            moveSequence.Append(_grillManager.MeatTablePanel.GetComponent<RectTransform>().DOAnchorPosX(0f, 0.15f));
        }

        moveSequence.Play();
        _grillManager.SetGrillingActive(toGrill);
    }

    private void BumpScreen(bool toLeft)
    {
        Sequence bumpSequence = DOTween.Sequence();

        if (toLeft)
        {
            RectTransform meatPanel = _grillManager.MeatTablePanel.GetComponent<RectTransform>();

            bumpSequence.Append(meatPanel.DOAnchorPosX(100f, 0.1f));
            bumpSequence.Append(meatPanel.DOAnchorPosX(0f, 0.05f));
        }
        else
        {
            RectTransform grillPanel = _grillManager.GrillPanel.GetComponent<RectTransform>();
            bumpSequence.Append(grillPanel.DOAnchorPosX(-100f, 0.1f));
            bumpSequence.Append(grillPanel.DOAnchorPosX(0f, 0.05f));
        }

        bumpSequence.Play();
    }

    private void EnsureOnGrillScreen()
    {
        if (!_grillManager._isGrillingActive)
        {
            SwitchStation(toGrill: true);
        }
    }
}

