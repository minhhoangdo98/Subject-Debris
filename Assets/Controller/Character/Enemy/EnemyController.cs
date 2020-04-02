using AudioConfig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterObject))]
public class EnemyController : MonoBehaviour
{
    [Header("State")]
    public bool patrolling = false;
    public bool detected = false, curious = false, canRoll = true, popUped = false, canPatrol = false;//Phat hiet va nghi vấn, di chuyen quanh mot khu vuc

    [HideInInspector]
    public CharacterObject charObj;
    private RaycastHit2D hit;

    [Header("Distance")]
    [Range(0f, 30f)]
    [SerializeField]
    private float lookDistance = 10f;
    [SerializeField]
    [Range(0f, 20f)]
    private float attackDistance = 5f, avoidDistance = 1.5f;

    [Header("Time")]
    [SerializeField]
    private float idleTime;

    [Header("Loot drop")]
    public GameObject[] lootDrop;

    void Start()
    {
        charObj = gameObject.GetComponent<CharacterObject>();
        charObj.SetValuesStart();
        charObj.isPlayer = false;       
    }

    void Update()
    {
        charObj.SetAnimatiorAndValuesUpdate();
        hit = Physics2D.Raycast(charObj.colider.transform.position, Vector2.right * charObj.faceRight, lookDistance);
        if (hit && hit.collider.CompareTag(charObj.target1Tag))//neu thay nguoi choi
        {
            canPatrol = false;
            PerformDetectedAction();
        }
        else//neu bi vat can hoac khong thay nguoi choi
        {
            detected = false;
        }

        if(charObj.diChuyen && canPatrol && patrolling)
        {
            charObj.DiChuyenNhanVat(charObj.faceRight);
        }
    }

    public void EnablePatrol()
    {
        canPatrol = true;
        patrolling = true;
    }

    private void PerformDetectedAction()
    {
        if (!popUped && charObj.canMove)
        {
            StartCoroutine(PopupExclamationMark());
        }

        if (hit.distance < avoidDistance && charObj.diChuyen && charObj.canMove)//lui lai
        {
            if (canRoll && !charObj.takeDam)
            {
                StopAllCoroutines();
                charObj.attackable = true;
                charObj.weaponAttack = 0;
                gameObject.SendMessage("RollBackward");
            }
            else
            {
                detected = true;
                charObj.atHome = false;
                charObj.holdWeapon = true;
                charObj.weaponAnimId = 1;
                curious = false;
                gameObject.SendMessage("PerformAttackAction");
            }  
        }

        if (hit.distance < attackDistance && hit.distance >= avoidDistance)//Tan cong nguoi choi
        {
            detected = true;
            charObj.atHome = false;
            charObj.holdWeapon = true;
            charObj.weaponAnimId = 1;
            curious = false;
            gameObject.SendMessage("PerformAttackAction");
        }

        if (hit.distance >= attackDistance && curious && charObj.diChuyen && charObj.canMove)//duoi theo nguoi choi
        {
            charObj.atHome = false;
            charObj.holdWeapon = false;
            charObj.weaponAnimId = 0;
            charObj.DiChuyenNhanVat(charObj.faceRight);
        }
    }

    IEnumerator PopupExclamationMark()
    {
        popUped = true;
        GameObject popUpDetect = gameObject.transform.Find("PopupDetect").gameObject;
        popUpDetect.SetActive(true);
        gameObject.SendMessage("MakeVoice", SendMessageOptions.DontRequireReceiver);
        yield return new WaitForSeconds(1f);
        popUpDetect.SetActive(false);
    }

    IEnumerator IdleAfterPatrol()
    {
        patrolling = false;
        charObj.DiChuyenNhanVat(0);
        yield return new WaitForSeconds(idleTime);
        charObj.Flip();
        patrolling = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PatrolPoint") && patrolling && canPatrol)
        {
            StartCoroutine(IdleAfterPatrol());
        }
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
