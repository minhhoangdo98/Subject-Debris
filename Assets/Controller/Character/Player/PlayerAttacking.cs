using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioConfig;
using UnityEngine.UI;

public class PlayerAttacking : MonoBehaviour
{
    private PlayerController player;
    public GameObject weaponParentObject;
    [HideInInspector]
    public GameObject[] weapon; 
    private GameObject[] weaponDealDamageCollider;
    public int weaponId = 0;

    void Start()
    {
        player = gameObject.GetComponent<PlayerController>();
        weaponId = PlayerPrefs.GetInt("currentWeaponId");
        weapon = new GameObject[weaponParentObject.transform.childCount];
        Weapon[] weapont = new Weapon[weaponParentObject.transform.childCount];
        weapont = weaponParentObject.GetComponentsInChildren<Weapon>(true);
        for (int i = 0; i < weapon.Length; i++)
        {
            weapon[i] = weapont[i].gameObject;
            SoundManager.SetSoundVolumeToObject(weapon[i]);
        }
    }

    //Thay doi trang thai sang cam vu khi hoac tay khong
    private void ChangeToWeapon()
    {
        if (player.charObj.canAttack && player.charObj.attackable)
        {
            weaponId = PlayerPrefs.GetInt("currentWeaponId");
            weapon[weaponId].GetComponent<Weapon>().WeaponStatInit(player.charObj);
            player.charObj.gc.touchButton.buttonAttack.SetActive(!player.charObj.gc.touchButton.buttonAttack.activeInHierarchy);
            player.charObj.gc.touchButton.button3DView.GetComponent<Button>().interactable = !player.charObj.gc.touchButton.button3DView.GetComponent<Button>().interactable;
            player.charObj.gc.touchButton.bagButton.SetActive(!player.charObj.gc.touchButton.bagButton.activeInHierarchy);
            weapon[weaponId].SetActive(!weapon[weaponId].activeInHierarchy);
            player.charObj.holdWeapon = !player.charObj.holdWeapon;
            player.charObj.gc.touchButton.buttonRoll.SetActive(!player.charObj.gc.touchButton.buttonRoll.activeInHierarchy);
            player.charObj.gc.touchButton.buttonSkill.SetActive(!player.charObj.gc.touchButton.buttonSkill.activeInHierarchy);
            switch (weaponId)
            {
                case 0://LS Sark (Laser sword)
                    player.charObj.weaponAnimId = 1;
                    break;
                case 1://R Blue (Rifle)
                    player.charObj.weaponAnimId = 2;
                    break;
                case 2://P Lite (Pistol)
                    player.charObj.weaponAnimId = 3;
                    break;
                case 3://R Thunder (Rifle)
                    player.charObj.weaponAnimId = 2;
                    break;
                case 4://Combat Sword
                    player.charObj.weaponAnimId = 1;
                    break;
                case 5://Sniper
                    player.charObj.weaponAnimId = 2;
                    break;
            }
            if (player.charObj.holdWeapon)
                player.charObj.gc.touchButton.buttonSwitchWeapon.GetComponent<Image>().sprite = Resources.Load<Sprite>("Content/UI/Button/ButtonIcon/mark hand");
            else
            {
                player.charObj.weaponAnimId = 0;
                player.charObj.gc.touchButton.buttonSwitchWeapon.GetComponent<Image>().sprite = Resources.Load<Sprite>("Content/UI/Button/ButtonIcon/sword");
            }

        }     
    }

    //Tan cong bang vu khi
    private void Attack()
    {
        if (player.charObj.holdWeapon && player.charObj.attackable && player.charObj.grounded && player.charObj.canAttack && !player.charObj.roll)
        {
            weapon[weaponId].GetComponent<Weapon>().WeaponAttack();      
        }

        if (player.charObj.holdWeapon && player.charObj.attackable && !player.charObj.grounded && player.charObj.canAttack && !player.charObj.roll)
        {
            weapon[weaponId].GetComponent<Weapon>().WeaponAirAttack();
        }
    }

    //Tan cong bam giu
    private void ChargeAttack()
    {
        if (player.charObj.holdWeapon && player.charObj.attackable && player.charObj.grounded && player.charObj.canAttack && !player.charObj.roll)
        {
            weapon[weaponId].GetComponent<Weapon>().WeaponChargeAttack();
        }
    }

    //Skill vu khi
    private void Skill()
    {
        if (player.charObj.holdWeapon && player.charObj.attackable && player.charObj.grounded && player.charObj.canAttack && !player.charObj.roll)
        {
            weapon[weaponId].GetComponent<Weapon>().WeaponSkillAttack();
        }
    }

    private void StealthAttack()
    {
        if (player.charObj.holdWeapon && player.charObj.attackable && player.charObj.grounded && player.charObj.canAttack && !player.charObj.roll)
        {
            weapon[weaponId].GetComponent<Weapon>().WeaponStealthAttack();
        }
    }

    public void SetCurrentWeaponId(int id)
    {
        PlayerPrefs.SetInt("currentWeaponId", id);
        weaponId = id;
    }
}
