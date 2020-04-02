using AudioConfig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private enum WeaponType {LaserSword, Sword, Rifle, Pistol}
    [SerializeField]
    private WeaponType type;
    [HideInInspector]
    public string weaponTypeString = "LaserSword";
    public string damageType = "VatLy";
    [SerializeField]
    private GameObject effectOnTarget;

    [Header("Stat Bonus")]
    [SerializeField]
    private int atkBonus;
    [SerializeField]
    private int matkBonus, defBonus, mdefBonus, hpBonus, energyBonus;
    public bool statAdded = false;

    [Header("Gun")]
    [SerializeField]
    private AudioClip bulletSound;
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private float bulletSpeed = 15f, delayTimeShoot = 0.5f;

    [SerializeField]
    private GameObject weaponDealDamageCollider, firePoint;
    private float timeStartCombo = 0;
    private float atk = 1;
    private string target1Tag, target2Tag;
    private MeleeWeaponTrail weaponTrail;
    private CharacterObject charObj;

    public void WeaponStatInit(CharacterObject charObject)
    {
        charObj = charObject;
        if (!statAdded)
        {
            statAdded = true;
            charObj.charStat.atk += atkBonus;
            charObj.charStat.matk += matkBonus;
            charObj.charStat.def += defBonus;
            charObj.charStat.mdef += mdefBonus;
            charObj.charStat.maxHp += hpBonus;
            charObj.charStat.maxEnergy += energyBonus;
            charObj.charStat.hp += hpBonus;
            charObj.charStat.energy += energyBonus;
        }
        
        switch (damageType)
        {
            case "VatLy":
                atk = charObj.charStat.atk;
                break;
            case "Phep":
                atk = charObj.charStat.matk;
                break;
            case "TrueDamage":
                atk = charObj.charStat.atk;
                break;
        }
        target1Tag = charObject.target1Tag;
        target2Tag = charObject.target2Tag;
        weaponTrail = gameObject.GetComponent<MeleeWeaponTrail>();
        switch (type)
        {
            case WeaponType.LaserSword:
            case WeaponType.Sword:
                weaponDealDamageCollider = gameObject.transform.Find("DealDamageCollider").gameObject;
                break;
            case WeaponType.Rifle:
            case WeaponType.Pistol:
                firePoint = gameObject.transform.Find("FirePoint").gameObject;
                break;
        }
        weaponTypeString = type.ToString();
    }

    public void WeaponDealDamageInit()
    {
        if (weaponDealDamageCollider != null)
        {
            weaponDealDamageCollider.GetComponent<DealDamageTrigger>().DealDamageInit(atk, damageType, effectOnTarget, 0.2f, target1Tag, target2Tag, 1, 100);
            weaponDealDamageCollider.SetActive(false);
            if (weaponTrail != null)
                weaponTrail.enabled = false;
        }        
    }

    private void Update()
    {
        switch (type)
        {
            case WeaponType.LaserSword:
            case WeaponType.Sword:
                ComboTimeBreak();
                break;
            case WeaponType.Rifle:

                break;
        }        
    }

    public void WeaponAttack()
    {
        switch (type)
        {
            case WeaponType.LaserSword:
                StartCoroutine(LaserSwordAttack());
                break;
            case WeaponType.Sword:
                StartCoroutine(LaserSwordAttack());
                break;
            case WeaponType.Rifle:
            case WeaponType.Pistol:
                StartCoroutine(GunShoot());
                break;
        }
    }

    public void WeaponAirAttack()
    {
        switch (type)
        {
            case WeaponType.LaserSword:
                StartCoroutine(LaserSwordAirAttack());
                break;
            case WeaponType.Sword:
                StartCoroutine(LaserSwordAirAttack());
                break;
            case WeaponType.Rifle:

                break;
        }
    }

    public void WeaponChargeAttack()
    {
        switch (type)
        {
            case WeaponType.LaserSword:
                StartCoroutine(LaserSwordChargedAttack());
                break;
            case WeaponType.Sword:

                break;
            case WeaponType.Rifle:

                break;
        }
    }

    public void WeaponSkillAttack()
    {
        switch (gameObject.name)
        {
            case "LS Spark":
                StartCoroutine(LaserSwordSkill());
                break;
            case "R Thunder":

                break;
            case "Combat Sword":

                break;
            default:
                WeaponAttack();
                break;
        }
    }

    public void WeaponStealthAttack()
    {
        switch (type)
        {
            case WeaponType.LaserSword:
            case WeaponType.Sword:
                StartCoroutine(SwordSteathAttack());
                break;
        }
    }
    //kiem tra thoi gian bam nut tan cong trong combo, neu qua thoi gian thi tro ve trang thai dung binh thuong
    private void ComboTimeBreak()
    {
        if (charObj.attackable && charObj.canAttack && charObj.weaponAttack > 0)
        {
            float cancelTime = Time.timeSinceLevelLoad - timeStartCombo;//Thoi gian ma nguoi choi khong bam lai nut tan cong
            if (cancelTime > 0.5f)
            {
                charObj.diChuyen = true;
                weaponTrail.enabled = false;
            }

            if (cancelTime > 0.8f)
                charObj.weaponAttack = 0;
        }
    }
    #region Sword Attack
    IEnumerator LaserSwordAttack()
    {
        timeStartCombo = Time.timeSinceLevelLoad;//cap nhat lai thoi gian dem delay combo
        charObj.r2.velocity = Vector2.zero;
        charObj.diChuyen = false;
        charObj.attackable = false;
        weaponTrail.enabled = true;
        charObj.anim.SetLayerWeight(0, 1);
        charObj.anim.SetLayerWeight(1, 0);
        charObj.anim.SetLayerWeight(2, 0);
        charObj.weaponAttack++;
        List<AudioClip> soundList;
        yield return new WaitForSeconds(0.1f);
        soundList = new List<AudioClip> { Resources.Load<AudioClip>("Audio/SoundEffect/swordSwing/swordSwing1"), Resources.Load<AudioClip>("Audio/SoundEffect/swordSwing/swordSwing2"), Resources.Load<AudioClip>("Audio/SoundEffect/swordSwing/swordSwing3") };
        SoundManager.PlayRandomSound(gameObject, soundList);

        weaponDealDamageCollider.SetActive(true);

        yield return new WaitForSeconds(0.3f);
        weaponDealDamageCollider.SetActive(false);
        if (charObj.weaponAttack >= 5)
        {
            charObj.weaponAttack = 0;
            weaponTrail.enabled = false;
            charObj.diChuyen = true;
        }
        charObj.attackable = true;
    }

    IEnumerator LaserSwordChargedAttack()
    {
        charObj.r2.velocity = Vector2.zero;
        charObj.diChuyen = false;
        charObj.attackable = false;
        weaponTrail.enabled = true;
        charObj.anim.SetLayerWeight(0, 1);
        charObj.anim.SetLayerWeight(1, 0);
        charObj.anim.SetLayerWeight(2, 0);
        charObj.weaponAttack = 7;
        List<AudioClip> soundList;
        weaponDealDamageCollider.SetActive(true);
        soundList = new List<AudioClip> { Resources.Load<AudioClip>("Audio/SoundEffect/swordSwing/swordSwing1"), Resources.Load<AudioClip>("Audio/SoundEffect/swordSwing/swordSwing2"), Resources.Load<AudioClip>("Audio/SoundEffect/swordSwing/swordSwing3") };
        for (int i = 0; i <= 3; i++)
        {
            yield return new WaitForSeconds(0.15f);
            StartCoroutine(ShootAPrefab(Resources.Load<GameObject>("Prefabs/Effect/HitEffect"), null, Resources.Load<GameObject>("Prefabs/Effect/BulletSlash"), weaponDealDamageCollider, 30f, 0.1f, "VatLy", charObj.charStat.atk, false));
            SoundManager.PlayRandomSound(gameObject, soundList);
        }

        yield return new WaitForSeconds(0.6f);
        weaponDealDamageCollider.SetActive(false);
        charObj.weaponAttack = 0;
        charObj.attackable = true;
        weaponTrail.enabled = false;
        charObj.diChuyen = true;
    }

    IEnumerator LaserSwordSkill()
    {
        charObj.r2.velocity = Vector2.zero;
        charObj.diChuyen = false;
        charObj.attackable = false;
        weaponTrail.enabled = true;
        charObj.anim.SetLayerWeight(0, 1);
        charObj.anim.SetLayerWeight(1, 0);
        charObj.anim.SetLayerWeight(2, 0);
        charObj.weaponAttack = 6;
        List<AudioClip> soundList;
        weaponDealDamageCollider.SetActive(true);
        soundList = new List<AudioClip> { Resources.Load<AudioClip>("Audio/SoundEffect/swordSwing/swordSwing1"), Resources.Load<AudioClip>("Audio/SoundEffect/swordSwing/swordSwing2"), Resources.Load<AudioClip>("Audio/SoundEffect/swordSwing/swordSwing3") };
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(ShootAPrefab(Resources.Load<GameObject>("Prefabs/Effect/HitEffect"), null, Resources.Load<GameObject>("Prefabs/Effect/BulletSlash"), weaponDealDamageCollider, 20f, 1f, "Phep", charObj.charStat.matk, false));
        SoundManager.PlayRandomSound(gameObject, soundList);
        charObj.charStat.energy -= 1;
        yield return new WaitForSeconds(0.5f);
        weaponDealDamageCollider.SetActive(false);
        charObj.weaponAttack = 0;
        charObj.attackable = true;
        weaponTrail.enabled = false;
        charObj.diChuyen = true;
    }

    IEnumerator LaserSwordAirAttack()
    {
        charObj.attackable = false;
        weaponTrail.enabled = true;
        charObj.weaponAttack = 1;
        List<AudioClip> soundList;
        yield return new WaitForSeconds(0.1f);
        soundList = new List<AudioClip> { Resources.Load<AudioClip>("Audio/SoundEffect/swordSwing/swordSwing1"), Resources.Load<AudioClip>("Audio/SoundEffect/swordSwing/swordSwing2"), Resources.Load<AudioClip>("Audio/SoundEffect/swordSwing/swordSwing3") };
        SoundManager.PlayRandomSound(gameObject, soundList);

        weaponDealDamageCollider.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        weaponDealDamageCollider.SetActive(false);
        charObj.weaponAttack = 0;
        weaponTrail.enabled = false;
        charObj.attackable = true;
    }

    IEnumerator SwordSteathAttack()
    {
        charObj.r2.velocity = Vector2.zero;
        charObj.diChuyen = false;
        charObj.attackable = false;
        charObj.anim.SetLayerWeight(0, 1);
        charObj.anim.SetLayerWeight(1, 0);
        charObj.anim.SetLayerWeight(2, 0);
        charObj.weaponAttack = 8;
        yield return new WaitForSeconds(0.2f);
        SoundManager.PlaySound(gameObject, Resources.Load<AudioClip>("Audio/SoundEffect/swordSwing/stab-knife-01"));
        yield return new WaitForSeconds(1.3f);
        charObj.weaponAttack = 0;
        charObj.attackable = true;
        charObj.diChuyen = true;
    }
    #endregion

    #region Gun
    private IEnumerator GunShoot()
    {
        charObj.diChuyen = false;
        if (charObj.r2.velocity != Vector2.zero)
        {
            charObj.r2.velocity = Vector2.zero;
            yield return new WaitForSeconds(0.1f);            
        }       
        charObj.attackable = false;
        charObj.anim.SetLayerWeight(0, 1);
        charObj.anim.SetLayerWeight(1, 0);
        charObj.anim.SetLayerWeight(2, 0);
        charObj.weaponAttack = 1;
        StartCoroutine(ShootAPrefab(effectOnTarget, bulletSound, bulletPrefab, firePoint, bulletSpeed, 2f, damageType, atk, true));
        charObj.weaponAttack = 0;
        charObj.diChuyen = true;
        yield return new WaitForSeconds(delayTimeShoot);
        charObj.attackable = true;
        
    }

    public IEnumerator ShootAPrefab(GameObject effectOnTarget, AudioClip shootSound, GameObject bulletPrefab, GameObject firePoint, float bulletSpeed, float bulletDestroyTime, string damageType, float atkAmount, bool destroyOnCollision)
    {
        firePoint.SetActive(true);
        if (shootSound != null)
            SoundManager.PlaySound(gameObject, shootSound);
        GameObject bull1 = Instantiate(bulletPrefab, firePoint.transform.position, Quaternion.identity);
        bull1.GetComponent<DealDamageTrigger>().DealDamageInit(atkAmount, damageType, effectOnTarget, 0.2f, target1Tag, target2Tag, charObj.faceRight, 10);
        bull1.GetComponent<Bullet>().KhoiTaoBullet(bulletSpeed, bulletDestroyTime, charObj.faceRight, destroyOnCollision);
        //Gay tieng dong
        GameObject soundObj = Instantiate(Resources.Load<GameObject>("Prefabs/EmptySoundObject"), firePoint.transform.position, Quaternion.identity);
        Destroy(soundObj, 1f);
        yield return new WaitForSeconds(0.1f);
        firePoint.SetActive(false);
    }
    #endregion
}
