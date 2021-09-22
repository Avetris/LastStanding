using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

public class LocalizeManager : MonoBehaviour
{
    private static string TAG = "LocalizeManager";
    [SerializeField] private LocalizedStringTable m_LocalizedStringTable;

    #region SINGLETON
    private static LocalizeManager m_Instance;
    public static LocalizeManager Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = FindObjectOfType<LocalizeManager>();
                if (m_Instance == null)
                {
                    GameObject prefab = Resources.Load<GameObject>("Prefabs/LocalizeManager");
                    GameObject instance = Instantiate(prefab, Vector3.zero, Quaternion.identity);

                    // DontDestroyOnLoad(instance);

                    m_Instance = instance.GetComponent<LocalizeManager>();
                }
            }
            return m_Instance;
        }
    }
    #endregion

    private StringTable m_CurrentStringTable;

    private IEnumerator Start()
    {
        m_Instance = this;
        // var tableLoading = m_LocalizedStringTable.GetTable();
        yield return LocalizationSettings.InitializationOperation;
        // m_CurrentStringTable = tableLoading.Result;
        // new LocalizationSettings().SetStringDatabase
    }

    public void SetLocalText(string textKey, TMP_Text textField)
    {

        var operation = LocalizationSettings.StringDatabase.GetLocalizedStringAsync(textKey);

        if (operation.IsDone)
        {
            textField.text = operation.Result;
        }
        else
        {
            operation.Completed += (o) => textField.text = o.Result;
        }

        // if (m_CurrentStringTable[textKey] == null)
        // {
        //     LogManager.Debug(TAG, "SetLocalText", $"Error getting the text: {textKey}");
        //     return m_CurrentStringTable[TextCodes.Error.Generic].LocalizedValue;
        // }
        // return m_CurrentStringTable[textKey].LocalizedValue;
    }

    public (List<String> locales, int selected) GetLocales()
    {
        List<String> locales = new List<String>();
        int selected = 0;
        for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
        {
            var locale = LocalizationSettings.AvailableLocales.Locales[i];
            if (LocalizationSettings.SelectedLocale == locale)
                selected = i;
            locales.Add(locale.name);
        }
        return (locales, selected);
    }

    public void ChangeLocale(int index)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
    }
}