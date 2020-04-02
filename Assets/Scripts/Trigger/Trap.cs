using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField]
    private GameObject effect; 
    [SerializeField]
    private float trapTime = 0.5f, damage;
    private bool delay = false;

    //Khi nguoi choi di vao trigger thi kich hoat trap
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerCollider") && !delay)
        {
            StartCoroutine(ActiveTrap());
        }
    }

    //Tao hieu ung va gay sat thuong cho nguoi choi
    IEnumerator ActiveTrap()
    {
        delay = true;
        yield return new WaitForSeconds(0.25f);
        GameObject ef = Instantiate(effect, gameObject.transform);
        ef.transform.position = gameObject.transform.position;
        ef.GetComponent<DealDamageTrigger>().DealDamageInit(damage, "TrueDamage", null, trapTime, "PlayerCollider", "Destroyable", 1, 10f);
        Destroy(ef, trapTime);
        yield return new WaitForSeconds(trapTime);
        delay = false;
    }
}
