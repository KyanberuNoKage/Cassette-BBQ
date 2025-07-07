using UnityEngine;

[CreateAssetMenu(fileName = "Base_Sausage_Data", menuName = "Scriptable Objects/Base_Sausage_Data")]
public class Base_Sausage_Data : ScriptableObject
{
    // Cooking times for Sausages.
    [SerializeField] float _SausageCookTimeMin = 3f; public float MinSausageCookTime => _SausageCookTimeMin;
    [SerializeField] float _SausageCookTimeMax = 5f; public float MaxSausageCookTime => _SausageCookTimeMax;

    [SerializeField] float _SausageBurnTimeMin = 3f; public float MinSausageBurnTime => _SausageBurnTimeMin;
    [SerializeField] float _SausageBurnTimeMax = 5f; public float MaxSausageBurnTime => _SausageBurnTimeMax;

    // The time the player has to flip the Sausage before it starts burning
    [SerializeField] float _SausageFlipTime = 2f; public float SausageFlipTime => _SausageFlipTime;

    // All Sausage sprites for different states.
    [SerializeField] Sprite _rawSausageSprite; public Sprite RawSausageSprite => _rawSausageSprite;
    [SerializeField] Sprite _cookedSausageSpriteOne; public Sprite CookedSausageSpriteOne => _cookedSausageSpriteOne;
    [SerializeField] Sprite _cookedSausageSpriteTwo; public Sprite CookedSausageSpriteTwo => _cookedSausageSpriteTwo;
    [SerializeField] Sprite _burntSausageSprite; public Sprite BurntSausageSprite => _burntSausageSprite;

    [SerializeField] GameObject _oilSplashPrefab; public GameObject OilSplashPrefab => _oilSplashPrefab;
    [SerializeField] GameObject _flipSmokePrefab; public GameObject FlipSmokePrefab => _flipSmokePrefab;
    [SerializeField] GameObject _finishedSparklePrefab; public GameObject FinishedSparklePrefab => _finishedSparklePrefab;

    [SerializeField] Sprite _oilOffFrame; public Sprite OilOffFrame => _oilOffFrame;
    [SerializeField] Sprite _oilOnFrame; public Sprite OilOnFrame => _oilOnFrame;

    // Way to have a one off anim without needing to create an animator or controller for it.
    [SerializeField] Sprite[] _SausageFlipSprites; public Sprite[] SausageFlipSprites => _SausageFlipSprites;
    [SerializeField] float _timeBetweenFrames = 0.2f; public float TimeBetweenFrames => _timeBetweenFrames;
    public float GetRandomCookTime()
    {
        return Random.Range(_SausageCookTimeMin, _SausageCookTimeMax);
    }

    public float GetRandomBurnTime()
    {
        return Random.Range(_SausageBurnTimeMin, _SausageBurnTimeMax);
    }
}
