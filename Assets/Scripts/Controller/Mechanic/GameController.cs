using AudioConfig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public bool loadComplete = false, connection = false;
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
        panel.loadingPanel.GetComponentInChildren<Slider>().value += 1;
        yield return new WaitForSeconds(0.1f);

        viewObj.player2d = GameObject.FindGameObjectWithTag("Player");
        viewObj.currentActiveCamera = Camera.main.gameObject;
        panel.loadingPanel.GetComponentInChildren<Slider>().value += 1;
        yield return new WaitForSeconds(0.1f);

        music = GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>();
        BgmManager.SetBgmVolumeToObject(music.gameObject);
        panel.loadingPanel.GetComponentInChildren<Slider>().value += 1;
        yield return new WaitForSeconds(0.1f);

        eve = model.GetComponent<Even>();
        panel.loadingPanel.GetComponentInChildren<Slider>().value += 1;
        yield return new WaitForSeconds(0.1f);

        evc = gameObject.GetComponent<EventController>();
        evc.LoadCotTruyen();
        panel.loadingPanel.GetComponentInChildren<Slider>().value += 1;
        yield return new WaitForSeconds(0.1f);

        StartCoroutine(CheckConnection("google.com"));
        yield return new WaitForSeconds(0.1f);

        StartCoroutine(EndLoading());
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
        panel.loadingPanel.transform.Find("TextLoadingHud").transform.Find("LoadingText").gameObject.GetComponent<Text>().text = "Loading";
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

    private IEnumerator CheckConnection(string url)//Kiem tra ket noi
    {
        WWW www = new WWW(url);
        float elapsedTime = 0.0f;
        while (!www.isDone)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= 10.0f && www.progress <= 0.5)
            {
                connection = false;
                break;
            }
            yield return null;
        }
        if (!www.isDone || !string.IsNullOrEmpty(www.error))
        {
            connection = false;
            yield break;
        }
        connection = true;
    }

    #region event
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

    public void ChangeScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex, LoadSceneMode.Single);
    }

    public IEnumerator LoadingAndChangeScene(int sceneIndex)
    {
        StartCoroutine(StartLoading());
        yield return new WaitForSeconds(1f);
        ChangeScene(sceneIndex);
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

    public void UnlockCursor()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
#endif
    }

    public void LockCursor()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
#endif
    }

    public void HienThongBao(string thongBaoText)
    {
        panel.eventPanel.SetActive(true);
        eve.hoiThoaiPanel.SetActive(false);
        eve.warnMessPanel.SetActive(true);
        eve.messText.text = thongBaoText;
    }

    public void ActiveStory1obj(GameObject obj)
    {
        if (eve.story == 1)
            obj.SetActive(true);
    }

    #endregion

    public void BagAction()
    { 
        if (viewObj.player2d.GetComponent<PlayerController>().dieuKhien && !viewObj.player2d.GetComponent<CharacterObject>().holdWeapon && viewObj.player2d.GetComponent<PlayerController>().canOpenBag)
        {
            PlayASound(Resources.Load<AudioClip>("Audio/SoundEffect/UISound/click4"));
            if (panel.bagPanel.GetComponent<RectTransform>().offsetMax.y != 0)//Hien bag
            {
                panel.bagPanel.GetComponent<RectTransform>().SetTop(0);
                panel.bagPanel.GetComponent<RectTransform>().SetBottom(0);
                viewObj.player2d.GetComponent<CharacterObject>().attackable = false;
#if UNITY_STANDALONE || UNITY_EDITOR
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
#endif
                if (viewObj.fpsPlayer.activeInHierarchy)
                {
                    viewObj.fpsPlayer.GetComponent<FirstPersonController>().canRotate = false;
                }
            }
            else//an Bag di
            {
                panel.bagPanel.GetComponent<RectTransform>().SetTop(-769);
                panel.bagPanel.GetComponent<RectTransform>().SetBottom(769);
                viewObj.player2d.GetComponent<CharacterObject>().attackable = true;
#if UNITY_STANDALONE || UNITY_EDITOR
                if (viewObj.fpsPlayer.activeInHierarchy)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
#endif
                if (viewObj.fpsPlayer.activeInHierarchy)
                {
                    viewObj.fpsPlayer.GetComponent<FirstPersonController>().canRotate = true;
                }
            }
            viewObj.player2d.GetComponent<PlayerController>().bag.DestroyAllItemSlot();
            viewObj.player2d.GetComponent<PlayerController>().bag.LoadItemIntoSlot();
        }    
    }

    public void ChangeCameraToTalkObject(GameObject obj)
    {
        GameObject cam = Camera.main.gameObject;
        cam.GetComponent<Camera>().cullingMask |= 1 << LayerMask.NameToLayer("Not2D");
        cam.GetComponent<CameraFollowPlayer>().smoothSpeed = 0.1f;
        cam.GetComponent<CameraFollowPlayer>().target = obj.transform.Find("PointLightFace").transform;
        int facePosition = viewObj.player2d.GetComponent<CharacterObject>().faceRight;
        if (facePosition == obj.GetComponent<CharacterObject>().faceRight)//neu 2 doi tuong quay cung mot phia
            obj.GetComponent<CharacterObject>().Flip();//cho talk object quay ve phia player
        cam.GetComponent<CameraFollowPlayer>().offset = new Vector3(-0.8f * facePosition, 0, 0);
        eve.talkCharacter = new GameObject[1];
        eve.talkCharacter[0] = obj;

        viewObj.player2d.SetActive(false);
    }

    public void ChangeCameraMeetFriend(GameObject obj)
    {
        StartCoroutine(ChangingFriendCam(obj));
    }

    IEnumerator ChangingFriendCam(GameObject obj)
    {
        yield return new WaitForSeconds(1f);
        GameObject cam = Camera.main.gameObject;
        cam.GetComponent<Camera>().cullingMask |= 1 << LayerMask.NameToLayer("Not2D");
        cam.GetComponent<CameraFollowPlayer>().smoothSpeed = 0.2f;
        cam.GetComponent<CameraFollowPlayer>().target = obj.transform.Find("PointLightFace").transform;
        int facePosition = obj.GetComponent<CharacterObject>().faceRight;
        cam.GetComponent<CameraFollowPlayer>().offset = new Vector3(0.8f * facePosition, 0, 0);
        eve.talkCharacter = new GameObject[1];
        eve.talkCharacter[0] = obj;
        viewObj.player2d.SetActive(false);
        evc.PlayStory();
    }

    public void ChangeCameraToPlayer2d()
    {
        viewObj.player2d.SetActive(true);
        GameObject cam = Camera.main.gameObject;
        cam.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("Not2D"));
        cam.GetComponent<CameraFollowPlayer>().target = viewObj.player2d.transform.Find("PointLightFace").transform;
        cam.GetComponent<CameraFollowPlayer>().offset = new Vector3(0, 0.2f, -10f);
        cam.GetComponent<CameraFollowPlayer>().smoothSpeed = 0.5f;
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
    #endregion

    private void OnApplicationQuit()
    {
        if (PlayerPrefs.GetInt("TryAgain") == 1)
            PlayerPrefs.SetInt("TryAgain", 0);
    }
}
