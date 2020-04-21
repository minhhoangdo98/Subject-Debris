using AudioConfig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LootObject : MonoBehaviour
{
    private ItemManager bag;
    [SerializeField]
    private GameObject itemToAdd;
    [SerializeField]
    private AudioClip lootSound;
    [SerializeField]
    private UnityEvent eventWhenAdd;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerCollider"))
        {
            eventWhenAdd.Invoke();
            Loot();
        }     
    }

    private void Loot()
    {
        bag = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().bag;
        GameObject item = Instantiate(itemToAdd);
        int itemCount = PlayerPrefs.GetInt("Bag" + item.GetComponent<ItemScript>().itemName + "count");
        if (PlayerPrefs.GetInt("BagslotUsed") < bag.maxSlot || itemCount > 0)
        {
            if (bag == null)//neu la goc nhin 3d
                bag = GameObject.FindGameObjectWithTag("Player3d").GetComponent<FirstPersonController>().bag;
            bag.LootItem(item);
            GameObject soundObj = Instantiate(Resources.Load<GameObject>("Prefabs/EmptySoundObject"), gameObject.transform.position, Quaternion.identity);
            SoundManager.PlaySound(soundObj, lootSound);
            Destroy(soundObj, 2f); 
            Destroy(gameObject.transform.parent.gameObject);
        }
        Destroy(item, 0.1f);
    }
}
