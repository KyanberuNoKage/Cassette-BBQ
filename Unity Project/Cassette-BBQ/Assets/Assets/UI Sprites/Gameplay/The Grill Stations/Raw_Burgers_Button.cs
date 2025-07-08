using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class Raw_Burgers_Button : MonoBehaviour
{
    public void SelectRawBurger()
    {
        DoSelectionAnim();

        GrillingEvents.AddRawBurger_ToGrill();
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

        // Reset the selected sprite to nothing so the event system doesn't get
        // confused and stop the button from being re-highlighted.
        EventSystem.current.SetSelectedGameObject(null);
    }
}
