using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemScript : MonoBehaviour
{
    public string itemName, itemUrl;
    [TextArea]
    public string itemDescription;
    public int atkBonus, matkBonus, defBonus, mdefBonus, hpBonus, energyBonus, healHp, healEnergy, slotId, weaponId;
    public enum ItemType { weapon, comsumable, other };
    public ItemType type;
    public ItemManager itemManager;
    public bool canBeUsed = false;
    public Text stackText;

    private void Awake()
    {
        itemManager = gameObject.GetComponentInParent<ItemManager>();
        itemName = gameObject.name;
        if (itemName.Contains("(Clone)"))
            itemName = itemName.Remove(itemName.Length - 7);
        switch (type)
        {
            case ItemType.weapon:
                itemUrl = "Prefabs/Items/Weapon/" + itemName;
                break;
            case ItemType.comsumable:
                itemUrl = "Prefabs/Items/Comsumable/" + itemName;
                break;
            case ItemType.other:
                itemUrl = "Prefabs/Items/Other/" + itemName;
                break;
        }
        
    }
}
