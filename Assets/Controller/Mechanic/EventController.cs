using AudioConfig;
using FIMSpace.FLook;
using Relationship;
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

    public void CompleteEvent(bool activeTouchPanel)//Hoan thanh 1 su kien nho hon story
    {
        gc.eve.enableNext = false;
        gc.panel.eventPanel.SetActive(false);
        gc.eve.hoiThoaiPanel.SetActive(false);
        gc.panel.touch2dPanel.SetActive(activeTouchPanel);
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

    private void HoiThoai(string ten, string faceName, string hoiThoai)
    {
        if (faceName != "")
        {
            gc.eve.facePanel.SetActive(true);
            if (ten != "???")
                gc.eve.faceImage.sprite = Resources.Load<Sprite>("Content/Faces/" + ten + "/" + faceName);
            else
                gc.eve.faceImage.sprite = Resources.Load<Sprite>("Content/Faces/" + faceName + "/" + faceName);
        }
        else
            gc.eve.facePanel.SetActive(false);
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
                    StartCoroutine(Story1());
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
                HoiThoai("???", "",
                    "Can you hear me?");
                gc.eve.enableNext = true;
                break;
            case 1:
                HoiThoai("???", "",
                    "Please wake up!");
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
                HoiThoai("???", "Lily",
                    "Over here!");
                break;
            case 4:
                CompleteEvent(true);
                break;
            case 5:
                gc.eve.hoiThoaiPanel.SetActive(true);
                gc.eve.enableNext = false;
                gc.eve.talkCharacter[0].GetComponent<FLookAnimator>().ObjectToFollow = Camera.main.transform;
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(3, 50);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().anim.SetLayerWeight(5, 1);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().anim.SetInteger("EmotionId", 4);
                HoiThoai("???", "Lily",
                     "Do you remember what happened?");
                gc.eve.luaChon1.text = "I don't";
                gc.eve.luaChon2.text = "I remember";
                gc.eve.choiceNumAdd = 5;
                gc.eve.luaChonPanel.SetActive(true);
                break;
            case 6:
                gc.eve.luaChonPanel.SetActive(false);
                HoiThoai("???", "Reid",
                     "I don't remember anything!");
                break;
            case 7:
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(3, 0);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().anim.SetInteger("EmotionId", 0);
                HoiThoai("???", "Lily",
                     "Ok! I will briefly explain the current situation");
                break;
            case 8:
                HoiThoai("???", "Lily",
                    "You are failed test subjects and will be executed by those bastards outside!");
                break;
            case 9:
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(25, 50);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().anim.SetInteger("EmotionId", 2);
                HoiThoai("Lily", "LilySmile",
                   "My name is Lily, I'm here to save you, so please trust me!");
                gc.eve.textNum += 2;
                break;
            case 10:
                gc.eve.luaChonPanel.SetActive(false);
                HoiThoai("???", "Reid",
                   "I remember! You take down this soldier to save me");
                break;
            case 11:
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(3, 0);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().anim.SetInteger("EmotionId", 0);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(25, 50);
                HoiThoai("Lily", "LilySmile",
                     "That good! I'm Lily, nice to meet you!");
                break;
            case 12:
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(25, 0);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().anim.SetInteger("EmotionId", 0);
                HoiThoai("Lily", "Lily",
                   "We don't have much time so let's get straight to the point!");
                break;
            case 13:
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(0, 50);
                HoiThoai("Lily","Lily",
                   "There is still another test subject here that needs to be rescued!");
                break;
            case 14:
                HoiThoai("Lily","Lily",
                   "But I've reached my limit!");
                break;
            case 15:
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(0, 0);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(3, 50);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().anim.SetInteger("EmotionId", 4);
                HoiThoai("Lily", "Lily",
                   "Please help me save her before being killed!");
                break;
            case 16:
                HoiThoai("???", "Reid",
                   "Okay! I will help you, but I don't know anything");
                break;
            case 17:
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(3, 0);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(25, 50);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().anim.SetInteger("EmotionId", 0);
                HoiThoai("Lily", "LilySmile",
                   "Here! Take these thing! I will talk to you via this mobile headset!");
                break;
            case 18:
                HoiThoai("Lily", "LilySmile",
                   "This laser sword will help you defend yourself!");
                break;
            case 19:
                HoiThoai("Lily", "LilySmile",
                   "And the Teleport Device! You can teleport to our base when this mission is completed!");
                break;
            case 20:
                HoiThoai("???", "Reid",
                   "Thank you!");
                break;
            case 21:
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(25, 0);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(3, 50);
                HoiThoai("Lily", "Lily",
                   "I was the one who had to say that! Anyway, good luck!");             
                break;
            case 22:
                gc.touchButton.bagButton.SetActive(true);
                gc.touchButton.buttonJump.SetActive(true);
                gc.ChangeCameraToPlayer2d();
                CompleteEvent(true);
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
                HoiThoai("Lily", "Lily",
                   "The door is locked");
                break;
            case 24:
                HoiThoai("Lily", "Lily",
                   "You need to find a keycard to unlock it");
                break;
            case 25:
                HoiThoai("Lily", "Lily",
                   "Look in this room");
                break;
            case 26:
                CompleteEvent(true);
                break;
            case 27:
                gc.eve.hoiThoaiPanel.SetActive(true);
                HoiThoai("Lily", "Lily",
                   "See that soldier ahead? Use your sword to take him down!");
                break;
            case 28:
                gc.eve.hoiThoaiPanel.SetActive(true);
                HoiThoai("Lily", "Lily",
                   "Approach the enemy slowly and attack them from behind!");
                break;
            case 29:
                CompleteEvent(true);
                break;
            case 30:
                gc.eve.luaChonPanel.SetActive(false);
                gc.eve.hoiThoaiPanel.SetActive(true);
                HoiThoai("Lily", "Lily",
                   "Look like we may not be able to make it in time");
                break;
            case 31:
                HoiThoai("???", "Reid",
                   "What now?");
                break;
            case 32:
                HoiThoai("Lily", "Lily",
                   "We need something to distract them, maybe the Star Core in this room can do the trick");
                break;
            case 33:
                HoiThoai("???", "Reid",
                   "Star Core?");
                break;
            case 34:
                HoiThoai("Lily", "Lily",
                   "It's a glittering object, you will see soon! Take it and the alarm will be turned on!");
                break;
            case 35:
                HoiThoai("???", "Reid",
                  "Ok, I will take this to distract them.");
                break;
            case 36:
                HoiThoai("Lily", "Lily",
                   "Thank you!");
                break;
            case 37:
                HoiThoai("Lily", "Lily",
                   "And be careful with the security systems");
                break;
            case 38:
                CompleteEvent(true);
                break;
            case 39:
                gc.viewObj.player2d.GetComponent<CharacterObject>().weaponAnimId = 0;
                gc.eve.luaChonPanel.SetActive(false);
                gc.eve.hoiThoaiPanel.SetActive(true);
                HoiThoai("???", "Reid",
                   "All of them are dead! What happening?");
                break;
            case 40:
                HoiThoai("Lily", "LilySad",
                   "Something not right!");
                break;
            case 41:
                HoiThoai("???", "Reid",
                   "Where is the Test Subject?");
                break;
            case 42:
                HoiThoai("Lily", "Lily",
                   "I don't know! But it's too dangerous here, teleport to our base now!");
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

    IEnumerator Story1()
    {
        yield return new WaitForSeconds(0);
        switch (gc.eve.textNum)
        {
            case 0:
                gc.eve.luaChonPanel.SetActive(false);
                gc.eve.hoiThoaiPanel.SetActive(true);
                gc.eve.talkCharacter[0].GetComponent<FLookAnimator>().ObjectToFollow = Camera.main.transform;
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(3, 80);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().anim.SetLayerWeight(5, 1);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().anim.SetInteger("EmotionId", 4);
                HoiThoai("Lily", "LilySad",
                     "Sorry for putting you in danger! Are you alright?");
                RelationshipSystem.IncreaseRelationship("Lily", 5);
                break;
            case 1:
                HoiThoai("???", "Reid", 
                    "No need to worry! But you need to explain me everything!");
                break;
            case 2:
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(3, 0);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().anim.SetInteger("EmotionId", 0);
                HoiThoai("Lily", "Lily",
                     "Right! But first...");
                break;
            case 3:
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(6, 80);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(25, 80);
                HoiThoai("Lily", "LilySmile",
                     "Do you remember your name?");
                break;
            case 4:
                HoiThoai("???", "Reid",
                    "My name?");
                break;
            case 5:
                HoiThoai("Lily", "LilySmile",
                    "Yes, your name.");
                break;
            case 6:
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(6, 80);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(25, 80);
                HoiThoai("Reid", "Reid",
                    "I'm Reid!");
                break;
            case 7:
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(5, 60);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(25, 50);
                HoiThoai("Lily", "lily",
                    "Is that... your real name?");
                break;
            case 8:
                HoiThoai("Reid", "Reid",
                    "...");
                break;
            case 9:
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(5, 0);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(25, 0);
                HoiThoai("Reid", "Reid",
                    "That my real name!");
                break;
            case 10:
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(3, 60);
                HoiThoai("Lily", "lilySad",
                    "Um... Reid?");
                break;
            case 11:
                HoiThoai("Reid", "Reid",
                    "What wrong?");
                break;
            case 12:
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(3, 30);
                HoiThoai("Lily", "Lily",
                    "Nevermind, I just feel like something's not right!");
                break;
            case 13:
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(3, 0);
                HoiThoai("Reid", "Reid",
                    "Just ignore it, can you explain it to me now?");
                break;
            case 14:
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(0, 50);
                HoiThoai("Lily", "Lily",
                    "Okay but I don't know that much!");
                break;
            case 15:
                HoiThoai("Lily", "Lily",
                    "Like I said before, we are the failed test subject, the organization decided to kill us!");
                break;
            case 16:
                HoiThoai("Lily", "Lily",
                   "No one can help us, we need to survive together!");
                break;
            case 17:
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(0, 0);
                HoiThoai("Reid", "Reid",
                    "Is that all?");      
                break;
            case 18:
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(3, 50);
                HoiThoai("Lily", "Lily",
                    "Yes, That's all I know!");
                break;
            case 19:
                gc.eve.enableNext = false;
                gc.eve.luaChon1.text = "Agree!";
                gc.eve.luaChon2.text = "I don't trust you!";
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().anim.SetInteger("EmotionId", 4);
                HoiThoai("Lily", "Lily",
                    "Can you cooperate with me? To survive!");
                gc.eve.luaChonPanel.SetActive(true);
                gc.eve.choiceNumAdd = 3;
                break;
            case 20:
                gc.eve.luaChonPanel.SetActive(false);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(3, 0);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().anim.SetInteger("EmotionId", 0);
                HoiThoai("Reid", "Reid",
                    "Okay! Nice to working with you, Lily!");
                break;
            case 21:
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(8, 20);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(15, 10);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(25, 80);
                HoiThoai("Lily", "LilySmile",
                    "Me too, Reid!");
                RelationshipSystem.IncreaseRelationship("Lily", 1);
                gc.eve.textNum += 14;
                break;
            case 22:
                gc.eve.luaChonPanel.SetActive(false);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(3, 0);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(9, 60);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(19, 60);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().anim.SetInteger("EmotionId", 0);
                HoiThoai("Reid", "Reid",
                    "No! I don't trust you!");
                break;
            case 23:
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(9, 0);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(19, 0);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(8, 80);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(18, 60);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(23, 60);
                HoiThoai("Lily", "LilySad",
                    "Wh-why?");
                RelationshipSystem.DecreaseRelationship("Lily", 2);
                break;
            case 24:
                HoiThoai("Reid", "Reid",
                    "Although I have lost my memory, I have a hunch that I shouldn't trust anyone!");
                break;
            case 25:
                HoiThoai("Lily", "LilySad",
                    "But I trust you!");
                break;
            case 26:
                HoiThoai("Reid", "Reid",
                    "I don't care! I will do what I want!");
                break;
            case 27:
                HoiThoai("Lily", "LilySad",
                    "I-I mean we need to help each other to survival!");
                break;
            case 28:
                HoiThoai("Reid", "Reid",
                    "I can do it alone!");
                break;
            case 29:
                HoiThoai("Lily", "LilySad",
                    "But what about me?");
                break;
            case 30:
                HoiThoai("Reid", "Reid",
                    "Well, You don't need to do anything, I'll do it all, okay?");
                break;
            case 31:
                HoiThoai("Lily", "LilySad",
                    "Not like that! I can help you!");
                break;
            case 32:
                HoiThoai("Reid", "Reid",
                    "You saved my life so I will repay you!");
                break;
            case 33:
                HoiThoai("Reid", "Reid",
                    "Until it's over, we're done! Just don't get in my way!");
                break;
            case 34:
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(8, 0);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(18, 0);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(23, 0);
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(0, 60);
                HoiThoai("Lily", "Lily",
                    "...");
                break;
            case 35:
                HoiThoai("Lily", "Lily",
                    "Okay! If that's what you want!");
                break;
            case 36:
                HoiThoai("Lily", "Lily",
                    "I will return to my room, if you need anything just call me!");
                break;
            case 37:
                gc.ChangeCameraToPlayer2d();
                CompleteEvent(true);
                GameObject lily = gc.eve.talkCharacter[0];
                lily.GetComponent<FLookAnimator>().ObjectToFollow = gc.viewObj.player2d.transform.Find("PointLightFace");
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().anim.SetLayerWeight(5, 0);
                //Move Lily
                GameObject posMove = lily.transform.parent.Find("LilyMovePos").gameObject;
                lily.GetComponent<CharacterObject>().postionMove = posMove;
                lily.GetComponent<CharacterObject>().objOnTheRight = -1;
                lily.GetComponent<CharacterObject>().movePos = true;
                yield return new WaitForSeconds(1.5f);
                lily.SetActive(false);
                posMove.SetActive(false);
                break;
            case 38:
                gc.eve.luaChonPanel.SetActive(false);
                gc.eve.hoiThoaiPanel.SetActive(true);
                gc.eve.talkCharacter[0].GetComponent<FLookAnimator>().ObjectToFollow = Camera.main.transform;
                gc.eve.talkCharacter[0].GetComponent<CharacterObject>().anim.SetLayerWeight(5, 1);
                if (PlayerPrefs.GetInt("LilyRelationship") >= 5)
                {
                    gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(8, 20);
                    gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(15, 10);
                    gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(25, 80);
                    HoiThoai("Lily", "LilySmile", 
                        "So you've come!");
                }
                else
                {
                    gc.eve.talkCharacter[0].GetComponent<CharacterObject>().ThayDoiBieuCam(0, 60);
                    HoiThoai("Lily", "LilyAngry", 
                        "What do you want? I thought you will do it alone!");
                }
                break;
            case 39:
                HoiThoai("Reid", "Reid",
                    "Sorry! But can you tell me what's next?");
                break;
            case 40:
                if (PlayerPrefs.GetInt("LilyRelationship") >= 5)
                {
                    
                    HoiThoai("Lily", "LilySmile", 
                        "Go to the Teleport Machine and type 198-307-485.");
                }
                else
                {
                    HoiThoai("Lily", "LilyAngry", 
                        "198-307-485! Try to understand that!");
                }
                break;
            case 41:
                if (PlayerPrefs.GetInt("LilyRelationship") >= 5)
                {

                    HoiThoai("Lily", "LilySmile",
                        "Good luck and be safe!");
                }
                else
                {
                    HoiThoai("Lily", "LilyAngry",
                        "Bye!");
                }
                break;
            case 42:
                CompleteEvent(false);
                break;
        }
    }
    #endregion
}
