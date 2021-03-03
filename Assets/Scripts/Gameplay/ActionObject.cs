using UnityEngine;
using Mirror;

public abstract class ActionObject : NetworkBehaviour
{
    [SerializeField] protected DialogDisplayHandler dialogDisplayHandler = null;

    protected bool anyDialogOpened = false;
    protected bool isPlayerNear = false;

    public virtual void Start()
    {
        dialogDisplayHandler.OnDialogChange += ChangeDialogStatus;
    }

    private void OnDestroy()
    {
        dialogDisplayHandler.OnDialogChange -= ChangeDialogStatus;
    }

    public void SetIsPlayerNear(bool isNear)
    {
        isPlayerNear = isNear;
    }

    public bool CanUse()
    {
        return isPlayerNear && !anyDialogOpened;
    }

    private void ChangeDialogStatus(bool status)
    {
        anyDialogOpened = status;
    }

    public abstract void OnClick();
}