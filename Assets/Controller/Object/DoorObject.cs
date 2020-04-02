using AudioConfig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorObject : MonoBehaviour
{
    [SerializeField]
    private bool interacable = true;
    public bool open = false;
    [SerializeField]
    private AudioClip openSound, closeSound;
    [SerializeField]
    private GameObject collider2d;
    [SerializeField]
    private string openAnimName, closeAnimName;//ten animation mo va dong
    public float openTime = 1f, closeTime = 1f;

    //Thuc hien dong/mo cua
    public void DoorPerform()
    {
        StartCoroutine(OpenCloseDoor());
    }

    private IEnumerator OpenCloseDoor()
    {
        interacable = false;
        //kiem tra cua dang mo hay dong de thuc hien hanh dong
        if (!open)
        {
            SoundManager.PlaySound(gameObject, openSound);//chay am thanh
            gameObject.GetComponent<Animation>().Play(openAnimName);//chay animation
            yield return new WaitForSeconds(closeTime);//doi
        }
        else
        {
            SoundManager.PlaySound(gameObject, closeSound);
            gameObject.GetComponent<Animation>().Play(closeAnimName);
            yield return new WaitForSeconds(openTime);
        }       
        open = !open;//thay doi trang thai cua
        if (collider2d != null)
            collider2d.SetActive(!collider2d.activeInHierarchy);
        interacable = true;
    }
}
