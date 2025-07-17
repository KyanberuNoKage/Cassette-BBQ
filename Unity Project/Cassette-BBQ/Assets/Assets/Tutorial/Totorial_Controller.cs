using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Totorial_Controller : MonoBehaviour
{
    [Header("Tutorial Background Images")]
    [SerializeField] Image _tutorialBackgroundPanel;
    [SerializeField] Sprite[] _tutorialBackgroundSprites;

    [Space, Header("Burger Guy")]
    [SerializeField] Image _burgerGuyImage;
    [SerializeField] Sprite[] _burgerGuySprites;
    private BurgerGuyStates _burgerGuyState;

    [Space, Header("Text")]
    [SerializeField] string DebugTestString;
    [SerializeField] TextMeshProUGUI _tutorialText;
    [SerializeField] TypeText _typeText;
    TextMeshProUGUI _ScoreText; // Needed for score tutorial. Impliment later.


    private void Start()
    {
        // Type the text in the box.
        _typeText.Type(_tutorialText, DebugTestString);
    }

    private enum BurgerGuyStates
    {
        Inform,
        Chear,
        Point,
        Love,
        Fear,
        Cry,
        Shy
    }
}
