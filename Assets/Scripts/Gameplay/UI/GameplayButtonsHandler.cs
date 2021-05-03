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
    }

    public void OnActionClick()
    {
        if (m_PlayerCollisionHandler != null && m_PlayerCollisionHandler.GeActionTarget() != null)
        {
            m_PlayerCollisionHandler.GeActionTarget().GetComponent<ActionObject>().OnClick();
        }
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