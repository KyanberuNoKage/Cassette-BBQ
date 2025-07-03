using DG.Tweening;
using System;
using UnityEngine;

public class MenuTransitionController : MonoBehaviour
{
    [SerializeField] Animator _trasitionAnimator;
    [SerializeField] Camera _mainCamera;

    [SerializeField] Transform _cassette_One;
    [SerializeField] Transform _cassette_Two;
    [SerializeField] Transform _cassette_Three;
    [SerializeField] Transform _cassette_Four;
    [SerializeField] Transform _cassette_Five;
    [SerializeField] Transform _cassette_Six;



    private void OnEnable()
    {
        TransitionEvents.CassetteSelected += StartTransition;
    }

    private void OnDisable()
    {
        TransitionEvents.CassetteSelected -= StartTransition;
    }

    private void StartTransition(Cassette_Anim_Control selectedCassette)
    {
        Debug.Log("Cassette selected: " + selectedCassette.name);

        MoveCassettesOffScreen();
    }

    private void MoveCassettesOffScreen()
    {
        float _offscreenX = 250f;
        float _moveDuration = 0.25f;

        Sequence _exitSequence;

        _exitSequence = DOTween.Sequence();

        // half‑delay so second row starts when first is 50% done,
        // etc.
        float halfDelay = _moveDuration * 0.5f;

        // Row 1: One & Two, at t = 0
        _exitSequence.Insert(0f,
            _cassette_One.DOMoveX(_cassette_One.position.x - _offscreenX, _moveDuration));
        _exitSequence.Insert(0f,
            _cassette_Two.DOMoveX(_cassette_Two.position.x + _offscreenX, _moveDuration));

        // Row 2: Three & Four, at t = halfDelay
        _exitSequence.Insert(halfDelay,
            _cassette_Three.DOMoveX(_cassette_Three.position.x - _offscreenX, _moveDuration));
        _exitSequence.Insert(halfDelay,
            _cassette_Four.DOMoveX(_cassette_Four.position.x + _offscreenX, _moveDuration));

        // Row 3: Five & Six, at t = 2 * halfDelay
        float thirdStartTime = 2f * halfDelay;
        _exitSequence.Insert(thirdStartTime,
            _cassette_Five.DOMoveX(_cassette_Five.position.x - _offscreenX, _moveDuration));
        _exitSequence.Insert(thirdStartTime,
            _cassette_Six.DOMoveX(_cassette_Six.position.x + _offscreenX, _moveDuration));

        // Fire your Transition() right when the last pair starts:
        _exitSequence.InsertCallback(thirdStartTime, () => StartTransition());

        // Finally play it
        _exitSequence.Play();

    }

    private void StartTransition()
    {
        Sequence _transitionSequence = DOTween.Sequence();
        _transitionSequence.SetDelay(0.15f); // Delay before the transition starts to let the animation play.
        _transitionSequence.Append
        (
            _mainCamera.transform.DOShakePosition(3f, 0.5f, 10, 70f, false, true)
        );
        _trasitionAnimator.SetTrigger("BoomBox"); // Starts the "BoomBox" transition animation.
        _transitionSequence.Play();
    }
}

// A message broker for talking between the Cassette_Anim_Control buttons and the MenuTransitionController.
public static class TransitionEvents
{
    public static event Action<Cassette_Anim_Control> CassetteSelected;

    public static void RaiseCassetteSelected(Cassette_Anim_Control cassette)
    {
        CassetteSelected?.Invoke(cassette);
    }
}
