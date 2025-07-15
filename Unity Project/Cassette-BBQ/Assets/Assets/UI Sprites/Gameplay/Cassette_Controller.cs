
using System.Collections.Generic;
using UnityEngine;

public class Cassette_Controller : MonoBehaviour
{
    [SerializeField] private List<Cassette_Anim_Control> Cassettes;
    [SerializeField] private List<Cassette_Anim_Control> _activatedCassettes;

    private void OnEnable()
    {
        SaveData_MessageBus.OnRequestRevealedCassettes += GetActiveCassettes;
        SaveData_MessageBus.OnSetRevealedCassettes += SetActiveCassettes;
    }

    private void OnDisable()
    {
        SaveData_MessageBus.OnRequestRevealedCassettes -= GetActiveCassettes;
        SaveData_MessageBus.OnSetRevealedCassettes -= SetActiveCassettes;
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
        foreach (var cassette in Cassettes)
        {
            if (activeCassetteList.Contains(cassette.thisCassettesName))
            {
                cassette.RevealCassette(true);
                _activatedCassettes.Add(cassette);
            }
            else
            {
                cassette.RevealCassette(false);

                foreach(var activeCassette in _activatedCassettes)
                {
                    if(activeCassette.thisCassettesName == cassette.thisCassettesName)
                    {
                        _activatedCassettes.Remove(activeCassette);
                    }
                }
            }
        }
    }
}
