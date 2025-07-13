
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

    private List<Cassette_Anim_Control> GetActiveCassettes()
    {
        return _activatedCassettes;
    }

    private void SetActiveCassettes()
    {
        _activatedCassettes = new List<Cassette_Anim_Control>();
        foreach (Cassette_Anim_Control cassette in Cassettes)
        {
            if (cassette != null)
            {
                cassette.RevealCassette(true);
                _activatedCassettes.Add(cassette);
            }
        }
    }

    private void SetActiveCassettes(List<Cassette_Anim_Control> activeCassetteList)
    {
        foreach (var cassette in Cassettes)
        {
            if (activeCassetteList.Contains(cassette))
            {
                cassette.RevealCassette(true);
            }
            else
            {
                cassette.RevealCassette(false);
            }
        }
    }
}
