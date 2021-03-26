using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;

public class LocalizeManager : MonoBehaviour
{
    [SerializeField] private LocalizedStringTable _localizedStringTable;
    
    #region SINGLETON
    private static LocalizeManager _instance;
    public static LocalizeManager singleton
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<LocalizeManager>();
            }
            return _instance;
        }
    }
    #endregion

    private StringTable _currentStringTable;
    
    private IEnumerator Start()
    {
        var tableLoading = _localizedStringTable.GetTable();
        yield return tableLoading;
        _currentStringTable = tableLoading.Result;
    }

    public string GetText(string textKey)
    {
        return _currentStringTable[textKey].LocalizedValue;
    }
}