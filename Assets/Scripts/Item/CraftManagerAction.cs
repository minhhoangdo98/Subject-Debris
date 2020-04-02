using AudioConfig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftManagerAction : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    public GameObject currentSelectedItem;
    public Image itemImage, completeImage;
    public Text itemNameText, itemInfoText, itemRequimentText, completeText;
    public Button buttonCraft;

    private void Start()
    {
        buttonCraft.gameObject.SetActive(false);
        itemImage.gameObject.SetActive(false);
        itemNameText.gameObject.SetActive(false);
        itemInfoText.gameObject.SetActive(false);
        itemRequimentText.gameObject.SetActive(false);
    }

    public void InfoInit(CraftItem itemCraft)
    {
        currentSelectedItem = itemCraft.gameObject;
        itemImage.gameObject.SetActive(true);
        itemNameText.gameObject.SetActive(true);
        itemInfoText.gameObject.SetActive(true);
        itemRequimentText.gameObject.SetActive(true);
        itemImage.sprite = itemCraft.transform.Find("Image").GetComponent<Image>().sprite;
        itemNameText.text = itemCraft.item.itemName;
        itemInfoText.text = "";
        if (itemCraft.item.atkBonus > 0)
            itemInfoText.text = "Attack +" + itemCraft.item.atkBonus + "\n";
        if (itemCraft.item.matkBonus > 0)
            itemInfoText.text += "Energy Attack +" + itemCraft.item.matkBonus + "\n";
        if (itemCraft.item.defBonus > 0)
            itemInfoText.text += "Defend +" + itemCraft.item.defBonus + "\n";
        if (itemCraft.item.mdefBonus > 0)
            itemInfoText.text += "Enegy defend +" + itemCraft.item.mdefBonus + "\n";
        if (itemCraft.item.hpBonus > 0)
            itemInfoText.text += "Health +" + itemCraft.item.hpBonus + "\n";
        if (itemCraft.item.energyBonus > 0)
            itemInfoText.text += "Energy +" + itemCraft.item.energyBonus + "\n";
        if (itemCraft.item.healHp > 0)
            itemInfoText.text += "Heal Hp " + itemCraft.item.healHp + "\n";
        if (itemCraft.item.healEnergy > 0)
            itemInfoText.text += "Recover Energy " + itemCraft.item.healEnergy;
        itemRequimentText.text = "Requiment\n";
        for (int i = 0; i < itemCraft.requiredItem.Length; i++)
        {
            itemRequimentText.text += "\t" + itemCraft.requiredItem[i].itemName + "\t x" + itemCraft.requiredItem[i].soLuong + "\n";
        }
        if (KiemTraDuDK(itemCraft))
            buttonCraft.gameObject.SetActive(true);

    }

    //Kiem tra co du dieu kien craft item hay khong
    private bool KiemTraDuDK(CraftItem itemCraft)
    {
        for (int i = 0; i < itemCraft.requiredItem.Length; i++)
        {
            //kiem tra so luong item yeu cau trong bag
            int itemCountInBag = PlayerPrefs.GetInt("Bag" + itemCraft.requiredItem[i].itemName + "count");
            if (itemCraft.requiredItem[i].soLuong > itemCountInBag)
                return false;
        }
        return true;
    }

    private void EmptySelected()
    {
        currentSelectedItem = null;
        buttonCraft.gameObject.SetActive(false);
        itemImage.gameObject.SetActive(false);
        itemNameText.gameObject.SetActive(false);
        itemInfoText.gameObject.SetActive(false);
        itemRequimentText.gameObject.SetActive(false);
    }

    public void ButtonCraft()
    {
        int slotUsed = PlayerPrefs.GetInt("BagslotUsed");
        CraftItem itemCraft = currentSelectedItem.GetComponent<CraftItem>();
        ItemScript item = currentSelectedItem.GetComponent<ItemScript>();
        for (int i = 0; i < itemCraft.requiredItem.Length; i++)
        {
            int itemCountInBag = PlayerPrefs.GetInt("Bag" + itemCraft.requiredItem[i].itemName + "count");
            itemCountInBag -= itemCraft.requiredItem[i].soLuong;//giam so luong xuong
            //neu bang 0 thi remove item khoi bag
            if (itemCountInBag <= 0)
            {
                PlayerPrefs.SetInt("Bag" + itemCraft.requiredItem[i].itemName + "count", 0);
                for (int j = 0; j < slotUsed; j++)
                {
                    string slotUrl = PlayerPrefs.GetString("Bag" + "slotUrl" + j);
                    if (("Prefabs/Items/Other/" + itemCraft.requiredItem[i].itemName) == slotUrl)
                        PlayerPrefs.SetString("Bag" + "slotUrl" + j, "");
                }

            }
            else
            {
                PlayerPrefs.SetInt("Bag" + itemCraft.requiredItem[i].itemName + "count", itemCountInBag);
            }
        }
        //Them item vua craft vao bag
        ItemManager bag = player.GetComponent<PlayerController>().bag;
        if (PlayerPrefs.GetInt("BagslotUsed") < bag.maxSlot)
        {
            bag.LootItem(currentSelectedItem);
        }
        completeImage.sprite = currentSelectedItem.transform.Find("Image").GetComponent<Image>().sprite;
        completeText.text = item.itemName + " Complete!";
        EmptySelected();
    }
}
