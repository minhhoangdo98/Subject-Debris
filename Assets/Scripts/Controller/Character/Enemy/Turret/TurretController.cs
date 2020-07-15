using AudioConfig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class TurretController : MonoBehaviour
{
    private EnemyController ene;
    [SerializeField]
    private GameObject bulletPrefab, effectOnTarget, firePoint; 
    [SerializeField]
    private int soLanBan = 1;
    [SerializeField]
    private float delayTimeShoot = 1, bulletSpeed = 15f;
    [SerializeField]
    private AudioClip prepareSound, bulletSound, reloadSound;
    private bool gunReady = false, readyingGun = false;

    private void Start()
    {
        ene = gameObject.GetComponent<EnemyController>();
        if (firePoint == null)
            firePoint = gameObject.transform.Find("FirePoint").gameObject;
        firePoint.SetActive(false);
    }

    private void PerformAttackAction()
    {
        if(gunReady)
        {
            ene.charObj.weaponAnimId = 1;
            if (ene.charObj.attackable && ene.charObj.canAttack)
            {
                StartCoroutine(GunShoot(soLanBan));
            }
        } 
        else
        {
            if (!readyingGun)
                StartCoroutine(PrepareGun());
        }

    }

    IEnumerator GunShoot(int soLanBan)
    {
        ene.charObj.r2.velocity = Vector2.zero;
        ene.charObj.diChuyen = false;
        ene.charObj.attackable = false;
        ene.charObj.weaponAttack = 1;

        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < soLanBan; i++)
        {
            StartCoroutine(ShootAPrefab(effectOnTarget, bulletSound, bulletPrefab, firePoint, bulletSpeed, 2f, "VatLy", ene.charObj.charStat.atk, true));
            yield return new WaitForSeconds(0.1f);
        }
        ene.charObj.weaponAttack = 0;
        yield return new WaitForSeconds(0.5f);
        if (reloadSound != null)
            SoundManager.PlaySound(gameObject, reloadSound);
        ene.charObj.diChuyen = true;
        yield return new WaitForSeconds(delayTimeShoot);
        ene.charObj.attackable = true;
    }

    IEnumerator ShootAPrefab(GameObject effectOnTarget, AudioClip shootSound, GameObject bulletPrefab, GameObject firePoint, float bulletSpeed, float bulletDestroyTime, string damageType, float atkAmount, bool destroyOnCollision)
    {
        firePoint.SetActive(true);
        if (shootSound != null)
            SoundManager.PlaySound(gameObject, shootSound);
        GameObject bull1 = Instantiate(bulletPrefab, firePoint.transform.position, Quaternion.identity);
        bull1.GetComponent<DealDamageTrigger>().DealDamageInit(atkAmount, damageType, effectOnTarget, 0.2f, ene.charObj.target1Tag, ene.charObj.target2Tag, ene.charObj.faceRight, 10);
        bull1.GetComponent<Bullet>().KhoiTaoBullet(bulletSpeed, bulletDestroyTime, ene.charObj.faceRight, destroyOnCollision);
        //Gay tieng dong
        GameObject soundObj = Instantiate(Resources.Load<GameObject>("Prefabs/EmptySoundObject"), firePoint.transform.position, Quaternion.identity);
        Destroy(soundObj, 1f);
        yield return new WaitForSeconds(0.1f);
        firePoint.SetActive(false);
    }

    IEnumerator PrepareGun()
    {
        readyingGun = true;
        if (prepareSound != null)
            SoundManager.PlaySound(gameObject, prepareSound);
        ene.charObj.weaponAnimId = 1;
        yield return new WaitForSeconds(2f);
        gunReady = true;
        readyingGun = false;
    }
}
