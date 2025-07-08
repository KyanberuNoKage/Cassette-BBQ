using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Sausage_Obj : MonoBehaviour
{

    [SerializeField] Base_Sausage_Data _globalSausageData;
    [SerializeField] Image _sausageImage;
    [SerializeField] Button _sausageButton;
    [SerializeField] Transform _thisSausageTransform;

    bool _isFlipped = false;
    bool _isBurnt = false;
    bool _isFinished = false;

    private float _sausageCookTime;
    private float _sausageBurnTime;

    private GameObject _oilSplash_Obj;
    private GameObject _smokeEffect_Obj;
    private GameObject _sparkleEffect_Obj;

    private Sprite oilOffSprite;
    private Sprite oilOnSprite;

    private void Start()
    {
        if (_sausageImage != null && _globalSausageData != null && _sausageButton != null)
        {
            SetUpThisSausage();

            StartCoroutine(StartCooking());
        }
        else
        {
            #region Debugging
            if (_sausageImage == null)
            {
                Debug.LogError("_SausageImage is not set in the Sausage_Obj script!");
            }
            if (_globalSausageData == null)
            {
                Debug.LogError("Base_Sausage_Data is not set in the Sausage_Obj script!");
            }
            if (_sausageButton == null)
            {
                Debug.LogError("Sausage button is not set in the Sausage_Obj script!");
            }
            #endregion
        }
    }

    private void SetUpThisSausage()
    {
        // Ensure Sausage image is raw to begin with.
        _sausageImage.sprite = _globalSausageData.RawSausageSprite;

        oilOnSprite = _globalSausageData.OilOnFrame;
        oilOffSprite = _globalSausageData.OilOffFrame;

        // Get randomized cook and burn times for this Sausage with ScriptableObject data.
        _sausageCookTime = _globalSausageData.GetRandomCookTime();
        _sausageBurnTime = _globalSausageData.GetRandomBurnTime();

        _oilSplash_Obj = Instantiate
            (
                _globalSausageData.OilSplashPrefab,
                _thisSausageTransform,
                false
            );
        // Ensure oil splash appears behind the Sausage image.
        _oilSplash_Obj.transform.SetAsFirstSibling();

        _sausageButton.interactable = false;
    }

    public void FlipSausage()
    {
        // Currently both burning and finishing the Sausages will do almost
        // the same thing, until the score system is built.
        if (_isFinished)
        {
            Debug.Log("Sausage has been collected!");
            StopAllCoroutines();
            OrderEvents.FillOrder(false); // Notify that a Sausage has been finished.
            GrillingEvents.DestroyGrill_Obj(this.gameObject); // Notify that there is one less item on grill.
            return;
        }
        else if (_isBurnt)
        {
            Debug.Log("Sausage has been thrown away!");
            StopAllCoroutines();
            GrillingEvents.DestroyGrill_Obj(this.gameObject);
            return;
        }

        if (!_isFlipped)
        {
            _isFlipped = true;
            Debug.Log("Sausage flipped!");
            Destroy(_smokeEffect_Obj);

            // Start the coroutine to finish cooking the Sausage.
            StartCoroutine(FinishCooking());
        }
        else
        {
            Debug.Log("Sausage already flipped!");
        }
    }

    private IEnumerator StartCooking()
    {
        Debug.Log("Sausage cooking started...");
        while (!_isFlipped && !_isFinished && !_isBurnt)
        {

            // Wait for the cook time before changing the sprite to cooked.
            yield return new WaitForSeconds(_sausageCookTime);
            _sausageImage.sprite = _globalSausageData.CookedSausageSpriteOne;
            Debug.Log("Sausage cooked!");

            // Enable the Sausage button to allow flipping.
            _sausageButton.interactable = true;

            _smokeEffect_Obj = Instantiate
                (
                    _globalSausageData.FlipSmokePrefab,
                    _thisSausageTransform,
                    false
                );
            // Ensure smoke appears behind the Sausage image.
            _smokeEffect_Obj.transform.SetAsFirstSibling();

            yield return new WaitForSeconds(_globalSausageData.SausageFlipTime);

            // After total burn time completed, turn the Sausage burnt.
            yield return new WaitForSeconds(_sausageBurnTime);

            _sausageImage.sprite = _globalSausageData.BurntSausageSprite;
            _isBurnt = true;
            Debug.Log("Sausage burnt!");
        }

        Debug.Log("First Sausage cooking coroutine ended.");

        yield break;
    }

    private IEnumerator FinishCooking()
    {
        // Now the Sausage has been "Flipped" in the data,
        // wait for the animation to play out before continuing.
        yield return StartCoroutine(FlipSausageAnimation());

        // Wait for the cook time before changing the sprite to cooked.
        yield return new WaitForSeconds(_sausageCookTime);
        Debug.Log("Sausage fully cooked!");

        // Enable the Sausage button to allow finishing.
        _isFinished = true;
        _sausageButton.interactable = true;

        _sparkleEffect_Obj = Instantiate
            (
                _globalSausageData.FinishedSparklePrefab,
                _thisSausageTransform,
                false
            );
        // Ensure sparkle appears on top of the Sausage image.
        _sparkleEffect_Obj.transform.SetAsLastSibling();

        // So sausage cant burn while player is processing that its finished.
        yield return new WaitForSeconds(_globalSausageData.SausageFlipTime);

        // After total burn time completed, turn the Sausage burnt.
        yield return new WaitForSeconds(_sausageBurnTime);

        _sausageImage.sprite = _globalSausageData.BurntSausageSprite;
        _isFinished = false;
        _isBurnt = true;
        Destroy(_sparkleEffect_Obj);
        Destroy(_oilSplash_Obj);
        Debug.Log("Sausage burnt!");
    }

    private IEnumerator FlipSausageAnimation()
    {
        foreach (Sprite flipAnimFrame in _globalSausageData.SausageFlipSprites)
        {
            // Signal to hide and show oil based on given frames.
            if (flipAnimFrame == oilOffSprite)
                FadeInOil(false);


            else if (flipAnimFrame == oilOnSprite)
                FadeInOil(true);

            _sausageImage.sprite = flipAnimFrame;
            yield return new WaitForSeconds(_globalSausageData.TimeBetweenFrames);
        }

        // After the animation is done, set the Sausage image to second cooked variant.
        _sausageImage.sprite = _globalSausageData.CookedSausageSpriteTwo;

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
