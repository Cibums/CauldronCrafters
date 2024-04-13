using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UserInterfaceController : MonoBehaviour
{
    public TextMeshProUGUI Text;

    public static UserInterfaceController instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void SetCustomerText(string text)
    {
        StartCoroutine(WriteText(text));
    }

    IEnumerator WriteText(string text)
    {
        string totalString = string.Empty;

        string[] words = text.Split(' ');

        foreach (string word in words)
        {
            yield return new WaitForSeconds(0.03f * word.Length);
            totalString += word + " ";
            Text.SetText(totalString);

            if (word.Contains("..."))
            {
                yield return new WaitForSeconds(1f);
            }
        }
    }
}
