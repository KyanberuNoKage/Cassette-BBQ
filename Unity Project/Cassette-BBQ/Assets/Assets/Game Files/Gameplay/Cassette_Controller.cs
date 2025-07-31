
using System;
using System.Collections.Generic;
using UnityEngine;
using KyanberuGames.Utilities;

public class Cassette_Controller : MonoBehaviour
{
    [SerializeField] private List<Cassette_Anim_Control> _cassettesList;
    [SerializeField] private List<Cassette_Anim_Control> _activatedCassettes;

    [SerializeField] private Cassette_Anim_Control _defaultCassette;

    private void OnEnable()
    {
        SaveData_MessageBus.OnRequestRevealedCassettes += GetActiveCassettes;
        SaveData_MessageBus.OnSetRevealedCassettes += SetActiveCassettes;

        CassetteEvents.OnCassetteUnlocked += UnlockCassette;
    }

    private void OnDisable()
    {
        SaveData_MessageBus.OnRequestRevealedCassettes -= GetActiveCassettes;
        SaveData_MessageBus.OnSetRevealedCassettes -= SetActiveCassettes;

        CassetteEvents.OnCassetteUnlocked -= UnlockCassette;
    }

    private List<string> GetActiveCassettes()
    {
        List<string> cassetteNames = new List<string>();

        foreach (Cassette_Anim_Control cassette in _activatedCassettes)
        {
            cassetteNames.Add(cassette.thisCassettesName);
        }

        return cassetteNames;
    }

    private void SetActiveCassettes(List<string> activeCassetteList)
    {
        _activatedCassettes.Clear();

        foreach (var cassette in _cassettesList)
        {
            bool shouldShow = activeCassetteList.Contains(cassette.thisCassettesName);
            cassette.RevealCassette(shouldShow);
            if (shouldShow)
            {
                _activatedCassettes.Add(cassette);
            }
        }
    }

    private void UnlockCassette(CassetteType typeToUnlock)
    {
        string unlockedCassetteName = typeToUnlock.ToString();

        foreach (Cassette_Anim_Control cassette in _activatedCassettes)
        {
            if (cassette.thisCassettesName == unlockedCassetteName)
            {
#if UNITY_EDITOR // To reduce unnecessary logs in build.
                DebugEvents.AddDebugWarning("Cassette " + unlockedCassetteName + " is already unlocked.");
#endif
                return; // Exit whole method early.
            }
        }

        foreach (Cassette_Anim_Control cassette in _cassettesList)
        {
            if (cassette.thisCassettesName == unlockedCassetteName)
            {
                cassette.RevealCassette(true);
                _activatedCassettes.Add(cassette);
                DebugEvents.AddDebugLog("Unlocked cassette:\n" + unlockedCassetteName);
                return; // Exit whole method once found.
            }
        }
    }
}

public static class CassetteEvents
{
    public static event Action<CassetteGameValues> OnCassetteSelected;

    public static void CassetteSelected(CassetteGameValues selectedCassetteValues)
    {
        OnCassetteSelected?.Invoke(selectedCassetteValues);
    }

    public static event Action<CassetteType> OnCassetteUnlocked;

    public static void UnlockCassette(CassetteType typeToUnlock)
    {
        OnCassetteUnlocked?.Invoke(typeToUnlock);
    }
}

