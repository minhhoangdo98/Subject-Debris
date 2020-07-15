using AudioConfig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterObject))]
public class BossController : MonoBehaviour
{
    [Header("State")]
    public bool canRoll = true;
    public bool rage = false;

    [HideInInspector]
    public CharacterObject charObj;
    private RaycastHit2D hit;

    [Header("Distance")]
    [Range(0f, 30f)]
    [SerializeField]
    private float closeAttackDistance = 1.5f;
    [SerializeField]
    [Range(0f, 20f)]
    private float rangedAttackDistance = 10f;

    [Header("Bar")]
    [SerializeField]
    private Slider hpBar;

    [Header("Event")]
    [SerializeField]
    UnityEvent eventWhenDefeated;

    [Header("Loot drop")]
    public GameObject[] lootDrop;

    void Start()
    {
        charObj = gameObject.GetComponent<CharacterObject>();
        charObj.SetValuesStart();
    }

    void Update()
    {
        charObj.SetAnimatiorAndValuesUpdate();
        hpBar.value = charObj.charStat.hp;
        hpBar.maxValue = charObj.charStat.maxHp;
        hit = Physics2D.Raycast(charObj.colider.transform.position, Vector2.right * charObj.faceRight, rangedAttackDistance * 2);
        if (hit && hit.collider.CompareTag(charObj.target1Tag))//neu thay nguoi choi
        {
            charObj.holdWeapon = true;
            charObj.weaponAnimId = 1;
            PerformAction();
        }

        if (charObj.charStat.hp <= (charObj.charStat.maxHp / 2))
        {
            rage = true;
            gameObject.SendMessage("RageAction");
        }
    }

    private void PerformAction()
    {
        if (hit.distance < closeAttackDistance)
        {
            gameObject.SendMessage("PerformClosedAction");
        }

        if (hit.distance < rangedAttackDistance && hit.distance >= closeAttackDistance)
        {
            gameObject.SendMessage("PerformRangedAction");
        }

        if (hit.distance >= rangedAttackDistance && charObj.diChuyen && charObj.canMove)//duoi theo nguoi choi
        {
            charObj.DiChuyenNhanVat(charObj.faceRight);
        }
    }

    private void Defeated()
    {
        eventWhenDefeated.Invoke();
        GameObject soundObj = Instantiate(Resources.Load<GameObject>("Prefabs/EmptySoundObject"), gameObject.transform.position, Quaternion.identity);
        SoundManager.PlaySound(soundObj, Resources.Load<AudioClip>("Audio/SoundEffect/UISound/TargetDestroySound"));
        Destroy(soundObj, 2f);
    }

    public void DropLoot()
    {
        StartCoroutine(LootDropAfterDeath());
    }

    IEnumerator LootDropAfterDeath()
    {
        yield return new WaitForSeconds(1);
        for (int i = 0; i < lootDrop.Length; i++)
        {
            GameObject loot = Instantiate(lootDrop[i], gameObject.transform.position, Quaternion.identity);
            loot.transform.position = new Vector3(loot.transform.position.x + (float)i / 2, loot.transform.position.y, loot.transform.position.z);
        }
    }
}
