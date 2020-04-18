using AudioConfig;
using FIMSpace.FLook;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EventController : MonoBehaviour
{
    [SerializeField]
    private GameController gc;
    public bool someOneTalk = false;

    private void Start()
    {
        gc = gameObject.GetComponent<GameController>();
    }

    #region luu, load cot truyen
    public void LuuCotTruyen()
    {
        PlayerPrefs.SetInt("story", gc.eve.story);
    }

    public void LoadCotTruyen()
    {
        gc.eve.story = PlayerPrefs.GetInt("story");
    }
    #endregion

    #region buttonLuaChon
    public void LuaChon1()
    {
        gc.eve.textNum++;
        PlayStory();
    }
    public void LuaChon2()
    {
        gc.eve.textNum += gc.eve.choiceNumAdd;
        PlayStory();
    }
    #endregion

    private void Update()
    {
        if (gc.loadComplete)
            if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetButtonDown("Submit"))
            {
                if (gc.eve.enableNext)
                {
                    gc.eve.textNum++;
                    PlayStory();
                }
            }
    }

    public void CompleteTalk()//Hoan thanh 1 doan hoi thoai voi npc
    {
        gc.eve.enableNext = false;
        gc.panel.eventPanel.SetActive(false);
        gc.eve.hoiThoaiPanel.SetActive(false);
        gc.panel.touch2dPanel.SetActive(true);
        gc.eve.textNum = 0;
        gc.viewObj.player2d.GetComponent<CharacterObject>().EnableCharacter();
    }

    public void CompleteEvent()//Hoan thanh 1 su kien nho hon story
    {
        gc.eve.enableNext = false;
        gc.panel.eventPanel.SetActive(false);
        gc.eve.hoiThoaiPanel.SetActive(false);
        gc.panel.touch2dPanel.SetActive(true);
        gc.eve.textNum++;
        gc.viewObj.player2d.GetComponent<CharacterObject>().EnableCharacter();
    }

    public void CompleteStory()//Hoan thanh 1 story
    {
        gc.eve.enableNext = false;
        gc.eve.textNum = 0;
        gc.eve.story++;
        LuuCotTruyen();
        gc.panel.touch2dPanel.SetActive(true);
        gc.panel.eventPanel.SetActive(false);
        gc.eve.hoiThoaiPanel.SetActive(false);
        gc.viewObj.player2d.GetComponent<CharacterObject>().EnableCharacter();
    }

    public IEnumerator ChuyenCanh(string nhacNen, GameObject cameraToActive, bool activeMenu, bool whiteScreen)//Chuyen khung hinh
    {
        gc.eve.enableNext = false;
        yield return new WaitForSeconds(0.5f);
        if (whiteScreen)
            StartCoroutine(gc.FadeOutScreenWhite());
        else
            StartCoroutine(gc.FadeOutScreenBlack());
        yield return new WaitForSeconds(1f);
        gc.eve.hoiThoaiPanel.SetActive(false);
        if (cameraToActive != null)
        {
            gc.viewObj.currentActiveCamera.SetActive(false);
            cameraToActive.SetActive(true);
            gc.viewObj.currentActiveCamera = cameraToActive;
        }
        yield return new WaitForSeconds(1f);
        if (nhacNen != "")
        {
            gc.ThayDoiNhacNen(nhacNen);
        }
        if (whiteScreen)
            StartCoroutine(gc.FadeInScreenWhite());
        else
            StartCoroutine(gc.FadeInScreenBlack());
        yield return new WaitForSeconds(1f);
        gc.eve.hoiThoaiPanel.SetActive(true);
        gc.eve.textNum++;
        gc.eve.enableNext = true;
        PlayStory();
    }

    private void HoiThoai(string ten, string hoiThoai)
    {
        gc.eve.ten.text = ten;
        gc.eve.talk.text = hoiThoai;
        gc.PlayASound(Resources.Load<AudioClip>("Audio/SoundEffect/UISound/TalkSound"));
    }

    #region event
    public void CheckStoryThenPlay(int storyToCheck)
    {
        if (gc.eve.story == storyToCheck)
        {
            PlayStory();
        }
    }

    public void PlayStory()
    {
        gc.viewObj.player2d.GetComponent<CharacterObject>().DisableCharacter();
        gc.eve.enableNext = true;
        gc.panel.eventPanel.SetActive(true);
        //Kiem tra xem dang la hoi thoai voi npc hay la cot truyen chinh
        if (someOneTalk)
        {

        }
        else
        {
            switch (gc.eve.story)
            {
                case 0:
                    StartCoroutine(Intro());
                    break;
                case 1:

                    break;
                case 2:

                    break;
            }
        }
    }

    IEnumerator Intro()
    {
        yield return new WaitForSeconds(0);
        switch (gc.eve.textNum)
        {
            case 0:
                gc.touchButton.bagButton.SetActive(false);
                gc.touchButton.buttonJump.SetActive(false);
                gc.touchButton.buttonRun.SetActive(false);
                gc.touchButton.buttonSwitchWeapon.SetActive(false);
                gc.eve.enableNext = false;
                gc.eve.manHinh.SetActive(true);
                gc.eve.hoiThoaiPanel.SetActive(false);
                gc.eve.manHinh.GetComponent<Image>().color = Color.black;
                yield return new WaitForSeconds(1f);
                gc.eve.luaChonPanel.SetActive(false);
                gc.eve.hoiThoaiPanel.SetActive(true);
                HoiThoai("???",
                    "[Hey!]");
                gc.eve.enableNext = true;
                break;
            case 1:
                HoiThoai("???",
                    "[Hey! Wake up!]");
                break;
            case 2:
                gc.viewObj.player2d.GetComponent<CharacterObject>().weaponAnimId = -2;
                gc.eve.enableNext = false;
                StartCoroutine(gc.FadeInScreenBlack());
                yield return new WaitForSeconds(5f);
                gc.viewObj.player2d.GetComponent<CharacterObject>().weaponAnimId = 0;
                gc.eve.enableNext = true;
                break;
            case 3:
                HoiThoai("???",
                    "[Over here!]");
                break;
            case 4:
                CompleteEvent();
                break;
            case 5:
                gc.eve.hoiThoaiPanel.SetActive(true);
                gc.eve.enableNext = false;
                gc.eve.talkCharacter[0].GetComponent<FLookAnimator>().ObjectToFollow = Camera.main.transform;
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(3, 50);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().anim.SetLayerWeight(5, 1);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().anim.SetInteger("EmotionId", 4);
                HoiThoai("???",
                     "[Do you remember what happened?]");
                gc.eve.luaChon1.text = "I don't";
                gc.eve.luaChon2.text = "I remember";
                gc.eve.choiceNumAdd = 5;
                gc.eve.luaChonPanel.SetActive(true);
                break;
            case 6:
                gc.eve.luaChonPanel.SetActive(false);
                HoiThoai("???",
                     "[I don't remember anything!]");
                break;
            case 7:
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(3, 0);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().anim.SetInteger("EmotionId", 0);
                HoiThoai("???",
                     "[Ok! I will briefly explain the current situation]");
                break;
            case 8:
                HoiThoai("???",
                    "[You are failed test subjects and will be executed by those bastards outside!]");
                break;
            case 9:
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(25, 50);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().anim.SetInteger("EmotionId", 2);
                HoiThoai("Lily",
                   "[My name is Lily, I'm here to save you, so please trust me!]");
                gc.eve.textNum += 2;
                break;
            case 10:
                gc.eve.luaChonPanel.SetActive(false);
                HoiThoai("???",
                   "[I remember! You take down this soldier to save me]");
                break;
            case 11:
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(3, 0);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().anim.SetInteger("EmotionId", 0);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(25, 50);
                HoiThoai("Lily",
                     "[That good! I'm Lily, nice to meet you!]");
                break;
            case 12:
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(25, 0);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().anim.SetInteger("EmotionId", 0);
                HoiThoai("Lily",
                   "[We don't have much time so let's get straight to the point!]");
                break;
            case 13:
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(0, 50);
                HoiThoai("Lily",
                   "[There is still another test subject here that needs to be rescued!]");
                break;
            case 14:
                HoiThoai("Lily",
                   "[But I've reached my limit!]");
                break;
            case 15:
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(0, 0);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(3, 50);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().anim.SetInteger("EmotionId", 4);
                HoiThoai("Lily",
                   "[Please help me save her!]");
                break;
            case 16:
                HoiThoai("???",
                   "[Okay! I will help you, but I don't know anything]");
                break;
            case 17:
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(3, 0);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(25, 50);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().anim.SetInteger("EmotionId", 0);
                HoiThoai("Lily",
                   "[Here! Take these thing! I will talk to you via this mobile headset!]");
                break;
            case 18:
                HoiThoai("Lily",
                   "[This laser sword will help you defend yourself!]");
                break;
            case 19:
                HoiThoai("Lily",
                   "[And the Teleport Device! You can teleport to our base when this mission is completed!]");
                break;
            case 20:
                HoiThoai("???",
                   "[Thank you!]");
                break;
            case 21:
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(25, 0);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(3, 50);
                HoiThoai("Lily",
                   "[I was the one who had to say that! Anyway, good luck!]");               
                break;
            case 22:
                gc.touchButton.bagButton.SetActive(true);
                gc.touchButton.buttonJump.SetActive(true);
                gc.ChangeCameraToPlayer2d();
                CompleteEvent();
                GameObject lily = gc.eve.talkCharacter[0];
                lily.GetComponent<FLookAnimator>().ObjectToFollow = gc.viewObj.player2d.transform.Find("PointLightFace");
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().anim.SetLayerWeight(5, 0);
                lily.GetComponent<CharacterObject>().weaponAnimId = -1;
                yield return new WaitForSeconds(0.3f);
                GameObject teleportEffect = Instantiate(Resources.Load<GameObject>("Prefabs/Effect/TeleportEffect"), new Vector3(lily.transform.position.x, lily.transform.position.y + 1, lily.transform.position.z - 0.5f), Quaternion.identity);
                SoundManager.PlaySound(teleportEffect, Resources.Load<AudioClip>("Audio/SoundEffect/ObjectSound/cardSwipe"));
                yield return new WaitForSeconds(1f);
                Destroy(teleportEffect);
                lily.SetActive(false);
                break;
            case 23:
                gc.eve.hoiThoaiPanel.SetActive(true);
                HoiThoai("Lily",
                   "[The door is locked]");
                break;
            case 24:
                HoiThoai("Lily",
                   "[You need to find a keycard to unlock it]");
                break;
            case 25:
                HoiThoai("Lily",
                   "[Look in this room]");
                break;
            case 26:
                CompleteEvent();
                break;
            case 27:
                gc.eve.hoiThoaiPanel.SetActive(true);
                HoiThoai("Lily",
                   "[See that soldier ahead? Use your sword to take him down!]");
                break;
            case 28:
                gc.eve.hoiThoaiPanel.SetActive(true);
                HoiThoai("Lily",
                   "[Approach the enemy slowly and attack them from behind!]");
                break;
            case 29:
                CompleteEvent();
                break;
            case 30:
                gc.eve.luaChonPanel.SetActive(false);
                gc.eve.hoiThoaiPanel.SetActive(true);
                HoiThoai("Lily",
                   "[There is one thing I want you to take]");
                break;
            case 31:
                HoiThoai("???",
                   "[What thing?]");
                break;
            case 32:
                HoiThoai("Lily",
                   "[It's called Star Core]");
                break;
            case 33:
                HoiThoai("???",
                   "[Star Core?]");
                break;
            case 34:
                HoiThoai("Lily",
                   "[It's in this room, a glittering object, you will see soon!]");
                break;
            case 35:
                HoiThoai("???",
                  "[Ok, I will take this for you.]");
                break;
            case 36:
                HoiThoai("Lily",
                   "[Thank you!]");
                break;
            case 37:
                HoiThoai("Lily",
                   "[And be careful, when you steal it, the alarm will be on!]");
                break;
            case 38:
                CompleteEvent();
                break;
            case 39:
                gc.viewObj.player2d.GetComponent<CharacterObject>().weaponAnimId = 0;
                gc.eve.luaChonPanel.SetActive(false);
                gc.eve.hoiThoaiPanel.SetActive(true);
                HoiThoai("???",
                   "[All of them are dead! What happening?]");
                break;
            case 40:
                HoiThoai("Lily",
                   "[Something not right!]");
                break;
            case 41:
                HoiThoai("???",
                   "[Where is the Test Subject?]");
                break;
            case 42:
                HoiThoai("Lily",
                   "[I don't know! But it's too dangerous here, teleport to our base now!]");
                break;
            case 43:
                CompleteStory();
                gc.viewObj.player2d.GetComponent<CharacterObject>().weaponAnimId = -1;
                yield return new WaitForSeconds(0.3f);
                GameObject teleportEffect2 = Instantiate(Resources.Load<GameObject>("Prefabs/Effect/TeleportEffect"), new Vector3(gc.viewObj.player2d.transform.position.x, gc.viewObj.player2d.transform.position.y + 1, gc.viewObj.player2d.transform.position.z - 0.5f), Quaternion.identity);
                gc.panel.missionCompletePanel.SetActive(true);
                gc.PlayASound(Resources.Load<AudioClip>("Audio/SoundEffect/UISound/MissionComplete"));
                yield return new WaitForSeconds(1f);
                Destroy(teleportEffect2);
                gc.viewObj.player2d.SetActive(false);
                yield return new WaitForSeconds(1f);
                StartCoroutine(gc.StartLoading());
                yield return new WaitForSeconds(1f);
                SceneManager.LoadScene(2, LoadSceneMode.Single);
                break;
        }
    }

    #endregion
}
