
using UnityEngine;

[CreateAssetMenu(fileName = "Cassette_Anim_Obj", menuName = "Scriptable Objects/Cassette_Anim_Obj")]
public class Cassette_Anim_Obj : ScriptableObject
{
    [SerializeField] private string _thisCassettesName; public string ThisCassetteName => _thisCassettesName;
    [SerializeField] private CassetteAnimation _thisCassettesAnim;
    [SerializeField] private bool _isUnlocked = false; public bool IsUnlocked => _isUnlocked;
    [SerializeField] private CassetteType _thisCassetteType; public CassetteType ThisCassetteType => _thisCassetteType;
    [SerializeField] private CassetteGameValues _thisCassetteGameValues; public CassetteGameValues ThisCassetteGameValues => _thisCassetteGameValues;

    private void OnEnable()
    {
        // Ensure name is set/spelled correctly when the object is enabled.
        _thisCassettesName = _thisCassetteType.ToString();
    }

    public CassetteAnimation GetAnim()
    {
        return _thisCassettesAnim;
    }

    public void OnThisCassetteSelected()
    {
        CassetteEvents.CassetteSelected(_thisCassetteGameValues);
    }
}

public enum CassetteAnimation
{
    Silhouette,
    SummerTime,
}

public enum CassetteType
{
    SummerTime,
    SlowShift,
    RushHour,
    BunVoyage,
    HotDawg,
    DoubleOrNothing
}

[System.Serializable]
public class CassetteGameValues
{
    // These private fields are edited using unity's inspector, and are read-only from other scripts.

    // For OilTimer.cs
    [SerializeField] private int timerDuration;
    public int TimerDuration => timerDuration;
    
    // For Score_Manager.cs
    [SerializeField] private int orderScore_BaseValue;
    [SerializeField] private float orderScore_DecayRate;
    [SerializeField] private int wastedFoodPenalty;
    public int OrderScore_BaseValue => orderScore_BaseValue;
    public float OrderScore_DecayRate => orderScore_DecayRate;
    public int WastedFoodPenalty => wastedFoodPenalty;
    
    // For Order_Manager.cs
    [SerializeField] private bool areOrderTypesRandom;
    [SerializeField] private bool areOrders_Burgers;
    public bool AreOrderTypesRandom => areOrderTypesRandom;
    public bool AreOrders_Burgers => areOrders_Burgers;
}
