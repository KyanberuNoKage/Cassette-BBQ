using System;
using UnityEngine;

public class YesAndNo_Buttons : MonoBehaviour
{
    [SerializeField] bool isYesButton;

    public void OnButtonPress()
    {
        if (ReceiptStateHolder.CurrentState == ReceiptState.DataReset)
        {
            if (isYesButton) // Reset data
            {
                ReceiptStateHolder.SetDeleteData(DeleteData.Yes);
                /** DataReset_UI_Events.ChangeToPrintReceipt(); **/
            }
            else if (!isYesButton) // Don't reset data
            {
                ReceiptStateHolder.SetDeleteData(DeleteData.No);
                //DataReset_UI_Events.ChangeToPrintReceipt();
            }

            /**ReceiptStateHolder.ChangeState(ReceiptState.PrintData);**/
            DataReset_UI_Events.RemovePrintReceipt();
            CompleteDataManagement();

        }
        /**
        else // In Printing State
        {
            if (isYesButton)
            {
                ReceiptStateHolder.SetPrintData(PrintData.Yes);
                DataReset_UI_Events.RemovePrintReceipt();
            }
            else if (!isYesButton)
            {
                ReceiptStateHolder.SetPrintData(PrintData.No);
                DataReset_UI_Events.RemovePrintReceipt();
            }

            ReceiptStateHolder.ChangeState(ReceiptState.DataReset);
            CompleteDataManagement();
        }
        **/
    }

    private void CompleteDataManagement()
    {
        switch (ReceiptStateHolder.DoDeleteData)
        {
            case DeleteData.Yes:
                // Logic to reset game data
                DeleteTheData();
                break;
            case DeleteData.No:
                // Logic to not reset game data
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        switch (ReceiptStateHolder.DoPrintData)
        {
            case PrintData.Yes:
                // Logic to print data
                PrintTheData();
                break;
            case PrintData.No:
                // Logic to not print data
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }


    }

    private void DeleteTheData()
    {
        ReceiptStateHolder.DeleteGameData();
    }

    private void PrintTheData()
    {
        ReceiptStateHolder.PrintGameData();
    }

    
}

public static class ReceiptStateHolder
{
    public static ReceiptState CurrentState;

    public static DeleteData DoDeleteData { get; private set; } = DeleteData.No;

    public static PrintData DoPrintData { get; private set; } = PrintData.No;


    public static void ChangeState(ReceiptState newState)
    {
        CurrentState = newState;
    }

    public static void SetDeleteData(DeleteData data)
    {
        DoDeleteData = data;
    }

    public static void SetPrintData(PrintData data)
    {
        DoPrintData = data;
    }

    #region Events
    public static event Action OnDeleteData;

    public static void DeleteGameData()
    {
        OnDeleteData?.Invoke();
    }

    public static event Action OnPrintData;

    public static void PrintGameData()
    {
        OnPrintData?.Invoke();
    }
    #endregion
}

public enum ReceiptState
{
    DataReset,
    PrintData
}

public enum DeleteData
{
    Yes,
    No
}

public enum PrintData
{
    Yes,
    No
}

