using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DestroyPointRacer : MonoBehaviourPun
{
    private RacerMinigame rc;
    private void Start()
    {
        rc = GameObject.FindGameObjectWithTag("GameController").GetComponent<RacerMinigame>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Dat"))
        {
            Destroy(collision.gameObject);
            rc.point++;
            rc.pointNum.text = rc.point.ToString();
        }

        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<RacerPlayer>().death = true;
            collision.GetComponent<RacerPlayer>().diChuyen = false;
            Destroy(collision.gameObject, 1f);
            StartCoroutine(CheckGameover());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Dat"))
        {
            Destroy(collision.gameObject);
            rc.point++;
            rc.pointNum.text = rc.point.ToString();
        }

        if (collision.collider.CompareTag("Player"))
        {
            collision.collider.GetComponent<RacerPlayer>().death = true;
            collision.collider.GetComponent<RacerPlayer>().diChuyen = false;
            Destroy(collision.gameObject, 1f);
            StartCoroutine(CheckGameover());
        }
    }

    IEnumerator CheckGameover()
    {
        yield return new WaitForSeconds(1.2f);
        GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
        if (rc.enableMoveCam && player.Length <= 1)
        {
            rc.enableMoveCam = false;
            rc.GetComponent<AudioSource>().Stop();
            rc.readyText.GetComponent<Text>().text = "Match end";
            rc.readyText.SetActive(true);
            Debug.Log("Leave scene");
            StartCoroutine(rc.ToHome());
        }
    }
}
