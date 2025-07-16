using System;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.XR;

public class YesAndNo_Buttons : MonoBehaviour
{
    [SerializeField] bool isYesButton;

    public void OnButtonPress()
    {
        if (ReceiptStateHolder.CurrentState == ReceiptState.DataReset)
        {
            if (isYesButton) // Reset data
            {
                Debug.LogWarning(new NotImplementedException());
            }
            else if (!isYesButton) // Don't reset data
            {
                ReceiptStateHolder.ChangeState(ReceiptState.PrintData);
                DataReset_UI_Events.ChangeToPrintReceipt();
            }
        }
        else // In Printing State
        {
            if (isYesButton)
            {
                Debug.LogWarning(new NotImplementedException());
            }
            else if (!isYesButton)
            {
                DataReset_UI_Events.RemovePrintReceipt();
            }
        }
    }
}

public static class ReceiptStateHolder
{
    public static ReceiptState CurrentState;

    public static void ChangeState(ReceiptState newState)
    {
        CurrentState = newState;
    }
}

public enum ReceiptState
{
    DataReset,
    PrintData
}
