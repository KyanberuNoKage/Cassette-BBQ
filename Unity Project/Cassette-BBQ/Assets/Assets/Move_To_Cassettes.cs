using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Move_To_Cassettes : MonoBehaviour
{
    [SerializeField] CanvasGroup _mainMenu_Group;
    [SerializeField] CanvasGroup _cassettesMenu_Group;
    [SerializeField] CanvasGroup _casetteBackground;
    [SerializeField] Animator _trasitionAnimator;

    [SerializeField] RectTransform[] _cassetteObjects;
    [SerializeField] Vector3[] _cassetteStartingPositions;

    private void OnEnable()
    {
        TimerEvents.OnTimerFinished += ResetCassette_Screen;
    }

    private void OnDisable()
    {
        TimerEvents.OnTimerFinished -= ResetCassette_Screen;
    }

    private void Start()
    {
        _cassetteStartingPositions = new Vector3[_cassetteObjects.Length];

        for (int i = 0; i < _cassetteObjects.Length; i++)
        {
            // Store the starting positions of each cassette object.
            _cassetteStartingPositions[i] = _cassetteObjects[i].position;
        }
    }

    public void SwitchToCassettesMenu()
    {
        _cassettesMenu_Group.alpha = 1;
        _cassettesMenu_Group.interactable = true;
        _cassettesMenu_Group.blocksRaycasts = true;

        _casetteBackground.alpha = 1;
        _casetteBackground.interactable = true;
        _casetteBackground.blocksRaycasts = true;

        _mainMenu_Group.alpha = 0;
        _mainMenu_Group.interactable = false;
        _mainMenu_Group.blocksRaycasts = false;

        AudioEvents.FadeOutMusic(); // Fade out the music when going to cassette menu.
    }

    private void ResetCassette_Screen()
    {
        if (_trasitionAnimator != null)
        {
            // Reset animation.
            _trasitionAnimator.Rebind();
            _trasitionAnimator.Update(0f);
        }
        else
        {
            Debug.LogWarning("Transition Animator is not assigned. Skipping animation reset.");
            return;
        }


            for (int i = 0; i < _cassetteObjects.Length; i++)
            {
                // Reset each cassette to its starting position.
                _cassetteObjects[i].position = _cassetteStartingPositions[i];
            }
    }
}
