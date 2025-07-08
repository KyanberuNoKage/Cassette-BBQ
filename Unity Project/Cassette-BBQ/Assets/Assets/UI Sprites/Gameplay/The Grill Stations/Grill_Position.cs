using UnityEngine;

public class Grill_Position : MonoBehaviour
{
    // Indicates if the grill position contains an item.
    [SerializeField] private bool _isFilled = false;
    public bool IsFilled => _isFilled;

    [SerializeField] private GameObject _thisGrillItem;
    public GameObject ThisGrillItem => _thisGrillItem;

    public void SetGrillItem(GameObject grillItem)
    {
        _thisGrillItem = grillItem;

        if (grillItem != null)
        {
            FillPosition();
        }
        else
        {
            EmptyPosition();
        }
    }

    public void FillPosition()
    {
        _isFilled = true;
    }

    public void EmptyPosition()
    {
        _isFilled = false;
    }
}
