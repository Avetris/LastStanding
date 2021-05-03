using Mirror;
using UnityEngine;

namespace Messaging
{
    public struct Notification : NetworkMessage
    {
        public string content;
    }

    public class MessagesManager : MonoBehaviour
    {
        
        // Start is called before the first frame update
        void Start()
        {
            if (!NetworkClient.active) { return; }

            NetworkClient.RegisterHandler<Notification>(OnNotification, false);
        }


        private void OnNotification(Notification notification)
        {
            Debug.Log("Notification received: " + notification.content);
        }
    }
}