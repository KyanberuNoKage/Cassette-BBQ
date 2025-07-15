using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Cassette_Anim_Obj", menuName = "Scriptable Objects/Cassette_Anim_Obj")]
public class Cassette_Anim_Obj : ScriptableObject
{
    [SerializeField] private string _thisCassettesName; public string ThisCassetteName => _thisCassettesName;
    [SerializeField] private CassetteAnimation _thisCassettesAnim;
    [SerializeField] private bool _isUnlocked = false; public bool IsUnlocked => _isUnlocked;


    public CassetteAnimation GetAnim()
    {
        return _thisCassettesAnim;
    }

    public void OnThisCassetteSelected()
    {
        // This method can be used to handle any logic when the cassette is selected.
        //Debug.Log($"Cassette Selected: {_thisCassettesName}");
        // USE EVENT TO CALL OUT THAT CASSETTE HAS BEEN CHOSEN.
    }
}

public enum CassetteAnimation
{
    Silhouette,
    SummerTime,
}
