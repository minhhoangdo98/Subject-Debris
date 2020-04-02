using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioConfig;

public class DealDamageTrigger : MonoBehaviour
{
    public float knockBackPower = 100f, damageAmount, atk = 1, effectDestroyTime = 0.2f;
    public int faceRight = 1;
    [SerializeField]
    public GameObject effectOnTarget;
    public string damageType = "VatLy", target1Tag, target2Tag;

    public void DealDamageInit(float atkInit, string typeOfDamage, GameObject effectOnTargetInit, float effectTime, string target1TagInit, string target2TagInit, int faceRightInit, float kBack)
    {
        atk = atkInit;
        damageType = typeOfDamage;
        effectOnTarget = effectOnTargetInit;
        effectDestroyTime = effectTime;
        target1Tag = target1TagInit;
        target2Tag = target2TagInit;
        faceRight = faceRightInit;
        knockBackPower = kBack;
    }

    public void CopyValueTo(DealDamageTrigger target)
    {
        target.atk = atk;
        target.damageType = damageType;
        target.effectOnTarget = effectOnTarget;
        target.effectDestroyTime = effectDestroyTime;
        target.target1Tag = target1Tag;
        target.target2Tag = target2Tag;
        target.faceRight = faceRight;
        target.knockBackPower = knockBackPower;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(target1Tag) || collision.CompareTag(target2Tag))
        {
            GaySatThuong(collision, effectDestroyTime);
        }
    }

    private void GaySatThuong(Collider2D collision, float effectDestroyTime)
    {
        switch (damageType)
        {
            case "VatLy":
                damageAmount = atk - collision.transform.parent.GetComponent<CharacterStat>().def;
                break;
            case "Phep":
                damageAmount = atk - collision.transform.parent.GetComponent<CharacterStat>().mdef;
                break;
            case "TrueDamage":
                damageAmount = atk;
                break;
        }

        damageAmount = atk;
        if (damageAmount < 1)
            damageAmount = 1;
        if (!collision.transform.parent.GetComponent<CharacterObject>().invisible)
        {
            if (effectOnTarget != null)
            {
                GameObject ef = Instantiate(effectOnTarget, collision.transform);
                SoundManager.SetSoundVolumeToObject(ef);
                ef.transform.position = new Vector3(collision.transform.position.x, collision.transform.position.y + 0.2f, collision.transform.position.z - 0.3f);
                Destroy(ef, effectDestroyTime);
            }
            collision.transform.parent.SendMessage("TakeDamage", damageAmount, SendMessageOptions.DontRequireReceiver);
            collision.transform.parent.SendMessage("GetKBack", knockBackPower, SendMessageOptions.DontRequireReceiver);
        }
    }
}
