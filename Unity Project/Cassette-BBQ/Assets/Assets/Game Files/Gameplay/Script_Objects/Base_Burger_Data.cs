using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Base_Burger_Data", menuName = "Scriptable Objects/Base_Burger_Data")]
public class Base_Burger_Data : ScriptableObject
{
    // Cooking times for burgers.
    [SerializeField] float _burgerCookTimeMin = 3f; public float MinBurgerCookTime => _burgerCookTimeMin;
    [SerializeField] float _burgerCookTimeMax = 5f; public float MaxBurgerCookTime => _burgerCookTimeMax;

    [SerializeField] float _burgerBurnTimeMin = 3f; public float MinBurgerBurnTime => _burgerBurnTimeMin;
    [SerializeField] float _burgerBurnTimeMax = 5f; public float MaxBurgerBurnTime => _burgerBurnTimeMax;

    // The time the player has to flip the burger before it starts burning
    [SerializeField] float _burgerFlipTime = 2f; public float BurgerFlipTime => _burgerFlipTime;

    // All burger sprites for different states.
    [SerializeField] Sprite _rawBurgerSprite; public Sprite RawBurgerSprite => _rawBurgerSprite;
    [SerializeField] Sprite _cookedBurgerSprite; public Sprite CookedBurgerSprite => _cookedBurgerSprite;
    [SerializeField] Sprite _burntBurgerSprite; public Sprite BurntBurgerSprite => _burntBurgerSprite;

    [SerializeField] GameObject _oilSplashPrefab; public GameObject OilSplashPrefab => _oilSplashPrefab;
    [SerializeField] GameObject _flipSmokePrefab; public GameObject FlipSmokePrefab => _flipSmokePrefab;
    [SerializeField] GameObject _finishedSparklePrefab; public GameObject FinishedSparklePrefab => _finishedSparklePrefab;

    [SerializeField] Sprite _oilOffFrame; public Sprite OilOffFrame => _oilOffFrame;
    [SerializeField] Sprite _oilOnFrame; public Sprite OilOnFrame => _oilOnFrame;
    [SerializeField] Sprite _wooshStartFrame; public Sprite WooshStartFrame => _wooshStartFrame;

    // Way to have a one off anim without needing to create an animator or controller for it.
    [SerializeField] Sprite[] _burgerFlipSprites; public Sprite[] BurgerFlipSprites => _burgerFlipSprites;
    [SerializeField] float _timeBetweenFrames = 0.2f; public float TimeBetweenFrames => _timeBetweenFrames;
    public float GetRandomCookTime()
    {
        return Random.Range(_burgerCookTimeMin, _burgerCookTimeMax);
    }

    public float GetRandomBurnTime()
    {
        return Random.Range(_burgerBurnTimeMin, _burgerBurnTimeMax);
    }
}
