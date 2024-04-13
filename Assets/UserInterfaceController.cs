using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using System.Linq;
using UnityEngine.UI;

public class UserInterfaceController : MonoBehaviour
{
    [Header("Overlay UI")]
    public Transform BackgroundPanel;
    public Transform CustomerRequestPanel;
    public TextMeshProUGUI CustomerRequestText;

    public Transform ReportPanel;
    public TextMeshProUGUI ReportPanelReport;
    public Button ReportPanelNextButton;

    [Header("Popup UI")]
    public GameObject InformationWindowPanel;
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
        StartCoroutine(WriteCustomerText(text));
    }

    public void SetReportVisibleState(bool state)
    {
        ReportPanel.gameObject.SetActive(state);
        BackgroundPanel.gameObject.SetActive(state);
    }

    public void SetCustomerRequestVisibleState(bool state)
    {
        CustomerRequestPanel.gameObject.SetActive(state);
    }

    private bool reportHasFailed = false;

    public IEnumerator FillInReportIEnumerator()
    {
        ReportPanelNextButton.enabled = false;
        ReportPanelReport.SetText("");

        Debug.Log("Filling Report");

        int totalFails = 0;

        MonsterProperties monsterProperties = GameController.instance.GetCurrentCustomerRequest().WantedMonsterProperties;
        List<MonsterProperySettings> settings = monsterProperties.otherProperties;

        Dictionary<string, int> currentMonsterProperties = MonsterController.instance.ComparableList();

        if (monsterProperties.SizeMatters)
        {
            if (MonsterController.instance.currentSize == monsterProperties.MonsterSize)
            {
                WriteOneReportLine($"[Success] The customer wanted a {monsterProperties.MonsterSize.ToString().ToUpper()} creature, and it was</color>");
            }
            else
            {
                WriteOneReportLine($"<color=red>[Fail] The customer wanted a {monsterProperties.MonsterSize.ToString().ToUpper()} creature, but it was not</color>");
                totalFails++;
            }
        }

        if(monsterProperties.ColorMatters)
        {
            if (MonsterController.instance.currentColor == monsterProperties.PaletteColor)
            {
                WriteOneReportLine($"[Success] The customer wanted a {monsterProperties.PaletteColor.ToString().ToUpper()} creature, and it was</color>");
            }
            else
            {
                WriteOneReportLine($"<color=red>[Fail] The customer wanted a {monsterProperties.PaletteColor.ToString().ToUpper()} creature, but it was not</color>");
                totalFails++;
            }
        }

        foreach (MonsterProperySettings setting in settings)
        {
            Debug.Log("Going through setting " + setting.Trait);

            if (setting.AmountRule == MonsterProperySettings.AmountSetting.Exact)
            {
                Debug.Log(setting.Trait + " should be exactly " + setting.Count);

                string countString = monsterProperties.GetCountLocalization(setting.Count);

                if (currentMonsterProperties.ContainsKey(setting.Trait) && currentMonsterProperties[setting.Trait] == setting.Count)
                {
                    WriteOneReportLine($"[Success] The customer wanted a {countString} {setting.Trait.ToUpper()} creature and you delivered");
                }
                else
                {
                    WriteOneReportLine($"<color=red>[Fail] The customer wanted a {countString} {setting.Trait.ToUpper()} creature, but the creature did not match the request</color>");
                    totalFails++;
                }
            }
            else if (setting.AmountRule == MonsterProperySettings.AmountSetting.Minimum)
            {
                Debug.Log(setting.Trait + " should be minimum " + setting.Count);

                string countString = monsterProperties.GetCountLocalization(setting.Count);

                if (currentMonsterProperties.ContainsKey(setting.Trait) && currentMonsterProperties[setting.Trait] >= setting.Count)
                {
                    WriteOneReportLine($"[Success] The customer wanted a creature that's at least {countString} {setting.Trait.ToUpper()} and the creature was");
                }
                else
                {
                    WriteOneReportLine($"<color=red>[Fail] The customer wanted a creature that's at least {countString} {setting.Trait.ToUpper()}, but your creature were not</color>");
                    totalFails++;
                }
            }
            else if (setting.AmountRule == MonsterProperySettings.AmountSetting.None)
            {
                Debug.Log(setting.Trait + " should be minimum 1");

                if (currentMonsterProperties.ContainsKey(setting.Trait))
                {
                    Debug.Log($"[Success] The customer wanted a {setting.Trait} creature and it was");
                    WriteOneReportLine($"[Success] The customer wanted a {setting.Trait} creature and it was");
                }
                else
                {
                    Debug.Log($"<color=red>[Fail] {setting.Trait}</color>");
                    WriteOneReportLine($"<color=red>[Fail] The customer wanted a {setting.Trait} creature, but it was not</color>");
                    totalFails++;
                }
            }
            else
            {
                WriteOneReportLine($"<color=red>[Fail] The customer wanted a {setting.Trait} creature, but it was not</color>");
                totalFails++;
            }

            yield return new WaitForSeconds(0.8f);
        }

        WriteOneReportLine("");
        WriteOneReportLine($"Total Fails: {totalFails}");

        if (totalFails > 0)
        {
            reportHasFailed = true;
        }

        ReportPanelNextButton.enabled = true;
    }

    public void WriteOneReportLine(string text)
    {
        string t = ReportPanelReport.text;
        Debug.Log(t + text + "\n");
        ReportPanelReport.SetText(t + text + "\n");
    }

    IEnumerator WriteCustomerText(string text)
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

    public void NextOrRetry()
    {
        GameController.instance.NextCustomer(reportHasFailed);
    }
}
