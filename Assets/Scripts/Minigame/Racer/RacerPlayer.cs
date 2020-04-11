using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacerPlayer : MonoBehaviourPun
{
    public float speed = 150f, maxspeed = 1, defaultSpeed, speedWithCam = 0f;
    public bool death = false;
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
        gameObject.transform.Translate(new Vector3(Time.deltaTime * speedWithCam, 0));
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
            float h = Input.GetAxis("Vertical");//Lay thong tin nut bam la nut mui ten (len: 1, xuong: -1)
            r2.AddForce(Vector2.up * speed * h);//Thay doi vi tri nhan vat dua vao speed va h

            //Ham gioi han toc do di chuyen
            if (r2.velocity.y > maxspeed) //Gioi han toc do di len
                r2.velocity = new Vector2(r2.velocity.x, maxspeed);
            if (r2.velocity.y < -maxspeed)// Gioi han toc do di ve ben trai
                r2.velocity = new Vector2(r2.velocity.x, -maxspeed);
        }
    }
}
