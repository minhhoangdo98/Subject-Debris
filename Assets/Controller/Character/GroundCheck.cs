using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    //Script duoc dung boi Main
    public CharacterObject chara;


    // Use this for initialization
    void Start()
    {
        chara = gameObject.transform.parent.GetComponent<CharacterObject>();
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger == false && collision.CompareTag("Dat"))
        {
            chara.grounded = true;
            chara.enableJumpDouble = false;
        }           
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.isTrigger == false)
        {
            if (collision.CompareTag("Tuong") && !chara.doubleJump)
            {
                chara.enableJumpDouble = true;
            }
            else
            {
                if (collision.CompareTag("Dat"))
                    chara.grounded = true;
            }
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Dat"))
            chara.grounded = false;
    }
}
