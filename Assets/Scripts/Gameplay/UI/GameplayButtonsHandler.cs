using UnityEngine;
using UnityEngine.UI;

public class GameplayButtonsHandler : MonoBehaviour
{
    [SerializeField] private Button actionButton = null;
    [SerializeField] private Button settingsButton = null;
    [SerializeField] private PlayerCollisionHandler playerCollisionHandler;

    private void Start()
    {
        actionButton.interactable = false;
        settingsButton.interactable = true;

        actionButton.onClick.AddListener(OnClick);
        settingsButton.onClick.AddListener(OnSettingsClick);
    }

    public void OnClick()
    {
        if (playerCollisionHandler.GeActionTarget() != null)
        {
            playerCollisionHandler.GeActionTarget().GetComponent<ActionObject>().OnClick();
        }
    }
    
    public void OnSettingsClick()
    {
       FindObjectOfType<DialogDisplayHandler>().OpenPanel(Enumerators.DialogType.Settings);
    }

    private void Update()
    {
        if (playerCollisionHandler.GeActionTarget() != null)
        {
            ActionObject actionObject = playerCollisionHandler.GeActionTarget().GetComponent<ActionObject>();

            ChangeActionButtonIteractable(actionObject.CanUse());
        }
        else
        {
            ChangeActionButtonIteractable(false);
        }
    }

    private void ChangeActionButtonIteractable(bool status)
    {
        if(actionButton.interactable != status)
        {
            actionButton.interactable = status;
        }
    }
}