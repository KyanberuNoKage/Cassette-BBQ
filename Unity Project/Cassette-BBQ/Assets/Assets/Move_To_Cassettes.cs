using System;
using System.Collections.Generic;
using UnityEngine;

public class Move_To_Cassettes : MonoBehaviour
{
    [SerializeField] CanvasGroup _mainMenu_Group;
    [SerializeField] CanvasGroup _cassettesMenu_Group;
    [SerializeField] CanvasGroup _casetteBackground;

    [Serializable]
    public class CassetteDefaultPositions
    {
        public RectTransform thisCassettesRectTransform;
        public Vector2 thisCassettesStartingPosition;
    }

    [SerializeField] List<CassetteDefaultPositions> _cassetteDefaultPositions;

    private void OnEnable()
    {
        TimerEvents.OnTimerFinished += ResetCassette_Screen;
    }

    private void OnDisable()
    {
        TimerEvents.OnTimerFinished -= ResetCassette_Screen;
    }

    private void Awake()
    {
        foreach(CassetteDefaultPositions cassette in _cassetteDefaultPositions)
        {
            cassette.thisCassettesStartingPosition = cassette.thisCassettesRectTransform.anchoredPosition;
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
        foreach (CassetteDefaultPositions cassette in _cassetteDefaultPositions)
        {
            cassette.thisCassettesRectTransform.anchoredPosition = cassette.thisCassettesStartingPosition;
        }
    }
}
