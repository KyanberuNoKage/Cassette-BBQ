using CustomInspector;
using UnityEngine;
using KyanberuGames.Utilities;
using UnityEngine.EventSystems;

public class Cassette_Anim_Control : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Cassette_Anim_Obj _thisCassette_Anim_Obj;
    public string thisCassettesName;
    [SerializeField] Animator _thisCassette_Animator;

    [Header("Data For Pop-ups")]
    [SerializeField] CassetteToolTip _cassettePopUp_Obj;
    [SerializeField] string _cassettePopUp_Text;
    [SerializeField] string _unlockCondition_Text;
    [SerializeField] Sprite _cassettePopUp_Sprite;
    [SerializeField] Sprite _lockedPopUp_Sprite;
    [SerializeField] Vector2 _cassettePopUp_Position;


    private void Awake()
    {
        if (_thisCassette_Anim_Obj == null)
        {
            DebugEvents.AddDebugError("Cassette_Anim_Obj is not assigned in " + gameObject.name);
            return;
        }

        thisCassettesName = _thisCassette_Anim_Obj.ThisCassetteName;
    }

    #region PopUp ToolTip
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_thisCassette_Anim_Obj.IsUnlocked)
        {
            _cassettePopUp_Obj.Show(_cassettePopUp_Position, _cassettePopUp_Sprite);
        }
        else
        {
            _cassettePopUp_Obj.Show(_cassettePopUp_Position, _lockedPopUp_Sprite);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _cassettePopUp_Obj.Hide();
    }
    #endregion

#if UNITY_EDITOR
#pragma warning disable CS0414 // Suppress: Field assigned but never used (its 'used' by CustomInspector Button)
    [Button(nameof(RevealCassette), true, label = "Reveal This Cassette", size = Size.small)]
    [SerializeField, HideField] private bool RevealCassette_true = true;
    #pragma warning restore CS0414
#endif

    // reveals the cassette in the cassette menu by changing its animation.
    public void RevealCassette(bool RevealTheCassette)
    {
        if (RevealTheCassette)
        {
            /**
             * All the cassette's animations are in their shared Animation Controller Component, 
             * when each individual cassette is revealed, it will show the animation
             * for whichever Cassette type it is set as in its Scriptable Object.
            **/

            if (_thisCassette_Anim_Obj == null)
            {
                DebugEvents.AddDebugError("Cassette_Anim_Obj is not assigned in " + gameObject.name);
                return;
            }
            else
            {
                _thisCassette_Anim_Obj.UnlockCassette(); // Unlock the cassette if it is revealed.
            }

                switch (_thisCassette_Anim_Obj.GetAnim())
                {
                    case CassetteAnimation.Silhouette:
                        _thisCassette_Animator.SetTrigger("Silhouette");
                    DebugEvents.AddDebugError("Cassette is not supposed to be set to Silhouette in Scriptable Object!!");
                        break;
                    case CassetteAnimation.SummerTime:
                        _thisCassette_Animator.SetTrigger("SummerTime");
                        break;
                    case CassetteAnimation.SlowShift:
                        _thisCassette_Animator.SetTrigger("SlowShift");
                        break;
                    case CassetteAnimation.RushHour:
                        _thisCassette_Animator.SetTrigger("RushHour");
                        break;
                    case CassetteAnimation.BunVoyage:
                        _thisCassette_Animator.SetTrigger("BunVoyage");
                        break;
                    case CassetteAnimation.HotDawg:
                        _thisCassette_Animator.SetTrigger("HotDawg");
                        break;
                    case CassetteAnimation.DoubleOrNothing:
                        _thisCassette_Animator.SetTrigger("DOR");
                        break;
                    default:
                    DebugEvents.AddDebugError("Unknown cassette animation type: " + _thisCassette_Anim_Obj.GetAnim());
                        break;
                }
        }
        else
        {
            // If the cassette is not revealed, it's set to Silhouette.
            _thisCassette_Animator.SetTrigger("Silhouette");
        }
    }


    public void SelectCassette()
    {
        AudioEvents.PlayEffect(SoundEffects.Click_1);

        // Cant select the cassette if it is not unlocked yet.
        if (_thisCassette_Anim_Obj.IsUnlocked == false) 
        {
#if UNITY_EDITOR // To reduce unnecessary logs in build.
            DebugEvents.AddDebugWarning("Cassette is not unlocked yet: " + _thisCassette_Anim_Obj.ThisCassetteName);
#endif
            return;
        }

        // Cant select the cassette if it is a Silhouette.
        if (_thisCassette_Anim_Obj.GetAnim() != CassetteAnimation.Silhouette)
        {
#if UNITY_EDITOR // To reduce unnecessary logs in build.
            DebugEvents.AddDebugLog("Cassette Selected: " + _thisCassette_Anim_Obj.ThisCassetteName);
#endif
            MenuTransitionEvents.RaiseCassetteSelected(this); // Event to notify transition when a cassette is selected.

            // Then the specific Cassette is set up.
            _thisCassette_Anim_Obj.OnThisCassetteSelected();
        }
    }
}

