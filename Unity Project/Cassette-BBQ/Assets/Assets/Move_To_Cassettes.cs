using UnityEngine;

public class Move_To_Cassettes : MonoBehaviour
{
    [SerializeField] CanvasGroup _mainMenu_Group;
    [SerializeField] CanvasGroup _cassettesMenu_Group;

    public void SwitchToCassettesMenu()
    {
        _cassettesMenu_Group.alpha = 1;
        _cassettesMenu_Group.interactable = true;
        _cassettesMenu_Group.blocksRaycasts = true;
        _mainMenu_Group.alpha = 0;
        _mainMenu_Group.interactable = false;
        _mainMenu_Group.blocksRaycasts = false;

        AudioEvents.FadeOutMusic(); // Fade out the music when going to cassette menu.
    }

    public void SwitchToMainMenu()
    {
        _cassettesMenu_Group.alpha = 0;
        _cassettesMenu_Group.interactable = false;
        _cassettesMenu_Group.blocksRaycasts = false;
        _mainMenu_Group.alpha = 1;
        _mainMenu_Group.interactable = true;
        _mainMenu_Group.blocksRaycasts = true;
    }
}
