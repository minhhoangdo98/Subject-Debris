using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DodgeMinigame : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private int soLuongPlatform;
    public GameObject respawnPoint, readyText, flash, buttonReady;
    public GameObject[] platform, player, readyObj;
    public bool canSpawn = false, enableSpawn = true, ready = false, enableIncreseSpeed = true;
    public float platformHeight = 1, maxHeight = 5, spawnDelay = 5, delayTime = 10, spawnDelayDecrease = 0.5f, playerSpeedIncrease = 50;
    public int point = 0;
    public Text pointNum;
    public float minRange = -8f, maxRange = 8f;

    private void Start()
    {
        soLuongPlatform = platform.Length;
    }

    private void Update()
    {
        if (canSpawn && enableSpawn)
        {
            if (enableIncreseSpeed)
            {
                StartCoroutine(TangTocDo());
            }

            if (photonView.IsMine)
            {
                StartCoroutine(SpawnPlatform());
            }
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
        player = GameObject.FindGameObjectsWithTag("Player");
        if (spawnDelay <= 0.2f)
        {
            spawnDelay = 0.2f;            
        }
        else
        {
            spawnDelay -= spawnDelayDecrease;
            foreach (GameObject pl in player)
                pl.GetComponent<PlayerDescend>().speed += playerSpeedIncrease;
        }
        gameObject.GetComponent<AudioSource>().pitch += 0.05f;
        if (gameObject.GetComponent<AudioSource>().pitch > 2)
            gameObject.GetComponent<AudioSource>().pitch = 2;
        yield return new WaitForSeconds(delayTime);
        enableIncreseSpeed = true;
    }

    IEnumerator SpawnPlatform()
    {
        enableSpawn = false;
        platformHeight = Random.Range(0.5f, maxHeight);
        int r = Random.Range(0, soLuongPlatform);
        Vector2 pos = new Vector2(Random.Range(minRange, maxRange), respawnPoint.transform.position.y);
        GameObject plat = PhotonNetwork.Instantiate(Path.Combine("Prefabs/Minigame/Dodge/", platform[r].name), pos, Quaternion.identity);
        plat.GetComponent<Rigidbody2D>().mass = platformHeight;
        if (spawnDelay <= 0.2f)
        {
            spawnDelay = 0.2f;
        }
        yield return new WaitForSeconds(spawnDelay);
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
        readyText.GetComponent<Text>().text = "Go";
        yield return new WaitForSeconds(0.5f);
        flash.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        flash.SetActive(false);
        readyText.gameObject.SetActive(false);
        gameObject.GetComponent<AudioSource>().Play();
        canSpawn = true;
    }

    public void GameoverChecker()
    {
        StartCoroutine(CheckGameover());
    }

    IEnumerator CheckGameover()
    {
        yield return new WaitForSeconds(1.2f);
        GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
        if (canSpawn && player.Length <= 1)
        {
            canSpawn = false;
            GetComponent<AudioSource>().Stop();
            readyText.GetComponent<Text>().text = "Match end";
            readyText.SetActive(true);
            Debug.Log("Leave scene");
            StartCoroutine(ToHome());
        }
    }

    public IEnumerator ToHome()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(2);
        PhotonNetwork.LeaveRoom();
    }
}
