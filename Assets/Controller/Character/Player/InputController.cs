using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    private GameController gc;
    private int timePressCount = 0;

    private void Start()
    {
        gc = gameObject.GetComponent<GameController>();
    }

    void Update()
    {
        HandleButtonDown();
        HandleButton();
        HandleButtonUp();
    }

    private void FixedUpdate()
    {
        HandleAxis();
    }

    private void HandleButtonDown()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (gc.viewObj.player2d.GetComponent<PlayerController>().dieuKhien)
                gc.viewObj.player2d.SendMessage("Jump", SendMessageOptions.DontRequireReceiver);
        }

        if (Input.GetButtonDown("Run"))
        {
            if (gc.viewObj.player2d.GetComponent<PlayerController>().dieuKhien)
                gc.viewObj.player2d.SendMessage("RunToggle", SendMessageOptions.DontRequireReceiver);
        }

        if (Input.GetButtonDown("Roll"))
        {
            if (gc.viewObj.player2d.GetComponent<PlayerController>().dieuKhien)
                gc.viewObj.player2d.SendMessage("Roll", SendMessageOptions.DontRequireReceiver);
        }

        if (Input.GetButtonDown("ChangeWeapon"))
        {
            if (gc.viewObj.player2d.GetComponent<PlayerController>().dieuKhien)
                gc.viewObj.player2d.SendMessage("ChangeToWeapon", SendMessageOptions.DontRequireReceiver);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            if (gc.viewObj.player2d.GetComponent<PlayerController>().dieuKhien)
            {
                Vector2 fwd = transform.TransformDirection(Vector2.right * gc.viewObj.player2d.GetComponent<CharacterObject>().faceRight);
                RaycastHit2D hit = Physics2D.Raycast(gc.viewObj.player2d.transform.Find("Collider").transform.position, fwd, 0.5f);
                if (hit)
                {
                    if (hit.transform.CompareTag("Enemy"))
                        if (!hit.transform.GetComponent<EnemyController>().curious && !hit.transform.GetComponent<EnemyController>().detected && !hit.transform.GetComponent<CharacterObject>().holdWeapon && gc.viewObj.player2d.GetComponent<PlayerAttacking>().weapon[PlayerPrefs.GetInt("currentWeaponId")].GetComponent<Weapon>().weaponTypeString.Contains("Sword"))
                        {
                            gc.viewObj.player2d.SendMessage("StealthAttack", SendMessageOptions.DontRequireReceiver);
                            hit.transform.SendMessage("DeathFromBack", SendMessageOptions.DontRequireReceiver);
                            return;
                        }                                  
                }
                gc.viewObj.player2d.SendMessage("Attack", SendMessageOptions.DontRequireReceiver);
            }           
        }

        if (Input.GetButtonDown("Fire2"))
        {
            if (gc.viewObj.player2d.GetComponent<PlayerController>().dieuKhien)
                gc.viewObj.player2d.SendMessage("Skill", SendMessageOptions.DontRequireReceiver);
        }

        if (Input.GetButtonDown("ChangeView"))
        {
            if (gc.viewObj.player2d.GetComponent<PlayerController>().canChangeView)
                gc.viewObj.SendMessage("ChangeView", SendMessageOptions.DontRequireReceiver);
        }

        if (Input.GetButtonDown("BagButton"))
        {
            gc.SendMessage("BagAction", SendMessageOptions.DontRequireReceiver);
        }
    }

    private void HandleButton()
    {
        //Long press attack (Charge attack)
        if (Input.GetButton("Fire1"))
        {
            timePressCount += 1;
        }
    }

    private void HandleButtonUp()
    {
        if (Input.GetButtonUp("Fire1"))
        {
            if (timePressCount >= 60 && gc.viewObj.player2d.GetComponent<PlayerController>().dieuKhien)
            {
                gc.viewObj.player2d.SendMessage("ChargeAttack", SendMessageOptions.DontRequireReceiver);
                timePressCount = 0;
            }
            else
                timePressCount = 0;
        }
    }

    private void HandleAxis()
    {
        if (Input.GetAxis("Horizontal") >= 0.5f || Input.GetAxis("Horizontal") <= 0.5f)
        {
            if (gc.viewObj.player2d.GetComponent<PlayerController>().dieuKhien)
                gc.viewObj.player2d.SendMessage("DiChuyenNhanVat", Input.GetAxis("Horizontal"), SendMessageOptions.DontRequireReceiver);
        }
    }
}
