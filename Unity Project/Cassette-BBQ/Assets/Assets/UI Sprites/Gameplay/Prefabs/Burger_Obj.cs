using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Burger_Obj : MonoBehaviour
{

    [SerializeField] Base_Burger_Data _globalBurgerData;
    [SerializeField] Image _burgerImage;
    [SerializeField] Button _burgerButton;
    [SerializeField] Transform _thisBurgerTransform;

    bool _isFlipped = false;
    bool _isBurnt = false;
    bool _isFinished = false;

    private float _burgerCookTime;
    private float _burgerBurnTime;

    private GameObject _oilSplash_Obj;
    private GameObject _smokeEffect_Obj;
    private GameObject _sparkleEffect_Obj;

    Sprite oilOffSprite;
    Sprite oilOnSprite;
    private Sprite _wooshOnSprite;

    private void Start()
    {
        if (_burgerImage != null && _globalBurgerData != null && _burgerButton != null)
        {
            SetUpThisBurger();

            StartCoroutine(StartCooking());
        }
        else
        {
            #region Debugging
            if (_burgerImage == null)
            {
                Debug.LogError("_burgerImage is not set in the Burger_Obj script!");
            }
            if (_globalBurgerData == null)
            {
                Debug.LogError("Base_Burger_Data is not set in the Burger_Obj script!");
            }
            if (_burgerButton == null)
            {
                Debug.LogError("Burger button is not set in the Burger_Obj script!");
            }
            #endregion
        }
    }

    private void SetUpThisBurger()
    {
        // Ensure burger image is raw to begin with.
        _burgerImage.sprite = _globalBurgerData.RawBurgerSprite;

        oilOffSprite = _globalBurgerData.OilOffFrame;
        oilOnSprite = _globalBurgerData.OilOnFrame;
        _wooshOnSprite = _globalBurgerData.WooshStartFrame;

    // Get randomized cook and burn times for this burger with ScriptableObject data.
    _burgerCookTime = _globalBurgerData.GetRandomCookTime();
        _burgerBurnTime = _globalBurgerData.GetRandomBurnTime();

        _oilSplash_Obj = Instantiate
            (
                _globalBurgerData.OilSplashPrefab,
                _thisBurgerTransform,
                false
            );
        // Ensure oil splash appears behind the burger image.
        _oilSplash_Obj.transform.SetAsFirstSibling();

        _burgerButton.interactable = false;
    }

    public void FlipBurger()
    {
        // Currently both burning and finishing the burgers will do almost
        // the same thing, until the score system is built.
        if (_isBurnt)
        {
            Debug.Log("Burger has been thrown away!");
            StopAllCoroutines();
            ScoreEvents.ItemWaste_DecreaseScore();
            GrillingEvents.DestroyGrill_Obj(this.gameObject);
            return;
        }
        else if (_isFinished)
        {
            Debug.Log("Burger has been collected!");
            StopAllCoroutines();
            OrderEvents.FillOrder(true); // Notify that a burger has been finished.
            GrillingEvents.DestroyGrill_Obj(this.gameObject); // Notify that there is one less item on grill.
            return;
        }

        if (!_isFlipped)
        {
            _isFlipped = true;
            Debug.Log("Burger flipped!");
            Destroy(_smokeEffect_Obj);

            // Start the coroutine to finish cooking the burger.
            StartCoroutine(FinishCooking());
        }
        else
        {
            Debug.Log("Burger already flipped!");
        }
    }

    private IEnumerator StartCooking()
    {
        Debug.Log("Burger cooking started...");
        while (!_isFlipped && !_isFinished && !_isBurnt)
        {

            // Wait for the cook time before changing the sprite to cooked.
            yield return new WaitForSeconds(_burgerCookTime);
            _burgerImage.sprite = _globalBurgerData.CookedBurgerSprite;
            Debug.Log("Burger cooked!");

            // Enable the burger button to allow flipping.
            _burgerButton.interactable = true;

            _smokeEffect_Obj = Instantiate
                (
                    _globalBurgerData.FlipSmokePrefab,
                    _thisBurgerTransform,
                    false
                );
            // Ensure smoke appears behind the burger image.
            _smokeEffect_Obj.transform.SetAsFirstSibling();

            yield return new WaitForSeconds(_globalBurgerData.BurgerFlipTime);

            // After total burn time completed, turn the burger burnt.
            yield return new WaitForSeconds(_burgerBurnTime);

            _burgerImage.sprite = _globalBurgerData.BurntBurgerSprite;
            _isBurnt = true;
            Debug.Log("Burger burnt!");
        }

        Debug.Log("First Burger cooking coroutine ended.");

        yield break;
    }

    private IEnumerator FinishCooking()
    {
        // Now the burger has been "Flipped" in the data,
        // wait for the animation to play out before continuing.
        yield return StartCoroutine(FlipBurgerAnimation());

        // Wait for the cook time before changing the sprite to cooked.
        yield return new WaitForSeconds(_burgerCookTime);
        Debug.Log("Burger fully cooked!");

        // Enable the burger button to allow finishing.
        _isFinished = true;
        _burgerButton.interactable = true;

        _sparkleEffect_Obj = Instantiate
            (
                _globalBurgerData.FinishedSparklePrefab,
                _thisBurgerTransform,
                false
            );
        // Ensure sparkle appears on top of the burger image.
        _sparkleEffect_Obj.transform.SetAsLastSibling(); 

        yield return new WaitForSeconds(_globalBurgerData.BurgerFlipTime);

        // After total burn time completed, turn the burger burnt.
        yield return new WaitForSeconds(_burgerBurnTime);

        _burgerImage.sprite = _globalBurgerData.BurntBurgerSprite;
        _isFinished = false;
        _isBurnt = true;
        Destroy(_sparkleEffect_Obj);
        Destroy(_oilSplash_Obj);
        Debug.Log("Burger burnt!");
    }

    private IEnumerator FlipBurgerAnimation()
    {

        foreach (Sprite flipAnimFrame in _globalBurgerData.BurgerFlipSprites)
        {
            // Signal to hide and show oil based on given frames.
            if (flipAnimFrame == oilOffSprite)
            {
                FadeInOil(false);
            }

            else if (flipAnimFrame == oilOnSprite)
            {
                FadeInOil(true);
                AudioEvents.PlayEffect(SoundEffects.Quick_Sizzle);
            }

            else if (flipAnimFrame == _wooshOnSprite)
            {
                AudioEvents.PlayRandomWoosh();
            }

            _burgerImage.sprite = flipAnimFrame;
            yield return new WaitForSeconds(_globalBurgerData.TimeBetweenFrames);
        }

        // After the animation is done, set the burger image back to cooked.
        _burgerImage.sprite = _globalBurgerData.CookedBurgerSprite;

        yield return null;
    }

    private void FadeInOil(bool fadeIn = true)
    {
        if (fadeIn)
        {
            _oilSplash_Obj.GetComponent<CanvasGroup>().DOFade(1f, 0.03f);
        }
        else 
        {
            _oilSplash_Obj.GetComponent<CanvasGroup>().DOFade(0f, 0.20f);
        }
    }
}
