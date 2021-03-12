using UnityEngine;
using Mirror;
using System;

public class DialogOpenAction : ActionObject
{
    public override void OnClick()
    {
        dialogDisplayHandler.OpenPanel(Constants.DialogType.Customize);
    }
}