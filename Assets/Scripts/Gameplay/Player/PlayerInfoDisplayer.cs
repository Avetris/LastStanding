using UnityEngine;
using Mirror;
using TMPro;
using System;

public class PlayerInfoDisplayer : MonoBehaviour
{
    [SerializeField] private Renderer[] colorRenderers = new Renderer[0];
    [SerializeField] private TMP_Text nameText = null;
    
    private void Awake() {
        GetComponent<PlayerInfo>().ClientOnNameUpdated += OnNameChangeHandler;
        GetComponent<PlayerInfo>().ClientOnColorUpdated += OnColorChangeHandler;
    }

    private void OnDestroy() {        
        GetComponent<PlayerInfo>().ClientOnNameUpdated -= OnNameChangeHandler;
        GetComponent<PlayerInfo>().ClientOnColorUpdated -= OnColorChangeHandler;
    }

    public void OnNameChangeHandler(string newName)
    {
        nameText.text = newName;
    }

    public void OnColorChangeHandler(Color newColor)
    {
        foreach (Renderer renderer in colorRenderers)
        {
            renderer.material.SetColor("_BaseColor", newColor);
        }
    }
}