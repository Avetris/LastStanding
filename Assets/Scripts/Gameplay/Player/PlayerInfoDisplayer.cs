using UnityEngine;
using Mirror;
using TMPro;
using System;

public class PlayerInfoDisplayer : MonoBehaviour
{
    [SerializeField] private Renderer[] m_ColorRenderers = new Renderer[0];
    [SerializeField] private TMP_Text m_NameText = null;

    public void OnNameChangeHandler(string newName)
    {
        m_NameText.text = newName;
    }
}