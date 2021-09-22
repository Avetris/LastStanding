using System.Collections.Generic;
using Epic.OnlineServices;
using Epic.OnlineServices.Lobby;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class LobbyListMenu : MonoBehaviour
{
    private const string TAG = "MainMenu";

    [Header("Buttons & Data Pickers")]
    [SerializeField] private RangeSlider m_ArrowSpawnNumber = null;
    [SerializeField] private RangeSlider m_ArrowSpawnInterval = null;
    [SerializeField] private RangeSlider m_ArrowCircleCloseRadius = null;
    [SerializeField] private RangeSlider m_ArrowCircleSpawnInterval = null;
    EOSLobby m_EOSLobby;
    List<LobbyDetails> m_FoundLobbies = new List<LobbyDetails>();
    List<Epic.OnlineServices.Lobby.Attribute> lobbyData = new List<Epic.OnlineServices.Lobby.Attribute>();

    private void Start()
    {
        m_EOSLobby = FindObjectOfType<EOSLobby>();

        m_ArrowSpawnNumber.UpdateText();
        m_ArrowSpawnInterval.UpdateText();
        m_ArrowCircleCloseRadius.UpdateText();
        m_ArrowCircleSpawnInterval.UpdateText();

        m_EOSLobby.FindLobbiesSucceeded += OnFindLobbiesSuccess;
    }

    private void OnDestroy()
    {
        m_EOSLobby.FindLobbiesSucceeded -= OnFindLobbiesSuccess;
    }

    private void OnFindLobbiesSuccess(List<LobbyDetails> lobbiesFound)
    {
        m_FoundLobbies = lobbiesFound;
    }

    public void JoinLobby(int index)
    {
        m_EOSLobby.JoinLobby(m_FoundLobbies[index], new string[] { Constants.LobbyName });
    }

    public void FindLobbies()
    {
        List<LobbySearchSetParameterOptions> searchParameterList = new List<LobbySearchSetParameterOptions>();

        searchParameterList.Add(
            CreateLobbySearchParameterOption(
                Enumerators.GameSetting.Arrow_Spawn_Number.ToString(),
                Utils.GetSliderValues(m_ArrowSpawnNumber),
                ComparisonOp.Anyof));


        searchParameterList.Add(
            CreateLobbySearchParameterOption(
                Enumerators.GameSetting.Arrow_Spawn_Interval.ToString(),
                Utils.GetSliderValues(m_ArrowSpawnInterval),
                ComparisonOp.Anyof));


        searchParameterList.Add(
            CreateLobbySearchParameterOption(
                Enumerators.GameSetting.Arrow_Circle_Close_Radius.ToString(),
                Utils.GetSliderValues(m_ArrowCircleCloseRadius),
                ComparisonOp.Anyof));


        searchParameterList.Add(
            CreateLobbySearchParameterOption(
                Enumerators.GameSetting.Arrow_Circle_Spawn_Interval.ToString(),
                Utils.GetSliderValues(m_ArrowCircleSpawnInterval),
                ComparisonOp.Anyof));

        m_EOSLobby.FindLobbies(Constants.LobbySearch, searchParameterList.ToArray());
    }


    private LobbySearchSetParameterOptions CreateLobbySearchParameterOption(string name, AttributeDataValue value, ComparisonOp compOperations)
    {
        return new LobbySearchSetParameterOptions
        {
            ComparisonOp = compOperations,
            Parameter = new AttributeData
            {
                Key = name,
                Value = value
            }
        };
    }
}
