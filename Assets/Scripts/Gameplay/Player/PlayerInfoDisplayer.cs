using UnityEngine;
using Mirror;
using TMPro;
using System;

public class PlayerInfoDisplayer : MonoBehaviour
{
    [SerializeField] private Renderer[] m_ColorRenderers = new Renderer[0];
    [SerializeField] private TMP_Text m_NameText = null;

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
        m_NameText.text = newName;
    }

    public void OnColorChangeHandler(Color newColor)
    {
        // foreach (Renderer renderer in m_ColorRenderers)
        // {
        //     renderer.material.SetColor("_BaseColor", newColor);
        // }
    }
}