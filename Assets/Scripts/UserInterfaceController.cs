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
    public Transform ReportBackgroundPanel;
    public Transform PopupBackgroundPanel;

    public Button summonButton;

    public Transform CustomerRequestPanel;
    public TextMeshProUGUI CustomerRequestText;

    public Transform ReportPanel;
    public TextMeshProUGUI ReportPanelReport;
    public Button ReportPanelNextButton;

    public Transform GeneralInformationWindowPopup;
    public TextMeshProUGUI GeneralInformationWindowPopupTitleText;
    public TextMeshProUGUI GeneralInformationWindowPopupContentText;

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

        HideGeneralPopup();
    }

    public void SetCustomerText(string text)
    {
        StartCoroutine(WriteCustomerText(text));
    }

    public void SetReportVisibleState(bool state)
    {
        ReportPanel.gameObject.SetActive(state);
        if (ReportBackgroundPanel.gameObject.activeSelf != state)
        {
            Debug.Log($"Set background state: {state}");
            ReportBackgroundPanel.gameObject.SetActive(state);
        }
    }

    public void ShowGeneralPopup(string title, string content)
    {
        GeneralInformationWindowPopup.gameObject.SetActive(true);
        Debug.Log($"Set background state: True");
        PopupBackgroundPanel.gameObject.SetActive(true);

        GeneralInformationWindowPopupTitleText.SetText(title);
        GeneralInformationWindowPopupContentText.SetText(content);
    }

    public void HideGeneralPopup()
    {
        GeneralInformationWindowPopup.gameObject.SetActive(false);
        Debug.Log($"Set background state: False");
        PopupBackgroundPanel.gameObject.SetActive(false);
    }

    public void SetCustomerRequestVisibleState(bool state)
    {
        CustomerRequestPanel.gameObject.SetActive(state);
    }

    private bool reportHasFailed = false;
    public IEnumerator FillInReportIEnumerator()
    {
        reportHasFailed = false;
        ReportPanelNextButton.enabled = false;
        ReportPanelReport.SetText("");

        int totalFails = 0;

        MonsterProperties monsterProperties = GameController.instance.GetCurrentCustomerRequest().WantedMonsterProperties;
        List<MonsterProperySettings> settings = monsterProperties.otherProperties;

        Dictionary<string, int> currentMonsterProperties = MonsterController.instance.monsterState.ComparableList();

        if (monsterProperties.SizeMatters)
        {
            if (MonsterController.instance.monsterState.currentSize == monsterProperties.MonsterSize)
            {
                WriteOneReportLine($"[Success] The customer wanted a {monsterProperties.MonsterSize.ToString().ToUpper()} creature, and it was");
            }
            else
            {
                WriteOneReportLine($"<color=red>[Fail] The customer wanted a {monsterProperties.MonsterSize.ToString().ToUpper()} creature, but it was not</color>");
                totalFails++;
            }
        }

        if(monsterProperties.ColorMatters)
        {
            if (MonsterController.instance.monsterState.currentColor == monsterProperties.PaletteColor)
            {
                WriteOneReportLine($"[Success] The customer wanted a {monsterProperties.PaletteColor.ToString().ToUpper()} creature, and it was");
            }
            else
            {
                WriteOneReportLine($"<color=red>[Fail] The customer wanted a {monsterProperties.PaletteColor.ToString().ToUpper()} creature, but it was not</color>");
                totalFails++;
            }
        }

        foreach (MonsterProperySettings setting in settings)
        {
            if (setting.AmountRule == MonsterProperySettings.AmountSetting.Exact)
            {
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
                if (currentMonsterProperties.ContainsKey(setting.Trait))
                {
                    WriteOneReportLine($"[Success] The customer wanted a {setting.Trait} creature and it was");
                }
                else
                {
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
            AudioController.instance.PlaySound(8, 0.6f); //fail
        }
        else
        {
            AudioController.instance.PlaySound(7, 0.6f); //success
        }

        ReportPanelNextButton.enabled = true;
    }

    private bool isSimplified = false;
    public void SwitchCustomerRequestText()
    {
        AudioController.instance.PlaySound(0);

        isSimplified = !isSimplified;

        if (isSimplified)
        {
            CustomerRequestText.SetText(GameController.instance.GetCurrentCustomerRequest().WantedMonsterProperties.SimplifiedMonsterDescription());
            return;
        }

        CustomerRequestText.SetText(GameController.instance.GetCurrentCustomerRequest().GetMonsterDescription());
    }

    public void WriteOneReportLine(string text)
    {
        string t = ReportPanelReport.text;
        ReportPanelReport.SetText(t + text + "\n");
        AudioController.instance.PlaySound(2); //on_floor
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
            AudioController.instance.PlaySound(4, 0.6f); //talk

            if (word.Contains("..."))
            {
                yield return new WaitForSeconds(1f);
            }
        }
    }

    public void NextOrRetry()
    {
        AudioController.instance.PlaySound(0); //click
        isSimplified = false;
        GameController.instance.NextCustomer(reportHasFailed);
    }
}
