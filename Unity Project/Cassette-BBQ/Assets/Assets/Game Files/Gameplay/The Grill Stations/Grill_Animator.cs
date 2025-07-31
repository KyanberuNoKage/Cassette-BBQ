using UnityEngine;
using UnityEngine.UI;

public class Grill_Animator : MonoBehaviour
{
    [SerializeField]
    Sprite[] _grillAnimSprites;

    [SerializeField] Image _spriteImage;

    [SerializeField, Tooltip("Time between each frame in seconds")] 
    float _animationSpeed = 1.5f;

    int _currentIndex = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // First change is instant to ensure current sprite is indexed and the next sprite is different.
        InvokeRepeating(nameof(ChangeSprite), 0f, _animationSpeed);
    }

    private void ChangeSprite()
    {
        int newIndex;

        do
        {
            newIndex = Random.Range(0, _grillAnimSprites.Length);
        } while (newIndex == _currentIndex);

        _currentIndex = newIndex;
        _spriteImage.sprite = _grillAnimSprites[_currentIndex];
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(ChangeSprite));
    }
}
