using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DestroyPoint : MonoBehaviourPun
{
    private DescendMinigame dc;
    private void Start()
    {
        dc = GameObject.FindGameObjectWithTag("GameController").GetComponent<DescendMinigame>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Dat"))
        {
            Destroy(collision.gameObject);
            dc.point++;
            dc.pointNum.text = dc.point.ToString();
        }

        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerDescend>().death = true;
            collision.GetComponent<PlayerDescend>().diChuyen = false;
            Destroy(collision.gameObject, 1f);
            StartCoroutine(CheckGameover());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Dat"))
        {
            Destroy(collision.gameObject);
            dc.point++;
            dc.pointNum.text = dc.point.ToString();
        }

        if (collision.collider.CompareTag("Player"))
        {
            collision.collider.GetComponent<PlayerDescend>().death = true;
            collision.collider.GetComponent<PlayerDescend>().diChuyen = false;
            Destroy(collision.gameObject, 1f);
            StartCoroutine(CheckGameover());
        }
    }

    IEnumerator CheckGameover()
    {
        yield return new WaitForSeconds(1.2f);
        GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
        if (dc.enableMoveCam && player.Length <= 1)
        {
            dc.enableMoveCam = false;
            dc.GetComponent<AudioSource>().Stop();
            dc.readyText.GetComponent<Text>().text = "Match end";
            dc.readyText.SetActive(true);
            Debug.Log("Leave scene");
            StartCoroutine(dc.ToHome());
        }
    }
}
