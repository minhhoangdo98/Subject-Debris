using AudioConfig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialSoldier : MonoBehaviour
{
    private BossController boss;
    [SerializeField]
    private GameObject weapon, firePoint, weapon2, dealDamageCollider;
    [SerializeField]
    private bool ultimateActive = false, ultimateFinish = false, rageMusic = true;
    [Header("Point")]
    [SerializeField]
    private GameObject thunderPointRoot, starPointRoot;
    [SerializeField]
    private Transform[] thunderPoint, startPoint;

    private void Start()
    {
        boss = gameObject.GetComponent<BossController>();
        thunderPoint = thunderPointRoot.GetComponentsInChildren<Transform>();
        startPoint = starPointRoot.GetComponentsInChildren<Transform>(); 
    }

    private void PerformRangedAction()
    {
        weapon.GetComponent<Weapon>().WeaponStatInit(boss.charObj);
        if (!boss.rage)
        {
            int r = Random.Range(0, 10);
            switch (r)
            {
                case 0:
                    if (boss.charObj.holdWeapon && boss.charObj.attackable && boss.charObj.grounded && boss.charObj.canAttack && !boss.charObj.roll)
                    {
                        StartCoroutine(PerformShoot(3));
                    }
                    break;
                case 1:
                    boss.charObj.DiChuyenNhanVat(boss.charObj.faceRight);
                    boss.charObj.Jump();
                    break;
                case 2:
                    if (boss.charObj.holdWeapon && boss.charObj.attackable && boss.charObj.grounded && boss.charObj.canAttack && !boss.charObj.roll)
                    {
                        StartCoroutine(PerformElectricShoot());
                    }
                    break;
                case 3:
                    if (boss.charObj.holdWeapon && boss.charObj.attackable && boss.charObj.grounded && boss.charObj.canAttack && !boss.charObj.roll)
                    {
                        StartCoroutine(PerformShoot(3));
                    }
                    break;
                case 4:
                    boss.charObj.DiChuyenNhanVat(boss.charObj.faceRight);
                    boss.charObj.Jump();
                    break;
                case 5:
                    if (boss.charObj.holdWeapon && boss.charObj.attackable && boss.charObj.grounded && boss.charObj.canAttack && !boss.charObj.roll)
                    {
                        StartCoroutine(PerformShoot(3));
                    }
                    break;
                default:
                    if (boss.charObj.diChuyen && boss.charObj.canMove)//duoi theo nguoi choi
                    {
                        boss.charObj.DiChuyenNhanVat(boss.charObj.faceRight);
                    }
                    break;
            }
        }       
    }

    private void PerformClosedAction()
    {
        weapon2.GetComponent<Weapon>().WeaponStatInit(boss.charObj);
        if (!boss.rage)
        {
            int r = Random.Range(0, 3);
            switch (r)
            {
                case 0:
                    if (boss.canRoll && boss.charObj.diChuyen && boss.charObj.canMove)
                    {
                        StopAllCoroutines();
                        boss.charObj.attackable = true;
                        boss.charObj.weaponAttack = 0;
                        weapon.SetActive(false);
                        weapon2.SetActive(false);
                        gameObject.SendMessage("RollBackward");
                    }
                    else if (!boss.canRoll)
                    {
                        boss.charObj.DiChuyenNhanVat(boss.charObj.faceRight);
                    }
                    break;
                default:
                    if (boss.charObj.holdWeapon && boss.charObj.attackable && boss.charObj.grounded && boss.charObj.canAttack && !boss.charObj.roll)
                    {
                        StartCoroutine(AttackWithSword());
                    }
                    break;
            }
        }       
    }

    private void RageAction()
    {
        if (!ultimateActive && !ultimateFinish && boss.charObj.attackable && boss.charObj.canAttack && boss.charObj.diChuyen)
            StartCoroutine(UltimateSkill(3));
        if (ultimateFinish)
            StartCoroutine(DelayUltimate());
    }

    IEnumerator PerformShoot(int soLanBan)
    {
        weapon.SetActive(true);
        weapon2.SetActive(false);
        boss.charObj.r2.velocity = Vector2.zero;
        boss.charObj.diChuyen = false;
        boss.charObj.attackable = false;
        boss.charObj.anim.SetLayerWeight(0, 1);
        boss.charObj.anim.SetLayerWeight(1, 0);
        boss.charObj.anim.SetLayerWeight(2, 0);
        boss.charObj.weaponAttack = 1;
        yield return new WaitForSeconds(1);
        boss.charObj.weaponAttack = 2;

        for (int i = 0; i < soLanBan; i++)
        {
            StartCoroutine(weapon.GetComponent<Weapon>().ShootAPrefab(Resources.Load<GameObject>("Prefabs/Effect/HitEffect2"), Resources.Load<AudioClip>("Audio/SoundEffect/Gun/cg1"), Resources.Load<GameObject>("Prefabs/Effect/Bullet"), weapon.transform.Find("FirePoint").gameObject, 15f, 2f, "VatLy", boss.charObj.charStat.atk, true));
            yield return new WaitForSeconds(0.1f);
        }

        boss.charObj.diChuyen = true;
        boss.charObj.weaponAttack = 0;
        yield return new WaitForSeconds(0.5f);
        boss.charObj.attackable = true;
        weapon.SetActive(false);
    }

    IEnumerator AttackWithSword()
    {
        weapon.SetActive(false);
        weapon2.SetActive(true);
        dealDamageCollider.GetComponent<DealDamageTrigger>().DealDamageInit(boss.charObj.charStat.atk, "VatLy", Resources.Load<GameObject>("Prefabs/Effect/HitEffect"), 0.2f, boss.charObj.target1Tag, boss.charObj.target2Tag, 1, 0);
        boss.charObj.r2.velocity = Vector2.zero;
        boss.charObj.diChuyen = false;
        boss.charObj.attackable = false;
        weapon2.GetComponent<MeleeWeaponTrail>().enabled = true;
        boss.charObj.anim.SetLayerWeight(0, 1);
        boss.charObj.anim.SetLayerWeight(1, 0);
        boss.charObj.anim.SetLayerWeight(2, 0);
        boss.charObj.weaponAttack = 3;
        List<AudioClip> soundList;
        dealDamageCollider.SetActive(true);
        soundList = new List<AudioClip> { Resources.Load<AudioClip>("Audio/SoundEffect/swordSwing/swordSwing1"), Resources.Load<AudioClip>("Audio/SoundEffect/swordSwing/swordSwing2"), Resources.Load<AudioClip>("Audio/SoundEffect/swordSwing/swordSwing3") };
        SoundManager.PlayRandomSound(weapon2, soundList);
        yield return new WaitForSeconds(0.6f);
        dealDamageCollider.SetActive(false);
        boss.charObj.weaponAttack = 0;
        boss.charObj.attackable = true;
        weapon2.GetComponent<MeleeWeaponTrail>().enabled = false;
        weapon2.SetActive(false);
        boss.charObj.diChuyen = true;
    }

    IEnumerator PerformElectricShoot()
    {
        weapon.SetActive(true);
        weapon2.SetActive(false);
        boss.charObj.r2.velocity = Vector2.zero;
        boss.charObj.diChuyen = false;
        boss.charObj.attackable = false;
        boss.charObj.anim.SetLayerWeight(0, 1);
        boss.charObj.anim.SetLayerWeight(1, 0);
        boss.charObj.anim.SetLayerWeight(2, 0);
        boss.charObj.weaponAttack = 1;

        firePoint.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        GameObject thunderBall = Instantiate(Resources.Load<GameObject>("Prefabs/Effect/ThunderBall"), firePoint.transform.position, Quaternion.identity);
        SoundManager.SetSoundVolumeToObject(thunderBall);
        thunderBall.GetComponent<DealDamageTrigger>().DealDamageInit(boss.charObj.charStat.matk, "Phep", Resources.Load<GameObject>("Prefabs/Effect/HitEffect2"), 0.2f, boss.charObj.target1Tag, boss.charObj.target2Tag, 1, 50);
        thunderBall.transform.position = new Vector3(thunderBall.transform.position.x, thunderBall.transform.position.y, thunderBall.transform.position.z - 0.1f);
        yield return new WaitForSeconds(3f);
        boss.charObj.weaponAttack = 2;
        GameObject thunder = Instantiate(Resources.Load<GameObject>("Prefabs/Effect/Thunder"), thunderBall.transform.position, Quaternion.identity);
        SoundManager.SetSoundVolumeToObject(thunder);
        thunder.GetComponent<DealDamageTrigger>().DealDamageInit(boss.charObj.charStat.matk, "Phep", Resources.Load<GameObject>("Prefabs/Effect/HitEffect2"), 0.2f, boss.charObj.target1Tag, boss.charObj.target2Tag, 1, 50);
        thunder.transform.position = new Vector3(thunder.transform.position.x + (9f * boss.charObj.faceRight), thunder.transform.position.y, thunder.transform.position.z - 0.1f);
        thunder.transform.localEulerAngles = new Vector3(0, 0, 90f * boss.charObj.faceRight);
        Destroy(thunder, 0.5f);
        Destroy(thunderBall, 0.4f);
        firePoint.SetActive(false);

        boss.charObj.diChuyen = true;
        boss.charObj.weaponAttack = 0;
        yield return new WaitForSeconds(0.5f);
        boss.charObj.attackable = true;
        weapon.SetActive(false);
    }

    IEnumerator UltimateSkill(int soLanBan)
    {
        if (rageMusic)
        {
            rageMusic = false;
            GameObject music = GameObject.FindGameObjectWithTag("Music");
            BgmManager.PlayBgm(music, Resources.Load<AudioClip>("Audio/Music/Battle3"));
        }
        ultimateActive = true;
        weapon.SetActive(false);
        weapon2.SetActive(false);
        boss.charObj.invisible = true;
        boss.charObj.r2.velocity = Vector2.zero;
        boss.charObj.diChuyen = false;
        boss.charObj.attackable = false;
        boss.charObj.anim.SetLayerWeight(0, 1);
        boss.charObj.anim.SetLayerWeight(1, 0);
        boss.charObj.anim.SetLayerWeight(2, 0);
        yield return new WaitForSeconds(0.3f);
        boss.charObj.weaponAttack = 4;
        yield return new WaitForSeconds(2f);
        GameObject rageElectric = Instantiate(Resources.Load<GameObject>("Prefabs/Effect/RageElectric"), gameObject.transform.position, Quaternion.identity, gameObject.transform);
        rageElectric.transform.position = new Vector3(rageElectric.transform.position.x, rageElectric.transform.position.y + 1f, rageElectric.transform.position.z - 0.5f);
        SoundManager.SetSoundVolumeToObject(rageElectric);
        yield return new WaitForSeconds(1f);
        boss.charObj.weaponAttack = 0;
        yield return new WaitForSeconds(0.5f);
        boss.charObj.weaponAttack = 1;
        weapon.SetActive(true);
        firePoint.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        for (int i = 0; i < soLanBan; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                SoundManager.PlaySound(weapon, Resources.Load<AudioClip>("Audio/SoundEffect/Effect/Thunder8"));
                GameObject thunderBall = Instantiate(Resources.Load<GameObject>("Prefabs/Effect/ThunderBall2"), firePoint.transform.position, Quaternion.identity);
                SoundManager.SetSoundVolumeToObject(thunderBall);
                thunderBall.GetComponent<DealDamageTrigger>().DealDamageInit(boss.charObj.charStat.matk, "Phep", Resources.Load<GameObject>("Prefabs/Effect/HitEffect2"), 0.2f, boss.charObj.target1Tag, boss.charObj.target2Tag, 1, 50);
                thunderBall.GetComponent<ThunderBall>().timeToStart = 3;
                thunderBall.GetComponent<ThunderBall>().timeEffect = 2;
                thunderBall.transform.position = new Vector3(thunderBall.transform.position.x, thunderBall.transform.position.y, thunderBall.transform.position.z - 0.8f);
                yield return new WaitForSeconds(0.1f);
                thunderBall.GetComponent<ThunderBall>().targetMove = thunderPoint[Random.Range(0, thunderPoint.Length)];
                thunderBall.GetComponent<ThunderBall>().moveable = true;
                thunderBall.GetComponent<ThunderBall>().speed = 10f;
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(6f);
        }
        for (int i = 0; i < 10; i++)
        {
            SoundManager.PlaySound(weapon, Resources.Load<AudioClip>("Audio/SoundEffect/Effect/Thunder8"));
            GameObject thunderBall = Instantiate(Resources.Load<GameObject>("Prefabs/Effect/ThunderBall2"), firePoint.transform.position, Quaternion.identity);
            SoundManager.SetSoundVolumeToObject(thunderBall);
            thunderBall.GetComponent<DealDamageTrigger>().DealDamageInit(boss.charObj.charStat.matk, "Phep", Resources.Load<GameObject>("Prefabs/Effect/HitEffect2"), 0.2f, boss.charObj.target1Tag, boss.charObj.target2Tag, 1, 50);
            thunderBall.GetComponent<ThunderBall>().timeToStart = 3;
            thunderBall.GetComponent<ThunderBall>().timeEffect = 1;
            thunderBall.transform.position = new Vector3(thunderBall.transform.position.x, thunderBall.transform.position.y, thunderBall.transform.position.z - 0.8f);
            thunderBall.transform.localEulerAngles = new Vector3(thunderBall.transform.localEulerAngles.x, thunderBall.transform.localEulerAngles.y, thunderBall.transform.localEulerAngles.z + i * -25);
            yield return new WaitForSeconds(0.1f);
            thunderBall.GetComponent<ThunderBall>().targetMove = startPoint[i + 1];
            thunderBall.GetComponent<ThunderBall>().moveable = true;
            thunderBall.GetComponent<ThunderBall>().speed = 10f;
            yield return new WaitForSeconds(0.1f);
        }
        Destroy(rageElectric, 0.8f);
        yield return new WaitForSeconds(1f);
        boss.charObj.weaponAttack = 0;
        weapon.SetActive(false);
        boss.charObj.invisible = false;
        ultimateFinish = true;
    }

    IEnumerator DelayUltimate()
    {
        ultimateFinish = false;
        boss.charObj.weaponAttack = -1;
        yield return new WaitForSeconds(13f);
        boss.charObj.weaponAttack = 0;
        boss.charObj.diChuyen = true;
        boss.charObj.attackable = true;
        ultimateActive = false;
    }
}
