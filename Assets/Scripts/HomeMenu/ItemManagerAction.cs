using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemManagerAction : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    public GameObject currentSelectedItem, transNumPanel;
    public bool atHome = false, recycle = false;//recylcle: chuc nang recycle
    public Image itemImage;
    public Text itemNameText, itemInfoText, itemDescriptionText, inputNumText, maxNumText;
    public Button useButton, equipButton, discardButton, toBagButton, toInventoryButton, buttonOkToTrans, buttonRecycle;
    public ItemManager bag, inventory;   

    private void Start()
    {
        if (!recycle)
        {
            useButton.gameObject.SetActive(false);
            discardButton.gameObject.SetActive(false);
            equipButton.gameObject.SetActive(false);
            if (toBagButton != null)
            {
                toBagButton.gameObject.SetActive(false);
                toInventoryButton.gameObject.SetActive(false);
            }
        }
        else
        {
            buttonRecycle.gameObject.SetActive(false);
        }
        itemImage.gameObject.SetActive(false);
        itemNameText.gameObject.SetActive(false);
        itemInfoText.gameObject.SetActive(false);
        itemDescriptionText.gameObject.SetActive(false);       
    }

    public void InfoInit(ItemScript item, ItemManager.ManagerType type)
    {
        currentSelectedItem = item.gameObject;
        itemImage.gameObject.SetActive(true);
        itemNameText.gameObject.SetActive(true);
        itemInfoText.gameObject.SetActive(true);
        itemDescriptionText.gameObject.SetActive(true);
        itemImage.sprite = item.transform.Find("Image").GetComponent<Image>().sprite;
        itemNameText.text = item.itemName;
        itemInfoText.text = "";
        if (item.atkBonus > 0)
            itemInfoText.text = "Attack +" + item.atkBonus + "\n";
        if (item.matkBonus > 0)
            itemInfoText.text += "Energy Attack +" + item.matkBonus + "\n";
        if (item.defBonus > 0)
            itemInfoText.text += "Defend +" + item.defBonus + "\n";
        if (item.mdefBonus > 0)
            itemInfoText.text += "Enegy defend +" + item.mdefBonus + "\n";
        if (item.hpBonus > 0)
            itemInfoText.text += "Health +" + item.hpBonus + "\n";
        if (item.energyBonus > 0)
            itemInfoText.text += "Energy +" + item.energyBonus + "\n";
        if(item.healHp>0)
            itemInfoText.text += "Heal Hp " + item.healHp + "\n";
        if (item.healEnergy > 0)
            itemInfoText.text += "Recover Energy " + item.healEnergy;
        itemDescriptionText.text = item.itemDescription;
        if (!recycle)
        {
            discardButton.gameObject.SetActive(true);
            switch (item.type)
            {
                case ItemScript.ItemType.comsumable:
                    if (item.canBeUsed && !atHome)//chi dung khi trong mission
                    {
                        useButton.gameObject.SetActive(true);
                        equipButton.gameObject.SetActive(false);
                    }
                    else
                    {
                        useButton.gameObject.SetActive(false);
                        equipButton.gameObject.SetActive(false);
                    }
                    break;
                case ItemScript.ItemType.weapon:
                    if (item.canBeUsed)
                    {
                        useButton.gameObject.SetActive(false);
                        equipButton.gameObject.SetActive(true);
                    }
                    else
                    {
                        useButton.gameObject.SetActive(false);
                        equipButton.gameObject.SetActive(false);
                    }
                    break;
                case ItemScript.ItemType.other:
                    useButton.gameObject.SetActive(false);
                    equipButton.gameObject.SetActive(false);
                    break;
            }
        }
        else
        {
            if (item.itemName != "Energy gem" && item.itemName != "Energy shard")
                buttonRecycle.gameObject.SetActive(true);
            else
                buttonRecycle.gameObject.SetActive(false);
        }

        if(toBagButton!=null)
            switch (type)
            {
                case ItemManager.ManagerType.Inventory:
                    toBagButton.gameObject.SetActive(true);
                    toInventoryButton.gameObject.SetActive(false);
                    break;
                case ItemManager.ManagerType.Bag:
                    toBagButton.gameObject.SetActive(false);
                    toInventoryButton.gameObject.SetActive(true);
                    break;
            }
    }

    public void ButtonToBag()
    {
        transNumPanel.SetActive(true);
        int itemCount = PlayerPrefs.GetInt("Inventory" + currentSelectedItem.GetComponent<ItemScript>().itemName + "count");
        maxNumText.text = itemCount.ToString();
        //bag.CLoneFromItem(currentSelectedItem);
        //inventory.RemoveItem(currentSelectedItem);
        //EmptySelected();
    }

    public void ButtonToInventory()
    {
        transNumPanel.SetActive(true);
        int itemCount = PlayerPrefs.GetInt("Bag" + currentSelectedItem.GetComponent<ItemScript>().itemName + "count");
        maxNumText.text = itemCount.ToString();
        //inventory.CLoneFromItem(currentSelectedItem);
        //bag.RemoveItem(currentSelectedItem);
        //EmptySelected();
    }

    public void ButtonOkToTrans()//Nut ok chuyen item tu bag/inventoy qua inventory/bag
    {
        int maxItem = Int32.Parse(maxNumText.text);
        int numItem = Int32.Parse(inputNumText.text);
        int currentSlot = currentSelectedItem.GetComponent<ItemScript>().slotId;
        string managerName = currentSelectedItem.GetComponent<ItemScript>().itemManager.managerName;
        string currentItemName = currentSelectedItem.GetComponent<ItemScript>().itemName;
        if (numItem <= maxItem)
        {
            for (int i = 0; i < numItem; i++)
            {
                GameObject itemClone = Instantiate(Resources.Load<GameObject>(currentSelectedItem.GetComponent<ItemScript>().itemUrl));
                switch (managerName)
                {
                    case "Bag":
                        inventory.CLoneFromItem(itemClone);
                        if(PlayerPrefs.GetInt("Bag" + currentItemName + "count") == 1)
                        {
                            bag.RemoveItemInSlot(currentSlot);
                        }
                        PlayerPrefs.SetInt("Bag" + currentItemName + "count", maxItem - (i + 1));
                        break;
                    case "Inventory":
                        bag.CLoneFromItem(itemClone);
                        if (PlayerPrefs.GetInt("Inventory" + currentItemName + "count") == 1)
                        {
                            inventory.RemoveItemInSlot(currentSlot);
                        }
                        PlayerPrefs.SetInt("Inventory" + currentItemName + "count", maxItem - (i + 1));
                        break;
                }
                Destroy(itemClone);
            }
            switch (managerName)
            {
                case "Bag":
                    bag.DestroyAllItemSlot();
                    bag.LoadItemIntoSlot();
                    break;
                case "Inventory":
                    inventory.DestroyAllItemSlot();
                    inventory.LoadItemIntoSlot();
                    break;
            }
            transNumPanel.SetActive(false);
            EmptySelected();
        }
    }

    private void EmptySelected()
    {
        currentSelectedItem = null;
        if (!recycle)
        {
            useButton.gameObject.SetActive(false);
            discardButton.gameObject.SetActive(false);
            equipButton.gameObject.SetActive(false);
        }
        else
            buttonRecycle.gameObject.SetActive(false);
        if (toBagButton != null)
        {
            toBagButton.gameObject.SetActive(false);
            toInventoryButton.gameObject.SetActive(false);
        } 
        itemImage.gameObject.SetActive(false);
        itemNameText.gameObject.SetActive(false);
        itemInfoText.gameObject.SetActive(false);
        itemDescriptionText.gameObject.SetActive(false);
    }

    public void ButtonUseItem()
    {
        player.GetComponent<CharacterStat>().atk += currentSelectedItem.GetComponent<ItemScript>().atkBonus;
        player.GetComponent<CharacterStat>().matk += currentSelectedItem.GetComponent<ItemScript>().matkBonus;
        player.GetComponent<CharacterStat>().def += currentSelectedItem.GetComponent<ItemScript>().defBonus;
        player.GetComponent<CharacterStat>().mdef += currentSelectedItem.GetComponent<ItemScript>().mdefBonus;
        player.GetComponent<CharacterStat>().hp += currentSelectedItem.GetComponent<ItemScript>().hpBonus;
        player.GetComponent<CharacterStat>().maxHp += currentSelectedItem.GetComponent<ItemScript>().hpBonus;
        player.GetComponent<CharacterStat>().hp += currentSelectedItem.GetComponent<ItemScript>().healHp;
        if (player.GetComponent<CharacterStat>().hp > player.GetComponent<CharacterStat>().maxHp)
            player.GetComponent<CharacterStat>().hp = player.GetComponent<CharacterStat>().maxHp;
        player.GetComponent<CharacterStat>().energy += currentSelectedItem.GetComponent<ItemScript>().energyBonus;
        player.GetComponent<CharacterStat>().maxEnergy += currentSelectedItem.GetComponent<ItemScript>().energyBonus;
        player.GetComponent<CharacterStat>().energy += currentSelectedItem.GetComponent<ItemScript>().healEnergy;
        if (player.GetComponent<CharacterStat>().energy > player.GetComponent<CharacterStat>().maxEnergy)
            player.GetComponent<CharacterStat>().energy = player.GetComponent<CharacterStat>().maxEnergy;
        bag.RemoveItem(currentSelectedItem);
    }

    public void ButtonEquipItem()
    {
        int currentWeaponId = player.GetComponent<PlayerAttacking>().weaponId;
        int itemWeaponId = currentSelectedItem.GetComponent<ItemScript>().weaponId;
        if (player.GetComponent<PlayerAttacking>().weapon[currentWeaponId].GetComponent<Weapon>().statAdded)
        {
            player.GetComponent<CharacterStat>().atk -= currentSelectedItem.GetComponent<ItemScript>().atkBonus;
            player.GetComponent<CharacterStat>().matk -= currentSelectedItem.GetComponent<ItemScript>().matkBonus;
            player.GetComponent<CharacterStat>().def -= currentSelectedItem.GetComponent<ItemScript>().defBonus;
            player.GetComponent<CharacterStat>().mdef -= currentSelectedItem.GetComponent<ItemScript>().mdefBonus;
            player.GetComponent<CharacterStat>().maxHp -= currentSelectedItem.GetComponent<ItemScript>().hpBonus;
            if (player.GetComponent<CharacterStat>().hp > player.GetComponent<CharacterStat>().maxHp)
                player.GetComponent<CharacterStat>().hp = player.GetComponent<CharacterStat>().maxHp;
            player.GetComponent<CharacterStat>().maxEnergy -= currentSelectedItem.GetComponent<ItemScript>().energyBonus;
            if (player.GetComponent<CharacterStat>().energy > player.GetComponent<CharacterStat>().maxEnergy)
                player.GetComponent<CharacterStat>().energy = player.GetComponent<CharacterStat>().maxEnergy;
            player.GetComponent<PlayerAttacking>().weapon[currentWeaponId].GetComponent<Weapon>().statAdded = false;
        }
        switch (currentWeaponId)
        {
            case 0:
                bag.AddWeaponString("LS Spark");
                break;
            case 1:
                bag.AddWeaponString("R Blue");
                break;
            case 2:
                bag.AddWeaponString("P Lite");
                break;
            case 3:
                bag.AddWeaponString("R Thunder");
                break;
            case 4:
                bag.AddWeaponString("Combat Sword");
                break;
        }
        player.GetComponent<PlayerAttacking>().SetCurrentWeaponId(itemWeaponId);
        bag.RemoveItem(currentSelectedItem);
        EmptySelected();
    }

    public void ButtonDiscard()
    {
        switch (currentSelectedItem.GetComponent<ItemScript>().itemManager.managerName)
        {
            case "Inventory":
                inventory.RemoveItem(currentSelectedItem);
                break;
            case "Bag":
                bag.RemoveItem(currentSelectedItem);
                break;
        }
        EmptySelected();
    }

    public void ButtonRecycle()
    {
        if (bag.slotUsed < bag.maxSlot)
        {
            ButtonDiscard();
            bag.AddKeyString("Energy shard");
        }        
    }
}
