using AudioConfig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public bool loadComplete = false;
    private GameObject model;
    [HideInInspector]
    public ViewObject viewObj;
    [HideInInspector]
    public TouchButtonManager touchButton;
    [HideInInspector]
    public AudioSource music;
    [HideInInspector]
    public Panel panel;
    [HideInInspector]
    public Even eve;
    [HideInInspector]
    public EventController evc;

    void Start()
    {
        StartCoroutine(LoadObject());
    }

    IEnumerator LoadObject()
    {
        model = GameObject.FindGameObjectWithTag("Model");
        panel = model.GetComponent<Panel>();
        panel.loadingPanel.SetActive(true);
        panel.loadingPanel.GetComponentInChildren<Slider>().value = 0;

        //load something
        yield return new WaitForSeconds(0.5f);

        touchButton = model.GetComponent<TouchButtonManager>();
        panel.loadingPanel.GetComponentInChildren<Slider>().value += 1;
        yield return new WaitForSeconds(0.1f);

        viewObj = model.GetComponent<ViewObject>();
        panel.loadingPanel.GetComponentInChildren<Slider>().value += 1;
        yield return new WaitForSeconds(0.1f);

        LoadPlayerPrefab();
        panel.bagPanel.SetActive(true);
        panel.loadingPanel.GetComponentInChildren<Slider>().value += 1;
        yield return new WaitForSeconds(0.1f);

        viewObj.player2d = GameObject.FindGameObjectWithTag("Player");
        viewObj.currentActiveCamera = Camera.main.gameObject;
        panel.loadingPanel.GetComponentInChildren<Slider>().value += 1;
        yield return new WaitForSeconds(0.1f);

        CapNhatColliderIgnore();
        panel.loadingPanel.GetComponentInChildren<Slider>().value += 1;
        yield return new WaitForSeconds(0.1f);

        music = GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>();
        BgmManager.SetBgmVolumeToObject(music.gameObject);
        panel.bagPanel.SetActive(false);
        panel.loadingPanel.GetComponentInChildren<Slider>().value += 1;
        yield return new WaitForSeconds(0.1f);

        eve = model.GetComponent<Even>();
        panel.loadingPanel.GetComponentInChildren<Slider>().value += 1;
        yield return new WaitForSeconds(0.1f);

        evc = gameObject.GetComponent<EventController>();
        evc.LoadCotTruyen();
        panel.loadingPanel.GetComponentInChildren<Slider>().value += 1;
        yield return new WaitForSeconds(0.1f);

        StartCoroutine(EndLoading());
        if (panel.readyPanel != null)
            panel.readyPanel.SetActive(true);
        yield return new WaitForSeconds(1f);
        gameObject.GetComponent<InputController>().enabled = true;
        loadComplete = true;
        yield return new WaitForSeconds(2f);
    }

    public void LoadingSimulate()
    {
        StartCoroutine(SimulateLoading());
    }

    public IEnumerator StartLoading()
    {
        panel.loadingPanel.SetActive(true);
        panel.loadingPanel.GetComponent<Animation>().Play("LoadingStart");
        panel.loadingPanel.GetComponentInChildren<Slider>().value = 0;
        yield return new WaitForSeconds(1f);       
    }

    public IEnumerator EndLoading()
    {
        panel.loadingPanel.transform.Find("TextLoadingHud").transform.Find("LoadingText").gameObject.GetComponent<Text>().text = "Complete";
        panel.loadingPanel.GetComponent<Animation>().Play("LoadingComplete");
        yield return new WaitForSeconds(0.2f);
        CheckStoryAutoPlay();
        if (PlayerPrefs.GetInt("TryAgain") == 1 && PlayerPrefs.GetInt("Checkpoint" + SceneManager.GetActiveScene().buildIndex) == 1)
        {
            viewObj.player2d.transform.position = new Vector2(PlayerPrefs.GetFloat("CheckpointXPos"), PlayerPrefs.GetFloat("CheckpointYPos"));
            viewObj.currentActiveCamera.transform.position = new Vector2(PlayerPrefs.GetFloat("CheckpointXPos"), PlayerPrefs.GetFloat("CheckpointYPos") + 2.2f);
            PlayerPrefs.SetInt("TryAgain", 0);
            panel.loadingPanel.GetComponentInChildren<Slider>().value += 1;
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.8f);
        music.Play();
        panel.loadingPanel.SetActive(false);
    }

    IEnumerator SimulateLoading()
    {
        panel.loadingPanel.transform.Find("TextLoadingHud").transform.Find("LoadingText").gameObject.GetComponent<Text>().text = "Loading";
        StartCoroutine(StartLoading());
        yield return new WaitForSeconds(1f);
        panel.loadingPanel.transform.Find("TextLoadingHud").transform.Find("LoadingText").gameObject.GetComponent<Text>().text = "Complete";
        panel.loadingPanel.GetComponent<Animation>().Play("LoadingComplete");
        yield return new WaitForSeconds(1f);
        panel.loadingPanel.SetActive(false);
    }

    private void CheckStoryAutoPlay()
    {
        if (PlayerPrefs.GetInt("TryAgain") == 1 && PlayerPrefs.GetInt("Checkpoint" + SceneManager.GetActiveScene().buildIndex) == 1)
            return;
        else
            switch (eve.story)
            {
                case 0:
                    evc.PlayStory();
                    break;
                default:

                    break;
            }
    }

    private void LoadPlayerPrefab()
    {

    }

    public void CapNhatColliderIgnore()
    {
        GameObject[] allEnemy;
        GameObject[] allBlockEnemy;
        allEnemy = GameObject.FindGameObjectsWithTag("Enemy");
        allBlockEnemy = GameObject.FindGameObjectsWithTag("BlockEnemy");
        GameObject boss = GameObject.FindGameObjectWithTag("Boss");
        GameObject[] allAlly = GameObject.FindGameObjectsWithTag("Ally");
        if (boss != null)
            Physics2D.IgnoreCollision(viewObj.player2d.transform.Find("Collider").GetComponent<Collider2D>(), boss.transform.Find("Collider").GetComponent<Collider2D>());
        for (int i = 0; i < allEnemy.Length; i++)
            Physics2D.IgnoreCollision(viewObj.player2d.transform.Find("Collider").GetComponent<Collider2D>(), allEnemy[i].transform.Find("Collider").GetComponent<Collider2D>());
        for (int i = 0; i < allBlockEnemy.Length; i++)
            Physics2D.IgnoreCollision(viewObj.player2d.transform.Find("Collider").GetComponent<Collider2D>(), allBlockEnemy[i].GetComponent<Collider2D>());
        for (int i = 0; i < allAlly.Length; i++)
            Physics2D.IgnoreCollision(viewObj.player2d.transform.Find("Collider").GetComponent<Collider2D>(), allAlly[i].transform.Find("Collider").GetComponent<Collider2D>());
    }

    public void ChangeScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex, LoadSceneMode.Single);
    }

    public void ActiveDeactivePanel(GameObject panel)
    {
        panel.SetActive(!panel.activeInHierarchy);
    }

    public void CheckPoint()
    {
        PlayerPrefs.SetInt("Checkpoint" + SceneManager.GetActiveScene().buildIndex, 1);
        PlayerPrefs.SetFloat("CheckpointXPos", viewObj.player2d.transform.position.x);
        PlayerPrefs.SetFloat("CheckpointYPos", viewObj.player2d.transform.position.y);
    }

    public void SetStory(int index)
    {
        PlayerPrefs.SetInt("story", index);
        eve.story = index;
    }

    public void SetTextNum(int num)
    {
        eve.textNum = num;
    }

    public void DeleteCheckPoint()
    {
        PlayerPrefs.SetInt("Checkpoint" + SceneManager.GetActiveScene().buildIndex, 0);
    }

    #region screenEvent
    public IEnumerator FadeOutScreenWhite()//Lam man hinh trang dan
    {
        panel.eventPanel.SetActive(true);
        eve.manHinh.SetActive(true);
        eve.manHinh.GetComponent<Animation>().Play("FadeOutWhite");
        yield return new WaitForSeconds(1f);
    }

    public IEnumerator FadeInScreenWhite()//lam man hinh giam do trang xuong
    {
        panel.eventPanel.SetActive(true);
        eve.manHinh.SetActive(true);
        eve.manHinh.GetComponent<Animation>().Play("FadeInWhite");
        yield return new WaitForSeconds(1f);
        eve.manHinh.SetActive(false);
    }

    public IEnumerator FadeOutScreenBlack()//Lam man hinh toi dan
    {
        panel.eventPanel.SetActive(true);
        eve.manHinh.SetActive(true);
        eve.manHinh.GetComponent<Animation>().Play("FadeOutBlack");
        yield return new WaitForSeconds(1f);
    }

    public IEnumerator FadeInScreenBlack()//lam man hinh sang len
    {
        panel.eventPanel.SetActive(true);
        eve.manHinh.SetActive(true);
        eve.manHinh.GetComponent<Animation>().Play("FadeInBlack");
        yield return new WaitForSeconds(1f);
        eve.manHinh.SetActive(false);
    }
    #endregion

    #region Sound
    public void ThayDoiNhacNen(string tenNhac)
    {
        if (tenNhac == "")
        {
            music.Stop();
            return;
        }
        AudioClip nhac = Resources.Load<AudioClip>("Audio/Music/" + tenNhac);
        BgmManager.PlayBgm(music.gameObject, nhac);
    }

    public void ThayDoiNhacNen(AudioClip nhacNen)
    {
        BgmManager.PlayBgm(music.gameObject, nhacNen);
    }

    public void PlayASound(AudioClip sound)
    {
        GameObject soundObj = Instantiate(Resources.Load<GameObject>("Prefabs/EmptySoundObject"), viewObj.player2d.transform.position, Quaternion.identity);
        SoundManager.PlaySound(soundObj, sound);
        Destroy(soundObj, 1.5f);
    }

    //public void ThayDoiSFX(string sfxName)
    //{
    //    if (sfxName == "")
    //    {
    //        sfx.Stop();
    //        return;
    //    }
    //    sfx.clip = Resources.Load<AudioClip>("Audio/SFX/" + sfxName);
    //    sfx.Play();
    //}

    //public void ChaySE(string seName)
    //{
    //    se.clip = Resources.Load<AudioClip>("Audio/SE/" + seName);
    //    se.Play();
    //}
    #endregion

    private void OnApplicationQuit()
    {
        if (PlayerPrefs.GetInt("TryAgain") == 1)
            PlayerPrefs.SetInt("TryAgain", 0);
    }
}
