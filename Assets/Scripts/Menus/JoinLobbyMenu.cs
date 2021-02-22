using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinLobbyMenu : MonoBehaviour
{
   [SerializeField] private GameObject landingPagePanel = null;
   [SerializeField] private TMP_InputField addressInput = null;
   [SerializeField] private Button joinButton = null;

   private void OnEnable() {
        CustomNetworkManager.ClientOnConnected += HandleClientConnect;
        CustomNetworkManager.ClientOnDisconnected += HandleClientDisconnect;
   }

   private void OnDisable() {       
        CustomNetworkManager.ClientOnConnected -= HandleClientConnect;
        CustomNetworkManager.ClientOnDisconnected -= HandleClientDisconnect;
   }

   public void Join() {
       string address = addressInput.text;

       NetworkManager.singleton.networkAddress = address;
       NetworkManager.singleton.StartClient();
       
       joinButton.interactable = false;
   }

   private void HandleClientConnect()
   {
       joinButton.interactable = true;

       gameObject.SetActive(false);
       landingPagePanel.SetActive(false);
   }

   private void HandleClientDisconnect()
   {       
       joinButton.interactable = true;
   }
}
