using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioConfig;
using UnityEngine.UI;

public class SoldierController : MonoBehaviour
{
    public enum Type { Rifle, Sword, Sniper, Shield};
    public Type soldierType;
    private EnemyController ene;
    [SerializeField]
    private GameObject weapon;
    public bool canVoice = false;
    private bool voiceSound = false;

    private void Start()
    {
        ene = gameObject.GetComponent<EnemyController>();
        StartCoroutine(CapNhatWeapon());
    }

    IEnumerator CapNhatWeapon()
    {
        yield return new WaitForSeconds(1f);
        weapon.SetActive(true);
        weapon.GetComponent<Weapon>().WeaponStatInit(ene.charObj);
    }

    #region Action void
    private void PerformAttackAction()
    {
        switch (soldierType)
        {
            case Type.Rifle:
                ene.charObj.weaponAnimId = 1;
                if (ene.charObj.holdWeapon && ene.charObj.attackable && ene.charObj.grounded && ene.charObj.canAttack && !ene.charObj.roll)
                {
                    StartCoroutine(PerformShoot(3));
                }              
                break;
            case Type.Sword:
                ene.charObj.weaponAnimId = 1;
                if (ene.charObj.holdWeapon && ene.charObj.attackable && ene.charObj.grounded && ene.charObj.canAttack && !ene.charObj.roll)
                {
                    StartCoroutine(PerformSword());
                }
                break;
            case Type.Sniper:
                ene.charObj.weaponAnimId = 1;
                if (ene.charObj.holdWeapon && ene.charObj.attackable && ene.charObj.grounded && ene.charObj.canAttack && !ene.charObj.roll)
                {
                    StartCoroutine(PerformSniper(1));
                }
                break;
        }
    }

    private void PerformAvoidAction()
    {
        if (ene.canRoll && !ene.charObj.takeDam)
        {
            switch (soldierType)
            {
                case Type.Rifle:
                case Type.Sword:
                    StopAllCoroutines();
                    ene.charObj.attackable = true;
                    ene.charObj.weaponAttack = 0;
                    gameObject.SendMessage("RollBackward");
                    break;
            }           
        }
        else
        {
            ene.detected = true;
            ene.charObj.atHome = false;
            ene.charObj.holdWeapon = true;
            ene.curious = false;
            gameObject.SendMessage("PerformAttackAction");
        }
    }

    private void MakeVoice()
    {
        if (canVoice && !voiceSound)
            StartCoroutine(VoiceSound());
    }

    #endregion

    #region Soldier Attack
    IEnumerator PerformShoot(int soLanBan)
    {
        ene.charObj.r2.velocity = Vector2.zero;
        ene.charObj.diChuyen = false;
        ene.charObj.attackable = false;
        ene.canRoll = false;
        ene.charObj.anim.SetLayerWeight(0, 1);
        ene.charObj.anim.SetLayerWeight(1, 0);
        ene.charObj.anim.SetLayerWeight(2, 0);
        yield return new WaitForSeconds(1);
        ene.charObj.weaponAttack = 1;

        for(int i = 0; i < soLanBan; i++)
        {
            StartCoroutine(weapon.GetComponent<Weapon>().ShootAPrefab(Resources.Load<GameObject>("Prefabs/Effect/HitEffect2"), Resources.Load<AudioClip>("Audio/SoundEffect/Gun/cg1"), Resources.Load<GameObject>("Prefabs/Effect/Bullet"), weapon.transform.Find("FirePoint").gameObject, 15f, 2f, "VatLy", ene.charObj.charStat.atk, true));
            yield return new WaitForSeconds(0.1f);
        }

        ene.charObj.diChuyen = true;
        ene.charObj.weaponAttack = 0;
        yield return new WaitForSeconds(0.5f);
        ene.canRoll = true;
        ene.charObj.attackable = true;       
    }    

    IEnumerator VoiceSound()
    {
        voiceSound = true;
        string voicePath = "Audio/SoundEffect/SoldierVoice/";
        GameObject soundObj = Instantiate(Resources.Load<GameObject>("Prefabs/EmptySoundObject"), gameObject.transform.position, Quaternion.identity);
        List<AudioClip> soundList = new List<AudioClip> { Resources.Load<AudioClip>(voicePath + "EnemySpotted"), Resources.Load<AudioClip>(voicePath + "Hostiles"), Resources.Load<AudioClip>(voicePath + "In_Range"), Resources.Load<AudioClip>(voicePath + "Locked_On_Target"), Resources.Load<AudioClip>(voicePath + "Target_Spotted") };
        SoundManager.PlayRandomSound(soundObj, soundList);
        Destroy(soundObj, 2f);
        yield return new WaitForSeconds(1.5f);
        voiceSound = false;
    }

