using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class UserInterfaceController : MonoBehaviour
{
    [InspectorLabel("Overlay UI")]
    public TextMeshProUGUI CustomerRequestText;

    [InspectorLabel("Popup UI")]
    public TextMeshProUGUI InformationWindowTitleText;
    public TextMeshProUGUI InformationWindowDescriptionText;

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
            CustomerRequestText.SetText(totalString);

            if (word.Contains("..."))
            {
                yield return new WaitForSeconds(1f);
            }
        }
    }
}
