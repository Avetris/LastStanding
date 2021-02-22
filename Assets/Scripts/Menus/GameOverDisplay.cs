using Mirror;
using TMPro;
using UnityEngine;

public class GameOverDisplay : MonoBehaviour
{
    [SerializeField] private GameObject gameOverDisplayParent = null;
    [SerializeField] private TMP_Text winnerNameText = null;

    // Start is called before the first frame update
    private void Start()
    {
        GameOverHandler.ClientOnGameOver += CliendHandleGameOver;
    }

    private void OnDestroy()
    {
        GameOverHandler.ClientOnGameOver -= CliendHandleGameOver;
    }

    public void LeaveGame()
    {
        if(NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();
        }
    }

    private void CliendHandleGameOver(string winner)
    {
        winnerNameText.text = $"{winner} Has Won!";
        gameOverDisplayParent.SetActive(true);
    }
}
