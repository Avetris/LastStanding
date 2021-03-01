using UnityEngine;
using UnityEngine.UI;

public class GameplayButtonsHandler : MonoBehaviour {
    
    [SerializeField]  private Button actionButton = null;

    private void Start() {
        actionButton.interactable = false;
    }
}