using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class GameplayButtonsHandler : MonoBehaviour
{
    [SerializeField] private Button m_ActionButton = null;
    [SerializeField] private Button m_SettingsButton = null;

    private PlayerCollisionHandler m_PlayerCollisionHandler;

    private void Start()
    {
        m_ActionButton.interactable = false;
        m_SettingsButton.interactable = true;

        m_ActionButton.onClick.AddListener(OnClick);
        m_SettingsButton.onClick.AddListener(OnSettingsClick);
    }

    public void OnClick()
    {
        if (m_PlayerCollisionHandler != null && m_PlayerCollisionHandler.GeActionTarget() != null)
        {
            m_PlayerCollisionHandler.GeActionTarget().GetComponent<ActionObject>().OnClick();
        }
    }

    public void OnSettingsClick()
    {
        FindObjectOfType<DialogDisplayHandler>().OpenPanel(Enumerators.DialogType.Settings);
    }

    private void Update()
    {        
        if (NetworkClient.connection.identity == null) { return; }
        if (m_PlayerCollisionHandler == null)
        {
            m_PlayerCollisionHandler = NetworkClient.connection.identity.GetComponent<PlayerCollisionHandler>();
        }
        if (m_PlayerCollisionHandler.GeActionTarget() != null)
        {
            ActionObject actionObject = m_PlayerCollisionHandler.GeActionTarget().GetComponent<ActionObject>();

            ChangeActionButtonIteractable(actionObject.CanUse());
        }
        else
        {
            ChangeActionButtonIteractable(false);
        }
    }

    private void ChangeActionButtonIteractable(bool status)
    {
        if (m_ActionButton.interactable != status)
        {
            m_ActionButton.interactable = status;
        }
    }
}