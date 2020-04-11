using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DescendMinigame : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private int soLuongPlatform, cNVLienTiep, lienTiepToiDa = 2;
    public GameObject cam, respawnPoint, diaHinh, readyText, flash, buttonReady;
    public GameObject[] platform, player, readyObj;
    public bool enableSpawn = true, enableIncreseSpeed = true, enableMoveCam = false, ready = false;
    public float camSpeedStart = 0.5f, camSpeedIncrease = 0.5f, playerSpeedIncrease = 50, delayTime = 10, delaySpawn = 2.5f;
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
            cam.transform.Translate(new Vector3(0, -1 * Time.deltaTime * camSpeedStart));
            if (enableIncreseSpeed)
            {
                StartCoroutine(TangTocDo());
            }

            if (enableSpawn && photonView.IsMine)
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
        camSpeedStart += camSpeedIncrease;
        player = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject pl in player)
            pl.GetComponent<PlayerDescend>().speed += playerSpeedIncrease;
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
        //Kiem tra chuong ngai vat lien tiep neu vuot qua so lan cho phep thi random lai cho den khi khong phai chuong ngai vat va reset lai so lan lien tiep
        if (platform[r].CompareTag("ChuongNgaiVat"))
        {
            if (cNVLienTiep < lienTiepToiDa)
                cNVLienTiep++;
            else
            {
                cNVLienTiep = 0;
                do
                {
                    r = Random.Range(0, soLuongPlatform);
                } while (platform[r].tag == "ChuongNgaiVat");
            }
        }
        else
            cNVLienTiep = 0;//Neu khong phai chuong ngai vat thi reset lai so lan lien tiep
        Vector2 pos = new Vector2(Random.Range(minRange, maxRange), respawnPoint.transform.position.y);
        GameObject plat = PhotonNetwork.Instantiate(Path.Combine("Prefabs/Minigame/Descend/", platform[r].name), pos, Quaternion.identity);
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
