using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DealDamageTrigger))]
public class Bullet : MonoBehaviour
{
    private float speed = 15f, destroyTime = 2f;
    private int huongBan = 1;
    private DealDamageTrigger de;
    public bool destroyOnCollision = true;

    void Start()
    {
        de = gameObject.GetComponent<DealDamageTrigger>();
        gameObject.GetComponent<Rigidbody2D>().velocity = transform.right * huongBan * speed;
        if (huongBan == -1)
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }
        Destroy(gameObject, destroyTime);
    }

    public void KhoiTaoBullet(float speedInit, float timeToDestroy, int huongBanInit, bool canDestroyOnCollision)
    {
        speed = speedInit;
        destroyTime = timeToDestroy;
        huongBan = huongBanInit;
        destroyOnCollision = canDestroyOnCollision;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (destroyOnCollision)
            if (collision.CompareTag("Dat") || collision.CompareTag(de.target1Tag) || collision.CompareTag(de.target2Tag) || collision.CompareTag("Tuong"))
            {
                if (collision.transform.parent.GetComponent<CharacterObject>() != null && collision.transform.parent.GetComponent<CharacterObject>().invisible)
                    return;
                Destroy(gameObject);
            }               
    }
}
