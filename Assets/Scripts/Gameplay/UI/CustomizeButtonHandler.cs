using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomizeButtonHandler : MonoBehaviour
{
    [SerializeField] private RawImage backgroundImage;
    [SerializeField] private RawImage image;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private GameObject notAvailableFilterGameObject;
    [SerializeField] private GameObject selectedImageGameObject;

    private PlayerInfo playerInfo;

    public Enumerators.CustomizeItem itemType;

    public void SetPlayerInfo(PlayerInfo playerInfo)
    {
        this.playerInfo = playerInfo;
    }

    public void SetItemType(Enumerators.CustomizeItem type)
    {
        itemType = type;
    }

    public void SetColor(Color color)
    {
        image.color = color;
    }

    public Color GetColor()
    {
        return image.color;
    }

    public void SetImage(Texture texture)
    {
        image.texture = texture;
    }

    public void ChangeAvailability(bool available)
    {
        notAvailableFilterGameObject.SetActive(!available);
    }

    public void ChangeSelected(bool selected)
    {
        selectedImageGameObject.SetActive(selected);
    }

    public void SetPrice(int price)
    {
        if (price == 0)
        {
            priceText.gameObject.SetActive(false);
        }
        else
        {
            priceText.gameObject.SetActive(true);
            priceText.text = $"{price}€";
        }
    }

    public void OnClick()
    {
        switch(itemType)
        {
            case Enumerators.CustomizeItem.Color:
                playerInfo.CmdSetDisplayColor(image.color);
                break;
        }
    }
}
