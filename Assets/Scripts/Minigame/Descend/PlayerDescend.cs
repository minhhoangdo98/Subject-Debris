using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDescend : MonoBehaviourPun
{
    public float speed = 150f, maxspeed = 1, defaultSpeed;
    public bool grounded = true, death = false;
    public Rigidbody2D r2;
    public Animator anim;
    public bool diChuyen, dieuKhien = true;


    void Start()
    {
        SetValuesStart();
    }

    private void SetValuesStart()
    {
        Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>());
        r2 = gameObject.GetComponent<Rigidbody2D>();//Lay nhan vat
        anim = gameObject.GetComponent<Animator>();//Bien chua animation cho Player
        diChuyen = true;//co the di chuyen
        defaultSpeed = speed;//speed ban dau
        if (photonView.IsMine)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(Random.Range(0f, 255f) / 255, Random.Range(0f, 255f) / 255, Random.Range(0f, 255f) / 255, 1);
            GameObject arrow = Instantiate(Resources.Load("Prefabs/Minigame/Descend/PlayerArrow"), gameObject.transform.position, Quaternion.identity) as GameObject;
            arrow.GetComponent<PlayerArrow>().targetPlayer = gameObject;
        }
    }

    private void Update()
    {
        anim.SetBool("Death", death);//animation khi death
    }

    void FixedUpdate()
    {
        if (photonView.IsMine)
            DiChuyenNhanVat();
    }

    private void DiChuyenNhanVat()
    {
        if (diChuyen && dieuKhien)//neu cho phep di chuyen (diChuyen = true)
        {
            float h = Input.GetAxis("Horizontal");//Lay thong tin nut bam la nut mui ten (Phai: 1, Trai: -1)
            r2.AddForce(Vector2.right * speed * h);//Thay doi vi tri nhan vat dua vao speed va h

            //Ham gioi han toc do di chuyen
            if (r2.velocity.x > maxspeed) //Gioi han toc do di ve ben phai
                r2.velocity = new Vector2(maxspeed, r2.velocity.y);
            if (r2.velocity.x < -maxspeed)// Gioi han toc do di ve ben trai
                r2.velocity = new Vector2(-maxspeed, r2.velocity.y);

            if (grounded)
            {
                r2.velocity = new Vector2(r2.velocity.x * 0.7f, r2.velocity.y);
            }

            if (!grounded)
            {
                r2.velocity = new Vector2(r2.velocity.x * 0.7f, r2.velocity.y);
            }

        }
    }
}
