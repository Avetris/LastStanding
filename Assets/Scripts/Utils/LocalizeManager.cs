using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;

public class LocalizeManager : MonoBehaviour
{
    [SerializeField] private LocalizedStringTable m_LocalizedStringTable;

    #region SINGLETON
    private static LocalizeManager m_Instance;
    public static LocalizeManager singleton
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

                    DontDestroyOnLoad(instance);

                    m_Instance = instance.GetComponent<LocalizeManager>();
                }
            }
            return m_Instance;
        }
    }
    #endregion

    private StringTable _currentStringTable;

    private IEnumerator Start()
    {
        var tableLoading = m_LocalizedStringTable.GetTable();
        yield return tableLoading;
        _currentStringTable = tableLoading.Result;
    }

    public string GetText(string textKey)
    {
        return _currentStringTable[textKey].LocalizedValue;
    }
}