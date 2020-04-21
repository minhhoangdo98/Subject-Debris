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

        if (toBagButton != null)
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

    public void ButtonTrans()
    {
        string managerName = currentSelectedItem.GetComponent<ItemScript>().itemManager.managerName;
        string currentItemName = currentSelectedItem.GetComponent<ItemScript>().itemName;
        int itemCount = PlayerPrefs.GetInt(managerName + currentItemName + "count");
        switch (managerName)
        {
            case "Bag":
                if (inventory.slotUsed >= inventory.maxSlot)
                {
                    GameController gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
                    gc.HienThongBao("Inventory full!");
                    return;
                }
                break;
            case "Inventory":
                if (bag.slotUsed >= bag.maxSlot)
                {
                    GameController gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
                    gc.HienThongBao("Bag full!");
                    return;
                }
                break;
        }
        if (itemCount > 1)
        {
            transNumPanel.SetActive(true);
            maxNumText.text = itemCount.ToString();
        }
        else
        {
            TransItem(1);
        }
    }

    public void ButtonOkToTrans()//Nut ok chuyen item tu bag/inventoy qua inventory/bag
    {
        int maxItem = Int32.Parse(maxNumText.text);
        int numItem = Int32.Parse(inputNumText.text);
        int currentSlot = currentSelectedItem.GetComponent<ItemScript>().slotId;
        string managerName = currentSelectedItem.GetComponent<ItemScript>().itemManager.managerName;
        string currentItemName = currentSelectedItem.GetComponent<ItemScript>().itemName;
        transNumPanel.SetActive(false);
        //kiem tra xem inventory/bag da full chua
        switch (managerName)
        {
            case "Bag":
                if (inventory.slotUsed >= inventory.maxSlot)
                {
                    GameController gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
                    gc.HienThongBao("Inventory full!");
                    return;
                }
                break;
            case "Inventory":
                if (bag.slotUsed >= bag.maxSlot)
                {
                    GameController gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
                    gc.HienThongBao("Bag full!");
                    return;
                }
                break;
        }
        if (numItem > maxItem)//neu so luong item nhap vao lon hon max thi cho bang max
            numItem = maxItem;
        TransItem(numItem);
    }

    private void TransItem(int numItem)
    {
        int currentSlot = currentSelectedItem.GetComponent<ItemScript>().slotId;
        string managerName = currentSelectedItem.GetComponent<ItemScript>().itemManager.managerName;
        string currentItemName = currentSelectedItem.GetComponent<ItemScript>().itemName;
        int maxItem = PlayerPrefs.GetInt(managerName + currentItemName + "count");
        //tao item clone de lay du lieu
        GameObject itemClone = Instantiate(Resources.Load<GameObject>(currentSelectedItem.GetComponent<ItemScript>().itemUrl));
        int rootItemCount = maxItem - numItem;
        //them item clone vao inventory/bag va xoa item tu bag/inventory
        switch (managerName)
        {
            case "Bag":
                PlayerPrefs.SetInt("Bag" + currentItemName + "count", rootItemCount);
                if (rootItemCount <= 0)
                {
                    bag.RemoveItemInSlot(currentSlot);
                }
                inventory.ThemItemVoiSoLuong(itemClone, numItem);
                break;
            case "Inventory":
                PlayerPrefs.SetInt("Inventory" + currentItemName + "count", rootItemCount);
                if (rootItemCount <= 0)
                {
                    inventory.RemoveItemInSlot(currentSlot);
                }
                bag.ThemItemVoiSoLuong(itemClone, numItem);
                break;
        }
        //Reload item trong bag/inventory
        switch (managerName)
        {
            case "Bag":
                bag.ReloadManager();
                break;
            case "Inventory":
                inventory.ReloadManager();
                break;
        }
        Destroy(itemClone);
        EmptySelected();
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
        if (bag.slotUsed < bag.maxSlot)
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
            bag.RemoveItem(currentSelectedItem);
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
                case 5:
                    bag.AddWeaponString("Sniper rifle");
                    break;
            }
            player.GetComponent<PlayerAttacking>().SetCurrentWeaponId(itemWeaponId);
            EmptySelected();
        }
        else
        {
            GameController gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
            gc.HienThongBao("Bag full!");
        }
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
        else
        {
            GameController gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
            gc.HienThongBao("Bag full!");
        }
    }
}
