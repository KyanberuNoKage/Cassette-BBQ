
using CustomInspector;
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
        if (_isUnlocked)
        {
            return _thisCassettesAnim;
        }
        else
        {
            return CassetteAnimation.Silhouette; // Default animation when not unlocked.
        }
    }

    public void OnThisCassetteSelected()
    {
        CassetteEvents.CassetteSelected(_thisCassetteGameValues);
    }

    public void UnlockCassette()
    {
        _isUnlocked = true;
    }
}

public enum CassetteAnimation
{
    Silhouette,
    SummerTime,
    SlowShift,
    RushHour,
    BunVoyage,
    HotDawg,
    DoubleOrNothing,
}

public enum CassetteType
{
    Silhouette,
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
    [SerializeField] private bool _isDoubleOrNothing;
    public bool IsDoubleOrNothing => _isDoubleOrNothing;

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
    // Shows only if orders aren't random.
    [SerializeField, ShowIf(nameof(IsNotRandom), style = DisabledStyle.Invisible, indent = 0)] 
    private bool areOrders_Burgers; private bool IsNotRandom() { return !areOrderTypesRandom; }
    public bool AreOrderTypesRandom => areOrderTypesRandom;
    public bool AreOrders_Burgers => areOrders_Burgers;
}
