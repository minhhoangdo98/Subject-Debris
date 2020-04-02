using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AudioConfig;
using UnityEngine.Events;

public class LockObject : MonoBehaviour
{
    [SerializeField]
    private bool interacable = true; 
    public bool locked = false, open = false;
    [SerializeField]
    private string lockText= "Locked", openText= "Open", closeText= "Close";
    [SerializeField]
    private float delayTime = 1f, delayPerform = 0f;
    [SerializeField]
    private AudioClip interactSound;
    [SerializeField]
    private UnityEvent actionPerform, actionPerform2;

    public void UnlockObject()
    {
        locked = !locked;
    }

    public void OpenCloseState()
    {
        open = !open;
    }

    //Perform action when interact
    private void ActionPerform()
    {
        if (interacable && !locked)
        {
            StartCoroutine(DelayToOpen());
            actionPerform.Invoke();
        }
    }

    private void ChangeText(Text actionText)
    {
        if (interacable)
        {
            if (!locked)
            {
                if (!open)
                    actionText.text = openText;
                else
                    actionText.text = closeText;
            }
            else
                actionText.text = lockText;
        }       
    }

    IEnumerator DelayToOpen()
    {
        interacable = false;
        if (interactSound != null)
        {
            GameObject soundObj = Instantiate(Resources.Load<GameObject>("Prefabs/EmptySoundObject"), gameObject.transform.position, Quaternion.identity);
            SoundManager.PlaySound(soundObj, interactSound);
            Destroy(soundObj, 2f);
        }
        yield return new WaitForSeconds(delayTime);
        open = !open;
        interacable = true;
        yield return new WaitForSeconds(delayPerform);
        actionPerform2.Invoke();
    }
}

