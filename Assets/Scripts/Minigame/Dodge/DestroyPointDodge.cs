using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DestroyPointDodge : MonoBehaviourPun
{
    private DodgeMinigame dg;
    private void Start()
    {
        dg = GameObject.FindGameObjectWithTag("GameController").GetComponent<DodgeMinigame>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerDescend>().death = true;
            collision.GetComponent<PlayerDescend>().diChuyen = false;
            Destroy(collision.gameObject, 1f);
            dg.GameoverChecker();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Dat"))
        {
            Destroy(gameObject, 0.1f);
            dg.point++;
            dg.pointNum.text = dg.point.ToString();
        }

        if (collision.collider.CompareTag("Player"))
        {
            collision.collider.GetComponent<PlayerDescend>().death = true;
            collision.collider.GetComponent<PlayerDescend>().diChuyen = false;
            Destroy(collision.gameObject, 1f);
            dg.GameoverChecker();
            Debug.Log("Death Here!");
        }
    }    
}
