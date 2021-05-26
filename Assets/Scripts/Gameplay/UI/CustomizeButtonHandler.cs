using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomizeButtonHandler : MonoBehaviour
{
    [SerializeField] private RawImage m_BackgroundImage;
    [SerializeField] private RawImage m_Image;
    [SerializeField] private TMP_Text m_PriceText;
    [SerializeField] private GameObject m_NotAvailableFilterGameObject;
    [SerializeField] private GameObject m_SelectedImageGameObject;

    private PlayerInfo m_PlayerInfo;

    public Enumerators.CustomizeItem m_ItemType;

    public void SetPlayerInfo(PlayerInfo playerInfo)
    {
        m_PlayerInfo = playerInfo;
    }

    public void SetItemType(Enumerators.CustomizeItem type)
    {
        m_ItemType = type;
    }

    public void SetColor(Color color)
    {
        m_Image.color = color;
    }

    public Color GetColor()
    {
        return m_Image.color;
    }

    public void SetImage(Texture texture)
    {
        m_Image.texture = texture;
    }

    public void ChangeAvailability(bool available)
    {
        m_NotAvailableFilterGameObject.SetActive(!available);
    }

    public void ChangeSelected(bool selected)
    {
        m_SelectedImageGameObject.SetActive(selected);
    }

    public void SetPrice(int price)
    {
        if (price == 0)
        {
            m_PriceText.gameObject.SetActive(false);
        }
        else
        {
            m_PriceText.gameObject.SetActive(true);
            m_PriceText.text = $"{price}€";
        }
    }

    public void OnClick()
    {
        switch(m_ItemType)
        {
            case Enumerators.CustomizeItem.Color:
                m_PlayerInfo.CmdSetDisplayColor(m_Image.color);
                break;
        }
    }
}
