using System.Xml.Serialization;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] CanvasGroup _pauseMenu_Holder_CanvasGroup;
    [SerializeField] CanvasGroup _pauseMenu_CanvasGroup;
    [SerializeField] CanvasGroup _optionsMenu_CanvasGroup;
    [SerializeField] CanvasGroup _soundOptions_CanvasGroup;
    [SerializeField] CanvasGroup _controlsOptions_CanvasGroup;

    CanvasGroup[] _allCanvasGroups;

    [SerializeField] private bool _isGamePlaying = false; 
    public void SetIsGamePlaying_False() { _isGamePlaying = false; }
    public void SetIsGamePlaying_True() { _isGamePlaying = true; }

    bool _isMenuOn = false;

    #region Events
    private void OnEnable()
    {
        OrderEvents.OnStartGame += SetIsGamePlaying_True;
        TimerEvents.OnTimerFinished += SetIsGamePlaying_False;
    }

    private void OnDisable()
    {
        OrderEvents.OnStartGame -= SetIsGamePlaying_True;
        TimerEvents.OnTimerFinished -= SetIsGamePlaying_False;
    }
    #endregion

    private enum MenuType
    {
        PauseMenu,
        OptionsMenu,
        SoundOptions,
        ControlsOptions
    }

    private void Start()
    {
        // Holds all canvas groups (Except main holder) for easy access.
        _allCanvasGroups = new CanvasGroup[]
        {
            _pauseMenu_CanvasGroup,
            _optionsMenu_CanvasGroup,
            _soundOptions_CanvasGroup,
            _controlsOptions_CanvasGroup
        };

        _isMenuOn = false;
        CheckShowMenu();
    }

    private void Update()
    {
        if (_isGamePlaying && Input.GetKeyDown(KeyCode.Escape))
        {
            _isMenuOn = !_isMenuOn;
            CheckShowMenu();
        }
    }

    private void CheckShowMenu()
    {
        if (_isMenuOn)
        {
            ShowPauseMenu_Holder();

            Time.timeScale = 0f; // Pause the game time.
        }
        else
        {
            HidePauseMenu_Holder();

            Time.timeScale = 1f; // Resume the game time.
        }
    }

    public void UnPauseGame()
    {
        _isMenuOn = false;
        HideAll_SubMenus();
        CheckShowMenu();
    }

    private void ShowPauseMenu_Holder()
    {
        // Show the holder.
        _pauseMenu_Holder_CanvasGroup.alpha = 1f;
        _pauseMenu_Holder_CanvasGroup.interactable = true;
        _pauseMenu_Holder_CanvasGroup.blocksRaycasts = true;
        // Show the first pause menu.
        _pauseMenu_CanvasGroup.alpha = 1f;
        _pauseMenu_CanvasGroup.interactable = true;
        _pauseMenu_CanvasGroup.blocksRaycasts = true;
    }

    private void HidePauseMenu_Holder()
    {
        // Hide all sub-menus.
        HideAll_SubMenus();
        // Hide the main holder.
        _pauseMenu_Holder_CanvasGroup.alpha = 0f;
        _pauseMenu_Holder_CanvasGroup.interactable = false;
        _pauseMenu_Holder_CanvasGroup.blocksRaycasts = false;
    }

    #region Button Methods
    public void ShowPauseMenu() { ShowByType(MenuType.PauseMenu); }
    public void ShowOptionsMenu() { ShowByType(MenuType.OptionsMenu); }
    public void ShowOptionsMenu_Controls() { ShowByType(MenuType.ControlsOptions); }
    public void ShowOptionsMenu_Sound() { ShowByType(MenuType.SoundOptions); }
    #endregion

    private void HideAll_SubMenus()
    {
        foreach(CanvasGroup group in _allCanvasGroups)
        {
            group.alpha = 0f;
            group.interactable = false;
            group.blocksRaycasts = false;
        }
    }

    private void ShowByType(MenuType type)
    {
        // First hide all sub-menus.
        HideAll_SubMenus();

        // Then show specified menu by the type given.
        switch (type)
        {
            case MenuType.PauseMenu:
                // Show Pause Menu
                _pauseMenu_CanvasGroup.alpha = 1f;
                _pauseMenu_CanvasGroup.interactable = true;
                _pauseMenu_CanvasGroup.blocksRaycasts = true;
                break;
            case MenuType.OptionsMenu:
                // Show Options Menu
                _optionsMenu_CanvasGroup.alpha = 1f;
                _optionsMenu_CanvasGroup.interactable = true;
                _optionsMenu_CanvasGroup.blocksRaycasts = true;
                break;
            case MenuType.ControlsOptions:
                // Show Controls Menu
                _controlsOptions_CanvasGroup.alpha = 1f;
                _controlsOptions_CanvasGroup.interactable = true;
                _controlsOptions_CanvasGroup.blocksRaycasts = true;
                break;
            case MenuType.SoundOptions:
                // Show Sound Menu
                _soundOptions_CanvasGroup.alpha = 1f;
                _soundOptions_CanvasGroup.interactable = true;
                _soundOptions_CanvasGroup.blocksRaycasts = true;
                break;
            default:
                // Show Pause Menu
                _pauseMenu_Holder_CanvasGroup.alpha = 1f;
                _pauseMenu_Holder_CanvasGroup.interactable = true;
                _pauseMenu_Holder_CanvasGroup.blocksRaycasts = true;
                break;
        }
    }
}
