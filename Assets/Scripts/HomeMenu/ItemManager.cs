using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    public enum ManagerType { Inventory, Bag };
    public ManagerType type;
    public int slotUsed, maxSlot = 40;
    public string managerName = "Inventory";
    [HideInInspector]
    public GameObject[] itemObject;
    public bool enableUseItem = false;
    [SerializeField]
    private ItemManagerAction managerAction;

    private void OnEnable()
    {
        if (itemObject.Length > 1)
            DestroyAllItemSlot();
        LoadItemIntoSlot();
    }

    //Load toan bo Item va slot
    public void LoadItemIntoSlot()
    {
        itemObject = new GameObject[maxSlot];//khoi tao mang chua item va slot
        slotUsed = 0;//bien dem so luong slot da dung
        //kiem tra loai inventory hay bag de gan managerName cho viec luu du lieu
        switch (type)
        {
            case ManagerType.Inventory:
                managerName = "Inventory";
                break;
            case ManagerType.Bag:
                managerName = "Bag";
                break;
        }
        //Tao slot
        for (int i = 0; i < maxSlot; i++)
        {
            string slotUrl = PlayerPrefs.GetString(managerName + "slotUrl" + i);//slot hien tai
            string slotUrl2 = PlayerPrefs.GetString(managerName + "slotUrl" + (i + 1));//slot tiep theo
            GameObject item;//bien chua prefab item
            if (slotUrl != "" && slotUrl != null)//neu slot hien tai co gia tri
            {
                item = Resources.Load<GameObject>(slotUrl);//load item theo duong dan
                slotUsed++;//tang bien dem so luong slot da dung
            }
            else//nguoc lai rong
                if (slotUrl2 != "" && slotUrl2 != null)//neu slot tiep theo co gia tri thi thuc hien hoan doi gia tri cho slot hien tai
            {
                item = Resources.Load<GameObject>(slotUrl2);//load item theo duong dan
                PlayerPrefs.SetString(managerName + "slotUrl" + i, slotUrl2);//luu gia tri slot hien tai
                PlayerPrefs.SetString(managerName + "slotUrl" + (i + 1), "");//cho gia tri slot tiep theo rong
                slotUsed++;
            }
            else//neu ca 2 slot trong
                item = Resources.Load<GameObject>("Prefabs/Items/Other/Item0");//load item rong
            itemObject[i] = Instantiate(item, gameObject.transform);//tao slot
            itemObject[i].GetComponent<ItemScript>().slotId = i;//dat id cho item trong slot
            itemObject[i].GetComponent<ItemScript>().canBeUsed = enableUseItem;//item co the dung khi dang o bag va khong the dung khi o inventory
            int itemCount = PlayerPrefs.GetInt(managerName + itemObject[i].GetComponent<ItemScript>().itemName + "count");//bien dem so luong item hien co voi ten itemName da luu
            itemObject[i].GetComponent<ItemScript>().stackText.text = itemCount.ToString();//text the hien so luong item
            ItemScript itemT = itemObject[i].GetComponent<ItemScript>();//bien chua tam thoi
            if (itemCount > 1 && itemObject[i].GetComponent<ItemScript>().itemName != "Item0")//neu item nay nhieu hon 1 thi tap hop no vao mot slot
                StackItem(i);
            else if (itemCount == 1 && itemObject[i].GetComponent<ItemScript>().itemName != "Item0")//them su kien click khoi tao thong tin item
                itemObject[i].GetComponent<Button>().onClick.AddListener(() =>
                {
                    managerAction.InfoInit(itemT, type);
                });
        }
        PlayerPrefs.SetInt(managerName + "slotUsed", slotUsed);
    }

    //Tap hop cac item giong nhau vao mot slot
    public void StackItem(int slotIndex)
    {
        for(int i = 0; i < slotUsed; i++)
        {
            if (i != slotIndex && itemObject[i].GetComponent<ItemScript>().itemName.Equals(itemObject[slotIndex].GetComponent<ItemScript>().itemName))
            {
                RemoveItemInSlot(slotIndex);//xoa slot chua item giong item truoc do
                slotUsed--;
                return;
            }
        }
        if (itemObject[slotIndex].GetComponent<ItemScript>().itemName != "Item0")
            itemObject[slotIndex].GetComponent<Button>().onClick.AddListener(() =>
            {
                managerAction.InfoInit(itemObject[slotIndex].GetComponent<ItemScript>(), type);
            });
    }

    //Them item vao slot
    public void AddItemToSlot(GameObject item, int slotIndex)
    {
        GameObject newItem = Instantiate(item, gameObject.transform);
        GameObject oldItem = itemObject[slotIndex];
        itemObject[slotIndex] = newItem;
        PlayerPrefs.SetString(managerName + "slotUrl" + slotIndex, itemObject[slotIndex].GetComponent<ItemScript>().itemUrl);
        //luu so luong item duoc dua vao
        int itemCount = PlayerPrefs.GetInt(managerName + itemObject[slotIndex].GetComponent<ItemScript>().itemName + "count");
        itemCount++;
        PlayerPrefs.SetInt(managerName + itemObject[slotIndex].GetComponent<ItemScript>().itemName + "count", itemCount);
        Destroy(oldItem);
        //Reset
        DestroyAllItemSlot();
        LoadItemIntoSlot();
    }

    //Them item vao slot rong gan nhat
    public void AddItemToLast(GameObject item)
    {
        slotUsed = PlayerPrefs.GetInt(managerName + "slotUsed");
        if (slotUsed < maxSlot)
        {
            slotUsed++;
            AddItemToSlot(item, slotUsed - 1);
        }            
    }
    
    //Clone item tu item dua vao
    public void CLoneFromItem(GameObject item)
    {
        GameObject cloneItem = Resources.Load<GameObject>(item.GetComponent<ItemScript>().itemUrl);
        AddItemToLast(cloneItem);
    }

    public void AddWeaponString(string itemName)
    {
        GameObject item = Resources.Load<GameObject>("Prefabs/Items/Weapon/" + itemName);
        AddItemToLast(item);
    }

    public void AddComsumableString(string itemName)
    {
        GameObject item = Resources.Load<GameObject>("Prefabs/Items/Comsumable/" + itemName);
        AddItemToLast(item);
    }

    public void AddKeyString(string itemName)
    {
        GameObject item = Resources.Load<GameObject>("Prefabs/Items/Other/" + itemName);
        AddItemToLast(item);
    }

    public void LootItem(GameObject item)
    {
        slotUsed = PlayerPrefs.GetInt("BagslotUsed");
        int itemCount = PlayerPrefs.GetInt("Bag" + item.GetComponent<ItemScript>().itemName + "count");
        if (slotUsed < maxSlot)
        {
            PlayerPrefs.SetString("BagslotUrl" + slotUsed, item.GetComponent<ItemScript>().itemUrl);
            //luu so luong item duoc dua vao
            itemCount++;
            PlayerPrefs.SetInt("Bag" + item.GetComponent<ItemScript>().itemName + "count", itemCount);
            slotUsed++;
            PlayerPrefs.SetInt("BagslotUsed", slotUsed);
            DestroyAllItemSlot();
            LoadItemIntoSlot();
        }
        
    }

    public void RemoveItem(GameObject item)
    {
        int slotIndex = item.GetComponent<ItemScript>().slotId;
        int itemCount = PlayerPrefs.GetInt(managerName + itemObject[slotIndex].GetComponent<ItemScript>().itemName + "count");//bien dem so luong item voi ten itenName da luu
        itemCount--;
        if (itemCount < 0)
            itemCount = 0;
        PlayerPrefs.SetInt(managerName + itemObject[slotIndex].GetComponent<ItemScript>().itemName + "count", itemCount);
        if (itemCount <= 0)
        {
            PlayerPrefs.SetString(managerName + "slotUrl" + slotIndex, "");
            GameObject newItem = Instantiate(Resources.Load<GameObject>("Prefabs/Items/Other/Item0"), gameObject.transform);
            GameObject oldItem = itemObject[slotIndex];
            itemObject[slotIndex] = newItem;
            Destroy(oldItem);
        }
        else
            itemObject[slotIndex].GetComponent<ItemScript>().stackText.text = itemCount.ToString();
        DestroyAllItemSlot();
        LoadItemIntoSlot();
    }

    public void RemoveItemInSlot(int slotIndex)
    {
        PlayerPrefs.SetString(managerName + "slotUrl" + slotIndex, "");
        GameObject newItem = Instantiate(Resources.Load<GameObject>("Prefabs/Items/Other/Item0"), gameObject.transform);
        GameObject oldItem = itemObject[slotIndex];
        itemObject[slotIndex] = newItem;
        Destroy(oldItem);
    }

    public void RemoveAllItem()
    {
        for (int i = 0; i < maxSlot; i++)
        {
            PlayerPrefs.SetInt(managerName + itemObject[i].GetComponent<ItemScript>().itemName + "count", 0);
            RemoveItemInSlot(i);        
        }
        DestroyAllItemSlot();
        LoadItemIntoSlot();
    }

    public void DestroyAllItemSlot()
    {
        for (int i = 0; i < maxSlot; i++)
        {
            Destroy(itemObject[i]);
        }
    }
}
