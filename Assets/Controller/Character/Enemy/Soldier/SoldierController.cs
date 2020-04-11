using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioConfig;

public class SoldierController : MonoBehaviour
{
    public enum Type { Ranged, Closed};
    public Type soldierType;
    private EnemyController ene;
    [SerializeField]
    private GameObject weapon;
    private bool voiceSound = false;

    private void Start()
    {
        ene = gameObject.GetComponent<EnemyController>();
    }

    #region Action void
    private void PerformAttackAction()
    {
        weapon.GetComponent<Weapon>().WeaponStatInit(ene.charObj);
        switch (soldierType)
        {
            case Type.Ranged:
                ene.charObj.weaponAnimId = 1;
                if (ene.charObj.holdWeapon && ene.charObj.attackable && ene.charObj.grounded && ene.charObj.canAttack && !ene.charObj.roll)
                {
                    StartCoroutine(PerformShoot(2));
                }
                
                break;
            case Type.Closed:
                ene.charObj.DiChuyenNhanVat(ene.charObj.faceRight);
                break;
        }
    }

    private void PerformAvoidAction()
    {
        if (ene.canRoll && !ene.charObj.takeDam)
        {
            switch (soldierType)
            {
                case Type.Ranged:
                    StopAllCoroutines();
                    ene.charObj.attackable = true;
                    ene.charObj.weaponAttack = 0;
                    gameObject.SendMessage("RollBackward");
                    break;
                case Type.Closed:

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
        if (!voiceSound)
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
        GameObject soundObj = Instantiate(Resources.Load<GameObject>("Prefabs/EmptySoundObject"), gameObject.transform.position, Quaternion.identity);
        List<AudioClip> soundList = new List<AudioClip> { Resources.Load<AudioClip>("Audio/SoundEffect/SoldierVoice/EnemySpotted"), Resources.Load<AudioClip>("Audio/SoundEffect/SoldierVoice/Hostiles"), Resources.Load<AudioClip>("Audio/SoundEffect/SoldierVoice/In_Range"), Resources.Load<AudioClip>("Audio/SoundEffect/SoldierVoice/Locked_On_Target"), Resources.Load<AudioClip>("Audio/SoundEffect/SoldierVoice/Target_Spotted") };
        SoundManager.PlayRandomSound(soundObj, soundList);
        Destroy(soundObj, 2f);
        yield return new WaitForSeconds(1.5f);
        voiceSound = false;
    }
    #endregion

    public void DeathFromBack()
    {
        ene.charObj.DiChuyenNhanVat(0);
        ene.charObj.anim.SetBool("DeathFromBack", true);
        ene.charObj.charStat.hp -= ene.charObj.charStat.maxHp * 2;
        if (!ene.charObj.death)
        {
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
            StopAllCoroutines();
        }
        else
        {
            StopAllCoroutines();
            return;
        }
    }
}
