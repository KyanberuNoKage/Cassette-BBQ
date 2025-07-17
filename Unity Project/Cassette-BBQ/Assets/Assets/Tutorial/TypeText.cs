using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class TypeText : MonoBehaviour
{
    Coroutine _typingCoroutine;

    public void Type(TextMeshProUGUI boxToTypeIn, string textToType, float letterPauseTime = 0.05f)
    {
        boxToTypeIn.text = ""; // Clear the text if there is any, then type.

        // Stop any ongoing typing coroutine before starting another.
        if (_typingCoroutine != null)
        {
            StopCoroutine(_typingCoroutine);
        }

        _typingCoroutine = StartCoroutine(TypeTextCoroutine(boxToTypeIn, textToType, letterPauseTime));
    }

    private IEnumerator TypeTextCoroutine(TextMeshProUGUI boxToTypeIn, string textToType, float letterPauseTime)
    {
        boxToTypeIn.text = ""; // Clear the text box first.

        for (int i = 0; i < textToType.Length; i++)
        {
            char currentChar = textToType[i];

            // Handle escape sequence for newline - \n.
            if (currentChar == '\\' && i + 1 < textToType.Length && textToType[i + 1] == 'n')
            {
                boxToTypeIn.text += '\n';
                i++; // Skip 'n'.
            }
            else
            {
                boxToTypeIn.text += currentChar;
            }

            yield return new WaitForSeconds(letterPauseTime);
        }
    }

}