    IEnumerator PerformSword()
    {
        ene.charObj.r2.velocity = Vector2.zero;
        ene.charObj.diChuyen = false;
        ene.charObj.attackable = false;
        weapon.GetComponent<Weapon>().weaponTrail.enabled = true;
        ene.charObj.anim.SetLayerWeight(0, 1);
        ene.charObj.anim.SetLayerWeight(1, 0);
        ene.charObj.anim.SetLayerWeight(2, 0);
        ene.charObj.weaponAttack = 1;
        List<AudioClip> soundList;
        yield return new WaitForSeconds(0.1f);
        soundList = new List<AudioClip> { Resources.Load<AudioClip>("Audio/SoundEffect/swordSwing/swordSwing1"), Resources.Load<AudioClip>("Audio/SoundEffect/swordSwing/swordSwing2"), Resources.Load<AudioClip>("Audio/SoundEffect/swordSwing/swordSwing3") };
        SoundManager.PlayRandomSound(gameObject, soundList);
        weapon.GetComponent<Weapon>().weaponDealDamageCollider.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        weapon.GetComponent<Weapon>().weaponDealDamageCollider.SetActive(false);
        ene.charObj.weaponAttack = 0;
        weapon.GetComponent<Weapon>().weaponTrail.enabled = false;
        ene.charObj.diChuyen = true;
        yield return new WaitForSeconds(1f);
        ene.charObj.attackable = true;
    }

    IEnumerator PerformSniper(int soLanBan)
    {
        ene.charObj.r2.velocity = Vector2.zero;
        ene.charObj.diChuyen = false;
        ene.charObj.attackable = false;
        ene.canRoll = false;
        ene.charObj.anim.SetLayerWeight(0, 1);
        ene.charObj.anim.SetLayerWeight(1, 0);
        ene.charObj.anim.SetLayerWeight(2, 0);
        yield return new WaitForSeconds(1);
        ene.charObj.weaponAttack = 1;

        for (int i = 0; i < soLanBan; i++)
        {
            StartCoroutine(weapon.GetComponent<Weapon>().ShootAPrefab(Resources.Load<GameObject>("Prefabs/Effect/HitEffect2"), Resources.Load<AudioClip>("Audio/SoundEffect/Gun/SniperShot"), Resources.Load<GameObject>("Prefabs/Effect/Bullet"), weapon.transform.Find("FirePoint").gameObject, 30f, 2f, "VatLy", ene.charObj.charStat.atk, true));
            yield return new WaitForSeconds(0.1f);
        }

        ene.charObj.diChuyen = true;
        ene.charObj.weaponAttack = 0;
        yield return new WaitForSeconds(0.5f);
        ene.canRoll = true;
        ene.charObj.attackable = true;
    }
    #endregion

    private void ChangeText(Text actionText)
    {
        actionText.text = "Stealth Kill";
    }

    public void DeathFromBack()
    {
        StartCoroutine(DeathFromStealth());
    }

    IEnumerator DeathFromStealth()
    {   
        ene.charObj.DiChuyenNhanVat(0);
        ene.charObj.anim.SetBool("DeathFromBack", true);
        ene.charObj.charStat.hp -= ene.charObj.charStat.maxHp * 2;
        if (!ene.charObj.death)
        {
            GameObject cam = Camera.main.gameObject;
            cam.GetComponent<CameraFollowPlayer>().offset = new Vector3(0, 0.2f, -6.5f);
            ene.charObj.death = true;
            ene.charObj.takeDam = false;
            ene.charObj.attackable = false;
            ene.charObj.canMove = false;
            ene.charObj.canAttack = false;
            ene.charObj.canJump = false;
            gameObject.tag = "Death";
            ene.charObj.colider.tag = "Death";
            ene.charObj.invisible = true;
            gameObject.SendMessage("DropLoot", SendMessageOptions.DontRequireReceiver);
            yield return new WaitForSeconds(1f);
            cam.GetComponent<CameraFollowPlayer>().offset = new Vector3(0, 0.2f, -10);
            StopAllCoroutines();
        }
        else
        {
            StopAllCoroutines();
            yield break;
        }
    }
}
