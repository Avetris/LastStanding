using TMPro;
using UnityEngine;

enum DialogType { None, Announcement, Company, Setting, Account, ErrorDialog };

public class MenuDialogsHandler : MonoBehaviour
{
    private const string TAG = "MenuDialogsHandler";

    [Header("Dialogs")]
    [SerializeField] private GameObject m_AnnouncementDialog = null;
    [SerializeField] private GameObject m_CompanyDialog = null;
    [SerializeField] private GameObject m_SettingDialog = null;
    [SerializeField] private GameObject m_AccountDialog = null;
    [SerializeField] private GameObject m_ErrorDialog = null;
    [SerializeField] private GameObject m_LoadingDialog = null;

    public void OpenDialogFromUI(string dialogName)
    {
        DialogType dialogType = DialogType.None;
        if (System.Enum.TryParse<DialogType>(dialogName, true, out dialogType))
        {
            OpenDialog(dialogType);
        }
        else
        {
            LogManager.Error(TAG, "OpenDialogFromUI", $"Dialog name not found. Dialog name provided: {dialogName}");
        }
    }

    private void OpenDialog(DialogType panelToOpen)
    {
        HideDialogs();

        switch (panelToOpen)
        {
            case DialogType.Announcement: m_AnnouncementDialog?.SetActive(true); break;
            case DialogType.Company: m_CompanyDialog?.SetActive(true); break;
            case DialogType.Setting: m_SettingDialog?.SetActive(true); break;
            case DialogType.Account: m_AccountDialog?.SetActive(true); break;
        }
    }
    public void HideDialogs()
    {
        m_AnnouncementDialog?.SetActive(false);
        m_CompanyDialog?.SetActive(false);
        m_SettingDialog?.SetActive(false);
        m_AccountDialog?.SetActive(false);
        m_ErrorDialog?.SetActive(false);
        m_LoadingDialog?.SetActive(false);
    }

    public void ShowErrorDialog(string textCode)
    {
        LocalizeManager.Instance.SetLocalText(textCode, m_ErrorDialog.GetComponentInChildren<TMP_Text>());
        m_ErrorDialog.SetActive(true);
    }

    public void ShowLoadingDialog(string textCode)
    {
        LocalizeManager.Instance.SetLocalText(textCode, m_LoadingDialog.GetComponentInChildren<TMP_Text>());
        m_LoadingDialog.SetActive(true);
    }
}