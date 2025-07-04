using DG.Tweening;
using UnityEngine;

public class Raw_Burgers_Button : MonoBehaviour
{
    public void SelectRawBurger()
    {
        DoSelectionAnim();
    }

    private void DoSelectionAnim()
    {
        gameObject.transform
            .DOScale
            (
                new Vector3(1.15f, 1.15f, 1.15f),
                0.15f
            )
            .OnComplete
            (
                () => gameObject.transform.DOScale
                    (
                        new Vector3(1f, 1f, 1f),
                        0.10f
                    )
            );
    }
}
