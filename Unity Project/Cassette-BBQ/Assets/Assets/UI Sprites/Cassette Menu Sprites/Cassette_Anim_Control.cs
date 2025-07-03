using UnityEngine;

public class Cassette_Anim_Control : MonoBehaviour
{
    [SerializeField] Cassette_Anim_Obj _thisCassette_Anim_Obj;
    [SerializeField] Animator _thisCassette_Animator;

    private void Start()
    {
        if (_thisCassette_Anim_Obj == null)
        {
            Debug.LogError("Cassette_Anim_Obj is not assigned in " + gameObject.name);
        }

        if(_thisCassette_Anim_Obj.IsUnlocked == true)
        {
            RevealCassette(true);
        }
        else
        {
            RevealCassette(false);
        }
    }


    public void RevealCassette(bool ReviealTheCassette) // reveals or hides the cassette in menu.
    {
        if (ReviealTheCassette)
        {
            /**
             * All the cassette's animations are in their shared Animation Controller Component, 
             * when each individual cassette is revealed, it will show the animation
             * for whichever Cassette type it is set as in its Scriptable Object.
            **/

            switch (_thisCassette_Anim_Obj.GetAnim())
            {
                case CassetteAnimation.Silhouette:
                    _thisCassette_Animator.SetTrigger("Silhouette");
                    Debug.LogWarning("Cassette is not supposed to be set to Silhouette in Scriptable Object!!");
                    break;
                case CassetteAnimation.SummerTime:
                    _thisCassette_Animator.SetTrigger("SummerTime");
                    break;
                default:
                    Debug.LogError("Unknown cassette animation type: " + _thisCassette_Anim_Obj.GetAnim());
                    break;
            }
        }
        else
        {
            // If the cassette is not revealed, we set it to Silhouette.
            _thisCassette_Animator.SetTrigger("Silhouette");
        }
    }

    public void SelectCassette()
    {
        // Cant select the cassette if it is a Silhouette.
        if (_thisCassette_Anim_Obj.GetAnim() != CassetteAnimation.Silhouette)
        {
            _thisCassette_Anim_Obj.OnThisCassetteSelected();
        }
    }
}

