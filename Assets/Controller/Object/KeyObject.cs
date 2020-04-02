using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AudioConfig;
using UnityEngine.Events;

public class KeyObject : MonoBehaviour
{
    private GameObject textAction;
    [SerializeField]
    private bool interacable = true;
    [SerializeField]
    private AudioClip pickupSound;
    [SerializeField]
    private UnityEvent actionWhenPickup;

    //Thuc hien hanh dong khi nguoi choi bam vao
    private void ActionPerform()
    {
        textAction = GameObject.FindGameObjectWithTag("ActionText");
        if (interacable)
        {
            textAction.GetComponent<Text>().text = "";
            //tao mot gameobject chua am thanh de chay
            GameObject soundObj = Instantiate(Resources.Load<GameObject>("Prefabs/EmptySoundObject"), gameObject.transform.position, Quaternion.identity);
            SoundManager.PlaySound(soundObj, pickupSound);
            Destroy(soundObj, 2f);
            gameObject.SetActive(false);
            actionWhenPickup.Invoke();
            textAction.SetActive(false);
        }
    }

    private void ChangeText(Text actionText)
    {
        actionText.text = "Take";
    }
}
