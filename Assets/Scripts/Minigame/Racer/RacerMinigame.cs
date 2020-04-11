using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RacerMinigame : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private int soLuongPlatform;
    public GameObject cam, respawnPoint, diaHinh, readyText, flash, buttonReady;
    public GameObject[] platform, player, readyObj;
    public bool enableSpawn = true, enableIncreseSpeed = true, enableMoveCam = false, ready = false;
    public float camSpeed = 0.5f, camSpeedIncrease = 0.5f, playerSpeedIncrease = 50, delayTime = 10, delaySpawn = 2.5f;
    public int point = 0;
    public Text pointNum;
    public float minRange = -6.5f, maxRange = 6.5f;

    private void Start()
    {
        soLuongPlatform = platform.Length;
    }

    private void Update()
    {
        if (enableMoveCam)
        {
            cam.transform.Translate(new Vector3(Time.deltaTime * camSpeed, 0));
            player = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject pl in player)
                pl.GetComponent<RacerPlayer>().speedWithCam = camSpeed;
            if (enableIncreseSpeed)
            {
                StartCoroutine(TangTocDo());
            }

            if (enableSpawn && photonView.IsMine)
            {
                StartCoroutine(SpawnPlatform());
            }
        }
        else
        {
            player = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject pl in player)
                pl.GetComponent<RacerPlayer>().speedWithCam = 0;
        }

        if (ready)
        {
            readyObj = GameObject.FindGameObjectsWithTag("ReadyObject");
            player = GameObject.FindGameObjectsWithTag("Player");
            if (readyObj.Length == player.Length)
            {
                ready = false;
                StartCoroutine(ReadyToGame());
            }
        }

    }

    IEnumerator TangTocDo()
    {
        enableIncreseSpeed = false;
        camSpeed += camSpeedIncrease;
        player = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject pl in player)
            pl.GetComponent<RacerPlayer>().speed += playerSpeedIncrease;
        delaySpawn -= 0.3f;
        if (delaySpawn < 0.5f)
            delaySpawn = 0.5f;
        gameObject.GetComponent<AudioSource>().pitch += 0.05f;
        if (gameObject.GetComponent<AudioSource>().pitch > 2)
            gameObject.GetComponent<AudioSource>().pitch = 2;
        yield return new WaitForSeconds(delayTime);
        enableIncreseSpeed = true;
    }

    IEnumerator SpawnPlatform()
    {
        enableSpawn = false;
        int r = Random.Range(0, soLuongPlatform);
        Vector2 pos = new Vector2(respawnPoint.transform.position.x, Random.Range(minRange, maxRange));
        GameObject plat = PhotonNetwork.Instantiate(Path.Combine("Prefabs/Minigame/Racer/", platform[r].name), pos, Quaternion.identity);
        yield return new WaitForSeconds(delaySpawn);
        enableSpawn = true;
    }

    public void ButtonReady()
    {
        if (!ready)
        {
            ready = true;
            buttonReady.SetActive(false);
            GameObject readyO = PhotonNetwork.Instantiate(Path.Combine("Prefabs", "Minigame", "Descend", "ReadyObject"), gameObject.transform.position, Quaternion.identity) as GameObject;
        }
    }

    private IEnumerator ReadyToGame()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        readyText.GetComponent<Text>().text = "Go!";
        yield return new WaitForSeconds(0.5f);
        flash.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        flash.SetActive(false);
        readyText.gameObject.SetActive(false);
        gameObject.GetComponent<AudioSource>().Play();
        enableMoveCam = true;
    }

    public IEnumerator ToHome()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(2);
        PhotonNetwork.LeaveRoom();
    }
}
