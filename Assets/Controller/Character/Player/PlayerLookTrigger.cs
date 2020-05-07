using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TouchControlsKit;
using UnityEngine.UI;

public class PlayerLookTrigger : MonoBehaviour
{
    [SerializeField]
    private GameObject actionButton, killButton;
    [SerializeField]
    private bool on2D = false, onAction2dPress = false;
    private GameObject collider2d;

    private void OnEnable()
    {
        if (on2D)
            collider2d = gameObject.transform.Find("Collider").gameObject;
    }

    void FixedUpdate()
    {
        if (!on2D)
            CheckObjectForward();
        else
        if (gameObject.GetComponent<CharacterObject>().gc.loadComplete)
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
        if (Physics.Raycast(transform.position, fwd, out hit, 3f))
        {
            //neu truoc mat la object co the tuong tac
            if (hit.transform.CompareTag("InteractObject"))
            {
                actionButton.SetActive(true);
                hit.transform.SendMessage("ChangeText", actionButton.GetComponent<Text>(), SendMessageOptions.DontRequireReceiver);
                if (Input.GetButtonDown("ActionButton") || TCKInput.GetAction("ActionButton", EActionEvent.Press))
                {
                    //goi ham tuong tac cua object do
                    hit.transform.SendMessage("ActionPerform", SendMessageOptions.DontRequireReceiver);
                }
            }
            else
            {
                actionButton.GetComponent<Text>().text = "";
                actionButton.gameObject.SetActive(false);
            }
        }
        else
        {
            actionButton.GetComponent<Text>().text = "";
            actionButton.gameObject.SetActive(false);
        }
    }

    private void CheckObject2D()
    {
        Vector2 uwd = transform.TransformDirection(Vector2.up);
        RaycastHit2D hit = Physics2D.Raycast(collider2d.transform.position, uwd, 5f);
        if (hit)
        {
            //neu truoc mat la object co the tuong tac
            if (hit.transform.CompareTag("InteractObject") || hit.transform.Find("TalkObject"))
            {
                actionButton.SetActive(true);
                hit.transform.SendMessage("ChangeText", actionButton.GetComponentInChildren<Text>(), SendMessageOptions.DontRequireReceiver);
                if (Input.GetButtonDown("ActionButton") || onAction2dPress)
                {
                    //goi ham tuong tac cua object do
                    hit.transform.SendMessage("ActionPerform", SendMessageOptions.DontRequireReceiver);
                    onAction2dPress = false;
                }
            }
            else
            {
                actionButton.GetComponentInChildren<Text>().text = "";
                actionButton.gameObject.SetActive(false);
            }
        }
        else
        {
            actionButton.GetComponentInChildren<Text>().text = "";
            actionButton.gameObject.SetActive(false);
        }

        //Stealth Kill action
        Vector2 rwd = transform.TransformDirection(-Vector2.right * gameObject.GetComponent<CharacterObject>().faceRight);
        RaycastHit2D hit2 = Physics2D.Raycast(gameObject.transform.Find("Collider").transform.position, rwd, 0.5f);
        Button buttonAttack = gameObject.GetComponent<CharacterObject>().gc.touchButton.buttonAttack.GetComponent<Button>();
        ColorBlock colorButton = buttonAttack.colors;
        if (hit2)
        {
            if (hit2.transform.CompareTag("Enemy"))
            {
                if (!hit2.transform.GetComponent<EnemyController>().curious && !hit2.transform.GetComponent<EnemyController>().detected && !hit2.transform.GetComponent<CharacterObject>().holdWeapon && gameObject.GetComponent<PlayerAttacking>().weapon[PlayerPrefs.GetInt("currentWeaponId")].GetComponent<Weapon>().weaponTypeString.Contains("Sword"))
                {
                    killButton.SetActive(true);
                    hit2.transform.SendMessage("ChangeText", killButton.GetComponentInChildren<Text>(), SendMessageOptions.DontRequireReceiver);
                    colorButton.normalColor = new Color((float)255 / 255, (float)0 / 255, (float)0 / 255, (float)150 / 255);
                    buttonAttack.colors = colorButton;
                }
                else
                {
                    colorButton.normalColor = new Color((float)255 / 255, (float)255 / 255, (float)255 / 255, (float)100 / 255);
                    buttonAttack.colors = colorButton;
                    killButton.GetComponentInChildren<Text>().text = "";
                    killButton.gameObject.SetActive(false);
                }
            }
            else
            {
                colorButton.normalColor = new Color((float)255 / 255, (float)255 / 255, (float)255 / 255, (float)100 / 255);
                buttonAttack.colors = colorButton;
                killButton.GetComponentInChildren<Text>().text = "";
                killButton.gameObject.SetActive(false);
            }
        }
        else
        {
            colorButton.normalColor = new Color((float)255 / 255, (float)255 / 255, (float)255 / 255, (float)100 / 255);
            buttonAttack.colors = colorButton;
            killButton.GetComponentInChildren<Text>().text = "";
            killButton.gameObject.SetActive(false);
        }
    }
}
