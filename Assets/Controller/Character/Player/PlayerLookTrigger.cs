using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TouchControlsKit;
using UnityEngine.UI;

public class PlayerLookTrigger : MonoBehaviour
{
    [SerializeField]
    private GameObject actionText;
    [SerializeField]
    private bool on2D = false, onAction2dPress = false;

    void FixedUpdate()
    {
        if (!on2D)
            CheckObjectForward();
        else
            CheckObject2D();
    }

    public void PressAction()
    {
        if (on2D)
            onAction2dPress = true;
    }

    public void DePressAction()
    {
        onAction2dPress = false;
    }

    private void CheckObjectForward()
    {
        RaycastHit hit;
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        if (Physics.Raycast(transform.position, fwd, out hit, 1.5f))
        {
            //neu truoc mat la object co the tuong tac
            if (hit.transform.CompareTag("InteractObject"))
            {
                actionText.SetActive(true);
                hit.transform.SendMessage("ChangeText", actionText.GetComponent<Text>(), SendMessageOptions.DontRequireReceiver);
                if (Input.GetButtonDown("ActionButton") || TCKInput.GetAction("ActionButton", EActionEvent.Press))
                {
                    //goi ham tuong tac cua object do
                    hit.transform.SendMessage("ActionPerform", SendMessageOptions.DontRequireReceiver);
                }
            }
            else
            {
                actionText.GetComponent<Text>().text = "";
                actionText.gameObject.SetActive(false);
            }
        }
        else
        {
            actionText.GetComponent<Text>().text = "";
            actionText.gameObject.SetActive(false);
        }
    }

    private void CheckObject2D()
    {
        Vector2 fwd = transform.TransformDirection(Vector2.up);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, fwd, 5f);
        if (hit)
        {
            //neu truoc mat la object co the tuong tac
            if (hit.transform.CompareTag("InteractObject"))
            {
                actionText.SetActive(true);
                hit.transform.SendMessage("ChangeText", actionText.GetComponentInChildren<Text>(), SendMessageOptions.DontRequireReceiver);
                if (Input.GetButtonDown("ActionButton") || onAction2dPress)
                {
                    //goi ham tuong tac cua object do
                    hit.transform.SendMessage("ActionPerform", SendMessageOptions.DontRequireReceiver);
                    onAction2dPress = false;
                }
            }
            else
            {
                actionText.GetComponentInChildren<Text>().text = "";
                actionText.gameObject.SetActive(false);
            }
        }
        else
        {
            actionText.GetComponentInChildren<Text>().text = "";
            actionText.gameObject.SetActive(false);
        }
    }
}
