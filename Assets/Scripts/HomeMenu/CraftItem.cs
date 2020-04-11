using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftItem : MonoBehaviour
{
    [System.Serializable]
    public class RequimentItem
    {
        public string itemName;
        public int soLuong;
    }

    public ItemScript item;
    public RequimentItem[] requiredItem;

    private void Start()
    {
        item = gameObject.GetComponent<ItemScript>();
    }
}
